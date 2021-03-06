/*
 * Bachelor of Software Engineering
 * Media Design School
 * Auckland
 * New Zealand
 * 
 * (c) 2022 Media Design School
 * 
 * File Name: Graph.cs
 * Description: 
 * Author: Nerys Thamm
 * Mail: nerysthamm@gmail.com
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Linq;
using UnityEditor.Callbacks;

namespace NodeAI
{
    /// <summary>
    /// The EditorWindow for the NodeAI Graph editor.
    /// </summary>
    /// <para>
    /// This class is used to create a GraphView and handle all of the events that occur in the GraphView.
    /// </para>
    /// <para>
    /// This class is also used to create a new NodeAI_Behaviour and save it to the project.
    /// </para>
    /// <para>
    /// This class is also used to load a NodeAI_Behaviour from the project.
    /// </para>
    public class Graph : EditorWindow
    {
        private GraphView graphView; ///< The GraphView that is currently being edited.
        private NodeAI_Behaviour behaviour; ///< The NodeAI_Behaviour that is currently being edited.
        private NodeAI_Behaviour runtimeBehaviour; ///< The NodeAI_Behaviour that has been selected at runtime, used for visual debugging.
        
        private ObjectField behaviourField; ///< The ObjectField that is used to select a NodeAI_Behaviour.

        private float timeSinceLastDraw = 0f;
        private int lastChildIndex = 0;

        /// <summary>
        ///  This function is called when the Graph editor is opened.
        /// </summary>
        [MenuItem("Window/NodeAI/Graph")]
        public static void OpenGraphWindow()
        {
            Graph window = (Graph)EditorWindow.GetWindow(typeof(Graph));
            window.titleContent = new GUIContent("NodeAI Graph");
            window.Show();
        }

        /// <summary>
        ///  This function is called when a NodeAI_Behaviour asset is opened in the project view.
        /// </summary>
        /// <para>
        /// It is responsible for opening the NodeAI Graph Editor window and loading the selected NodeAI_Behaviour asset.
        /// </para>
        /// <param name="instanceId"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        [OnOpenAsset]
        public static bool OnOpenAsset(int instanceId, int line) {
            if(Selection.activeObject is NodeAI_Behaviour)
            {
                Graph window = (Graph)EditorWindow.GetWindow(typeof(Graph));
                window.titleContent = new GUIContent("NodeAI Graph");
                window.behaviour = Selection.activeObject as NodeAI_Behaviour;

                Serializer.GetInstance(window.graphView).Deserialize(window.behaviour);
                window.behaviourField.value = window.behaviour;
                window.Show();
                return true;
            }
            return false;
        }

        /// <summary>
        ///  This function is called when the Graph editor is opened.
        /// </summary>
        private void OnEnable()
        {
            graphView = new GraphView();
            graphView.StretchToParentSize();
            rootVisualElement.Add(graphView);
            GenerateBlackboard();
            GenerateToolbar();
            if(behaviour != null)
            {
                Serializer.GetInstance(graphView).Deserialize(behaviour);
            }
        }

        /// <summary>
        ///  This function is called every frame.
        /// </summary>
        void Update()
        {
            if(Selection.activeGameObject != null)
            {
                if(runtimeBehaviour == null && Selection.activeGameObject.GetComponent<NodeAI_Agent>() && Selection.activeGameObject.GetComponent<NodeAI_Agent>().AI_Behaviour == behaviour)
                {
                    runtimeBehaviour = Selection.activeGameObject.GetComponent<NodeAI_Agent>().behaviour;
                    //Debug.Log("runtime behaviour set");
                }
                else
                {
                    runtimeBehaviour = null;
                    //Debug.Log("runtime behaviour unset");
                }
            }
            else
            {
                runtimeBehaviour = null;
                //Debug.Log("runtime behaviour reset");
            }
            if(Time.frameCount % 1 == 0)
            {
                if(runtimeBehaviour != null)
                {
                    foreach(NodeData data in runtimeBehaviour.nodeData)
                    {
                        if(data.runtimeLogic != null)
                        {
                            //Find node with matching GUID
                            Node n = (Node)graphView.nodes.ToList().Where(x => ((Node)x).GUID == data.GUID).First();
                            n.mainContainer.style.borderBottomWidth = 3;
                            switch(data.runtimeLogic.state)
                            {
                                case NodeData.State.Running:
                                    n.mainContainer.style.borderBottomColor = Color.yellow;
                                    break;
                                case NodeData.State.Success:
                                    n.mainContainer.style.borderBottomColor = Color.green;
                                    break;
                                case NodeData.State.Failure:
                                    n.mainContainer.style.borderBottomColor = Color.red;
                                    break;
                                case NodeData.State.Idle:
                                    n.mainContainer.style.borderBottomColor = Color.white;
                                    break;
                            }
                        }
                    }
                }

                if(!Application.isPlaying)
                {
                    foreach(var n in graphView.nodes.ToList())
                    {
                        if(n is Node node)
                        {
                            node.mainContainer.style.borderBottomColor = Color.black;
                            node.mainContainer.style.borderBottomWidth = 0;
                        }
                    }
                }
            }
            timeSinceLastDraw += Time.deltaTime;
            if(graphView.currHoveredNode != null)
            {
                if(graphView.currHoveredNode is Node)
                {
                    //Tooltip



                    //Animation.
                    Node n = graphView.currHoveredNode;
                    if(timeSinceLastDraw < 0.5f) return;
                    if(n.nodeType == NodeData.Type.Selector || n.nodeType == NodeData.Type.Sequence)
                    {
                        int childCount = n.outputPort.connections.Count();
                        List<Node> children = n.outputPort.connections.Select(x => (Node)x.input.node).ToList().OrderBy(x => x.GetPosition().y).ToList();
                        if(childCount > 0)
                        {
                            int currChildToAnimate = Mathf.FloorToInt(Time.realtimeSinceStartup / 0.5f) % childCount;
                            for(int i = 0; i < childCount; i++)
                            {
                                if(i == currChildToAnimate)
                                {
                                    children[i].mainContainer.style.backgroundColor = Color.white;
                                }
                                else
                                {
                                    children[i].mainContainer.style.backgroundColor = Color.gray;
                                }
                            }
                        }
                        

                        
                    }
                    else if(n.nodeType == NodeData.Type.Parallel)
                    {
                        n.outputPort.connections.ToList().ForEach(x => ((Node)x.input.node).mainContainer.style.backgroundColor = Color.white);
                    }

                    
                }
            }
            Repaint();
        }

        /// <summary>
        ///  This function is used to create a new NodeAI_Behaviour asset.
        /// </summary>
        private void CreateNewBehaviour()
        {
            GraphView newGraph = new GraphView();
            newGraph.AddEntryNode();
            var serializer = Serializer.GetInstance(newGraph);
            behaviour = ScriptableObject.CreateInstance<NodeAI_Behaviour>();
            ProjectWindowUtil.CreateAsset(behaviour, "NodeAI_Behaviour.asset");
            AssetDatabase.SaveAssets();
            serializer.Serialize(behaviour);
            AssetDatabase.Refresh();
            EditorUtility.SetDirty(behaviour);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Serializer.GetInstance(graphView).Deserialize(behaviour);
        }

        /// <summary>
        ///  Called when the editor window is closed.
        /// </summary>
        private void OnDisable()
        {
            //GraphView graphView = rootVisualElement.GetFirstAncestorOfType<GraphView>();
            graphView.RemoveFromHierarchy();
        }

        /// <summary>
        ///  This function is used to generate the toolbar for the editor window.
        /// </summary>
        private void GenerateToolbar()
        {
            var toolbar = new Toolbar();
            var saveButton = new ToolbarButton(() => { 
                if(behaviour != null)
                {
                    Serializer.GetInstance(graphView).Serialize(behaviour);
                    EditorUtility.SetDirty(behaviour);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
            });
            saveButton.text = "Save";
            toolbar.Add(saveButton);

            var newBehaviourButton = new ToolbarButton(() => { CreateNewBehaviour(); });
            newBehaviourButton.text = "New Behaviour";
            toolbar.Add(newBehaviourButton);

            behaviourField = new ObjectField();
            behaviourField.objectType = typeof(NodeAI_Behaviour);
            behaviourField.allowSceneObjects = false;
            behaviourField.value = behaviour;
            behaviourField.RegisterValueChangedCallback(evt => {
                behaviour = (NodeAI_Behaviour)evt.newValue;
                if(behaviour != null)
                {
                    Serializer.GetInstance(graphView).Deserialize(behaviour);
                }
            });
            
            
            toolbar.Add(behaviourField);

            
            rootVisualElement.Add(toolbar);


        }

        
        /// <summary>
        ///  This function is used to generate the Blackboard for the editor window.
        /// </summary>
        private void GenerateBlackboard()
        {
            var blackboard = new Blackboard(graphView);
            blackboard.title = "Parameters:";
            blackboard.Add(new BlackboardSection{ title = "Exposed Properties" });
            blackboard.addItemRequested = _blackboard =>
            {
                graphView.AddBlackboardSearchWindow(GUIUtility.GUIToScreenPoint(Event.current.mousePosition));
            };
            blackboard.editTextRequested = (bb, element, newVal) =>
            {
                var oldName = ((BlackboardField)element).text;
                if(graphView.exposedProperties.Any(x => x.name == newVal))
                {
                    EditorUtility.DisplayDialog("Error", "Property with name " + newVal + " already exists", "OK");
                    return;
                }

                var index = graphView.exposedProperties.FindIndex(x => x.name == oldName);
                graphView.exposedProperties[index].name = newVal;
                ((BlackboardField)element).text = newVal;

                graphView.nodes.ForEach(x =>
                {
                    if(((Node)x).paramReference == graphView.exposedProperties[index].GUID)
                    {
                        ((Node)x).title = graphView.exposedProperties[index].name;
                    }
                });
            };

            blackboard.SetPosition(new Rect(10, 30, 275, 200));
            graphView.blackboard = blackboard;
            graphView.Add(blackboard);
        }



        
    }
}
