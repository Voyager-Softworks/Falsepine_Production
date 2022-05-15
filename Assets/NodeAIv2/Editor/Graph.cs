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
    public class Graph : EditorWindow
    {
        private GraphView graphView;
        private NodeAI_Behaviour behaviour;

        private ObjectField behaviourField;

        [MenuItem("Window/NodeAI/Graph")]
        public static void OpenGraphWindow()
        {
            Graph window = (Graph)EditorWindow.GetWindow(typeof(Graph));
            window.titleContent = new GUIContent("NodeAI Graph");
            window.Show();
        }

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

        

        private void DrawUI()
        {
            
        }

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

        
        private void ProcessEvents(Event e)
        {
            
        }

        private void ProcessContextMenu(Vector2 mousePosition)
        {
            
        }

        private void OnDisable()
        {
            //GraphView graphView = rootVisualElement.GetFirstAncestorOfType<GraphView>();
            graphView.RemoveFromHierarchy();
        }

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

        private void GenerateBlackboard()
        {
            var blackboard = new Blackboard(graphView);
            blackboard.Add(new BlackboardSection{ title = "Exposed Properties" });
            blackboard.addItemRequested = _blackboard =>
            {
                var contextMenu = new GenericMenu();
                contextMenu.AddItem(new GUIContent("String"), false, () =>
                {
                    graphView.AddPropertyToBlackboard(new NodeData.Property<string> { name = "New Property", value = "String" });
                });
                contextMenu.AddItem(new GUIContent("Int"), false, () =>
                {
                    graphView.AddPropertyToBlackboard(new NodeData.Property<int> { name = "New Property", value = 0 });
                });
                contextMenu.AddItem(new GUIContent("Float"), false, () =>
                {
                    graphView.AddPropertyToBlackboard(new NodeData.Property<float> { name = "New Property", value = 0.0f });
                });
                contextMenu.AddItem(new GUIContent("Bool"), false, () =>
                {
                    graphView.AddPropertyToBlackboard(new NodeData.Property<bool> { name = "New Property", value = false });
                });
                contextMenu.AddItem(new GUIContent("Vector2"), false, () =>
                {
                    graphView.AddPropertyToBlackboard(new NodeData.Property<Vector2> { name = "New Property", value = Vector2.zero });
                });
                contextMenu.AddItem(new GUIContent("Vector3"), false, () =>
                {
                    graphView.AddPropertyToBlackboard(new NodeData.Property<Vector3> { name = "New Property", value = Vector3.zero });
                });
                contextMenu.AddItem(new GUIContent("Vector4"), false, () =>
                {
                    graphView.AddPropertyToBlackboard(new NodeData.Property<Vector4> { name = "New Property", value = Vector4.zero });
                });
                contextMenu.AddItem(new GUIContent("Color"), false, () =>
                {
                    graphView.AddPropertyToBlackboard(new NodeData.Property<Color> { name = "New Property", value = Color.white });
                });
                contextMenu.ShowAsContext();
                
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
