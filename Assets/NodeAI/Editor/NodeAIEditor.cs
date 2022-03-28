#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Events;


//namespace NodeAI;
namespace NodeAI
{

public class NodeAIEditor : EditorWindow
{
    //Node UI

    [SerializeField]
    private GUIStyle style, startNodeStyle, startNodeSelectedStyle;
    [SerializeField]
    private GUIStyle inputStyle, outputStyle, selectedStyle;

    private LinkPoint selectedInput, selectedOutput;

    private Vector2 offset;

    private LinkPointEvent OnInputEvent, OnOutputEvent;
    private LinkEvent OnLinkEvent;

    private NodeEvent OnNodeEvent;


    //Window UI

    private bool creatingNewObj = false;
    private string newObjName = "";

    //Internal Data
    private AIController controller;
    private SerializedObject serializedController;

    
    private List<AIController.Parameter> parameters;

    [MenuItem("Window/NodeAI")]
    private static void OpenWindow()
    {
        NodeAIEditor window = GetWindow<NodeAIEditor>();
        window.titleContent = new GUIContent("NodeAI");
    }

    // OnEnable
    //Description: Called when the window is opened
    private void OnEnable()
    {
        if(OnInputEvent == null) OnInputEvent = new LinkPointEvent();
        if(OnOutputEvent == null) OnOutputEvent = new LinkPointEvent();
        if(OnLinkEvent == null) OnLinkEvent = new LinkEvent();
        if(OnNodeEvent == null) OnNodeEvent = new NodeEvent();

        OnInputEvent.AddListener(OnClickInput);
        OnOutputEvent.AddListener(OnClickOutput);
        OnLinkEvent.AddListener(RemoveLink);
        OnNodeEvent.AddListener(OnRemoveNode);
        if(style == null)
        {
            style = new GUIStyle();
            style.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1.png") as Texture2D;
            style.border = new RectOffset(12, 12, 12, 12);
            style.alignment = TextAnchor.MiddleCenter;

            startNodeStyle = new GUIStyle();
            startNodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node2.png") as Texture2D;
            startNodeStyle.border = new RectOffset(12, 12, 12, 12);
            startNodeStyle.alignment = TextAnchor.MiddleCenter;

            startNodeSelectedStyle = new GUIStyle();
            startNodeSelectedStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node2 on.png") as Texture2D;
            startNodeSelectedStyle.border = new RectOffset(12, 12, 12, 12);
            startNodeSelectedStyle.alignment = TextAnchor.MiddleCenter;

            inputStyle = new GUIStyle();
            inputStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn left.png") as Texture2D;
            inputStyle.active.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn left on.png") as Texture2D;
            inputStyle.border = new RectOffset(4, 4, 12, 12);
            

            outputStyle = new GUIStyle();
            outputStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn right.png") as Texture2D;
            outputStyle.active.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn right on.png") as Texture2D;
            outputStyle.border = new RectOffset(4, 4, 12, 12);


            selectedStyle = new GUIStyle();
            selectedStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1 on.png") as Texture2D;
            selectedStyle.border = new RectOffset(12, 12, 12, 12);
            selectedStyle.alignment = TextAnchor.MiddleCenter;
        }
        if(controller != null)
        {
            foreach(Node node in controller.nodes)
            {
                if(node.miscOutput != null)
                {
                    node.miscOutput.node = node;
                }
                if(node.seqInput != null)
                {
                    node.seqInput.node = node;
                }
                if(node.seqOutput != null)
                {
                    node.seqOutput.node = node;
                }
                if(node.conditionTrueOutput != null)
                {
                    node.conditionTrueOutput.node = node;
                }
                if(node.conditionFalseOutput != null)
                {
                    node.conditionFalseOutput.node = node;
                }
                if(node.fields != null)
                {
                    foreach(Node.NodeField field in node.fields)
                    {
                        if(field.output != null)
                        {
                            field.output.node = node;
                        }
                        if(field.input != null)
                        {
                            field.input.node = node;
                        }
                    }
                }
            }
        }
        if(controller != null)
        {
            foreach(Node n in controller.nodes)
            {
                n.RelinkEvents(OnInputEvent, OnOutputEvent, OnNodeEvent);
                n.ReconnectLinks(controller);
            }
            foreach(Link l in controller.links)
            {
                l.RelinkEvents(OnLinkEvent);
            }
        }
    }

    //OnGUI
    //Description: Called when the window is drawn
    private void OnGUI()
    {
        if(controller != null)
        {
            DrawGrid(20, 0.2f, Color.gray);
            DrawGrid(100, 0.4f, Color.gray);
            
            DrawNodes();
            DrawLinks();
            DrawLinkLine(Event.current);
            ProcessNodeEvents(Event.current);
            ProcessEvents(Event.current);
        }
        DrawUI();
        if(controller != null)
        {
            if(serializedController == null)
            {
                serializedController = new SerializedObject(controller);
                //parameters = serializedController.FindProperty("parameters").;
            }
            if(controller.nodes == null)
            {
                controller.nodes = new List<Node>();
            }
            if(controller.links == null)
            {
                controller.links = new List<Link>();
            }
            if(controller.nodes.Count == 0)
            {
                Node node = new Node(new Vector2(200, 200), 200, 50, startNodeStyle, startNodeSelectedStyle, outputStyle, OnOutputEvent);
                controller.AddNode(node);
            }
            foreach(Node node in controller.nodes)
            {
                if(node.miscOutput != null)
                {
                    node.miscOutput.node = node;
                }
                if(node.seqInput != null)
                {
                    node.seqInput.node = node;
                }
                if(node.seqOutput != null)
                {
                    node.seqOutput.node = node;
                }
                if(node.conditionTrueOutput != null)
                {
                    node.conditionTrueOutput.node = node;
                }
                if(node.conditionFalseOutput != null)
                {
                    node.conditionFalseOutput.node = node;
                }
                if(node.fields != null)
                {
                    foreach(Node.NodeField field in node.fields)
                    {
                        if(field.output != null)
                        {
                            field.output.node = node;
                        }
                        if(field.input != null)
                        {
                            field.input.node = node;
                        }
                    }
                }
            }
        }

        

        if(GUI.changed) Repaint();
    }

    //DrawUI
    //Description: Draws the UI for the window
    private void DrawUI()
    {
        EditorGUILayout.BeginHorizontal();
        if(creatingNewObj == false && GUILayout.Button("New"))
        {
            creatingNewObj = true;
        }
        if(creatingNewObj == true)
        {
            GUILayout.Label("Name");
            newObjName = GUILayout.TextField(newObjName);
            if(GUILayout.Button("Create") && newObjName != "")
            {
                creatingNewObj = false;
                CreateNewAIController(newObjName);
                newObjName = "";
                
                
                
            }
            if(GUILayout.Button("Cancel"))
            {
                creatingNewObj = false;
            }
        }
        else
        {
            AIController cache = controller;
            controller = EditorGUILayout.ObjectField(controller, typeof(AIController), true) as AIController;
            if(controller != cache)
            {
                if(controller != null)
                {
                    foreach(Node n in controller.nodes)
                    {
                        n.RelinkEvents(OnInputEvent, OnOutputEvent, OnNodeEvent);
                        n.ReconnectLinks(controller);
                    }
                    foreach(Link l in controller.links)
                    {
                        l.RelinkEvents(OnLinkEvent);
                    }
                }
            }
            if(GUILayout.Button("Save") && controller != null)
            {
                EditorUtility.SetDirty(controller);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }
        
        EditorGUILayout.EndHorizontal();
        

        
    }
    
    
    //CreateNewAIController
    //Description: Creates a new AIController
    private void CreateNewAIController(string name)
    {
        controller = ScriptableObject.CreateInstance<AIController>();
        
        //AssetDatabase.CreateAsset(controller, "Assets/Resources/AI/" + name + ".asset");
        ProjectWindowUtil.CreateAsset(controller, newObjName + "_AICtrl.asset");
        AssetDatabase.Refresh();
        EditorUtility.SetDirty(controller);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
    


//NODE UI CODE
    //DrawGrid
    //Parameters:
    //  float gridSpacing: The spacing between each grid line
    //  float gridOpacity: The opacity of the grid lines
    //  Color gridColor: The color of the grid lines
    //Description: Draws the grid lines
    private void DrawGrid(float gridSpacing, float gridOpacity, Color gridColor)
    {
        int widthDivs = Mathf.CeilToInt(position.width / gridSpacing);
        int heightDivs = Mathf.CeilToInt(position.height / gridSpacing);

        Handles.BeginGUI();
        Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);

        
        Vector3 newOffset = new Vector3(offset.x % gridSpacing, offset.y % gridSpacing, 0);

        for(int i = 0; i < widthDivs; i++)
        {
            Handles.DrawLine(new Vector3(gridSpacing * i, -gridSpacing, 0) + newOffset, new Vector3(gridSpacing * i, position.height, 0f) + newOffset);
        }

        for(int j = 0; j < heightDivs; j++)
        {
            Handles.DrawLine(new Vector3(-gridSpacing, gridSpacing * j, 0) + newOffset, new Vector3(position.width, gridSpacing * j, 0f) + newOffset);
        }

        Handles.color = Color.white;
        Handles.EndGUI();
    }

    //DrawLinkLine
    //Parameters:
    //  Event e: The current event
    //Description: Draws the line between the nodes
    private void DrawLinkLine(Event e)
    {
        if(selectedInput != null && selectedOutput == null)
        {
            Handles.DrawBezier(
                selectedInput.rect.center,
                e.mousePosition,
                selectedInput.rect.center + Vector2.left * 50f,
                e.mousePosition - Vector2.left * 50f,
                Color.white,
                null,
                2f
            );

            GUI.changed = true;
        }

        if(selectedInput == null && selectedOutput != null)
        {
            Handles.DrawBezier(
                e.mousePosition,
                selectedOutput.rect.center,
                e.mousePosition + Vector2.left * 50f,
                selectedOutput.rect.center - Vector2.left * 50f,
                Color.white,
                null,
                2f
            );

            GUI.changed = true;
        }
    }

    //DrawNodes
    //Description: Draws all the nodes
    private void DrawNodes()
    {
        if(controller.nodes != null)
        {
            foreach(Node node in controller.nodes)
            {
                node.Draw();
            }
        }
    }

    //DrawLinks
    //Description: Draws all the links
    private void DrawLinks()
    {
        if(controller.links != null)
        {
            foreach(Link link in controller.links.ToArray())
            {
                link.Draw();
            }
        }
    }

    //ProcessEvents
    //Parameters:
    //  Event e: The current event
    //Description: Processes all the events
    private void ProcessEvents(Event e)
    {

        switch(e.type)
        {
            case EventType.MouseDown:
                if(e.button == 1)
                {
                    ProcessContextMenu(e.mousePosition);
                }
                if(e.button == 0)
                {
                    selectedInput = null;
                    selectedOutput = null;
                }
                break;
            case EventType.MouseDrag:
                if(e.button == 2)
                {
                    OnMoveCanvas(e.delta);
                    EditorUtility.SetDirty(controller);
                }
                break;
        }
    }

    //OnMoveCanvas
    //Parameters:
    //  Vector2 delta: The amount to move the canvas
    //Description: Moves the canvas
    private void OnMoveCanvas(Vector2 delta)
    {
        offset += delta;

        if(controller.nodes != null)
        {
            foreach(Node node in controller.nodes)
            {
                node.Move(delta);
            }
        }

        GUI.changed = true;
    }

    //ProcessNodeEvents
    //Parameters:
    //  Event e: The current event
    //Description: Processes all the node events
    private void ProcessNodeEvents(Event e)
    {
        if(controller.nodes != null)
        {
            for(int i = controller.nodes.Count - 1; i >= 0; i--)
            {
                bool guiChanged = controller.nodes[i].ProcessEvents(e);

                if(guiChanged)
                {
                    GUI.changed = true;
                }
            }
        }
    }

    //GenerateRandomString
    //Parameters:
    //  int length: The length of the string
    //Description: Generates a random string
    private string GenerateRandomString(int length)
    {
        string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        string result = "";
        for(int i = 0; i < length; i++)
        {
            result += chars[Random.Range(0, chars.Length)];
        }
        return result;
    }

    //ProcessContextMenu
    //Parameters:
    //  Vector2 mousePosition: The position of the mouse
    //Description: Processes the context menu
    private void ProcessContextMenu(Vector2 mousePosition)
    {
        GenericMenu genericMenu = new GenericMenu();
        genericMenu.AddItem(new GUIContent("Add Node/State/IDLE"), false, () =>
        {
            if(controller.nodes == null) controller.nodes = new List<Node>();
            Node newNode = new Node(mousePosition, 200, 100, style, selectedStyle, inputStyle, outputStyle, OnInputEvent, OnOutputEvent, OnNodeEvent, Node.NodeType.State);
            newNode.stateType = Node.StateType.Idle;
            
            controller.AddNode(newNode);
        });
        genericMenu.AddItem(new GUIContent("Add Node/State/SEEK"), false, () =>
        {
            if(controller.nodes == null) controller.nodes = new List<Node>();
            Node newNode = new Node(mousePosition, 200, 100, style, selectedStyle, inputStyle, outputStyle, OnInputEvent, OnOutputEvent, OnNodeEvent, Node.NodeType.State);
            
            newNode.stateType = Node.StateType.Seek;
            
            controller.AddNode(newNode);
        });
        genericMenu.AddItem(new GUIContent("Add Node/State/FLEE"), false, () =>
        {
            if(controller.nodes == null) controller.nodes = new List<Node>();
            Node newNode = new Node(mousePosition, 200, 100, style, selectedStyle, inputStyle, outputStyle, OnInputEvent, OnOutputEvent, OnNodeEvent, Node.NodeType.State);
            
            newNode.stateType = Node.StateType.Flee;
            
            controller.AddNode(newNode);
        });
        genericMenu.AddItem(new GUIContent("Add Node/State/WANDER"), false, () =>
        {
            if(controller.nodes == null) controller.nodes = new List<Node>();
            Node newNode = new Node(mousePosition, 200, 100, style, selectedStyle, inputStyle, outputStyle, OnInputEvent, OnOutputEvent, OnNodeEvent, Node.NodeType.State);
            
            newNode.stateType = Node.StateType.Wander;
            
            controller.AddNode(newNode);
        });
        genericMenu.AddItem(new GUIContent("Add Node/State/CUSTOM"), false, () =>
        {
            if(controller.nodes == null) controller.nodes = new List<Node>();
            Node newNode = new Node(mousePosition, 200, 100, style, selectedStyle, inputStyle, outputStyle, OnInputEvent, OnOutputEvent, OnNodeEvent, Node.NodeType.State);
            
            newNode.stateType = Node.StateType.Custom;
            
            controller.AddNode(newNode);
        });
        genericMenu.AddItem(new GUIContent("Add Node/Condition"), false, () =>
        {
            if(controller.nodes == null) controller.nodes = new List<Node>();
            Node newNode = new Node(mousePosition, 200, 100, style, selectedStyle, inputStyle, outputStyle, OnInputEvent, OnOutputEvent, OnNodeEvent, Node.NodeType.Condition);
            
            
            controller.AddNode(newNode);
        });
        genericMenu.AddItem(new GUIContent("Add Node/Action"), false, () =>
        {
            if(controller.nodes == null) controller.nodes = new List<Node>();
            Node newNode = new Node(mousePosition, 200, 100, style, selectedStyle, inputStyle, outputStyle, OnInputEvent, OnOutputEvent, OnNodeEvent, Node.NodeType.Action);
            
            
            controller.AddNode(newNode);
        });
        genericMenu.AddItem(new GUIContent("Add Node/Delay"), false, () =>
        {
            if(controller.nodes == null) controller.nodes = new List<Node>();
            Node newNode = new Node(mousePosition, 200, 100, style, selectedStyle, inputStyle, outputStyle, OnInputEvent, OnOutputEvent, OnNodeEvent, Node.NodeType.Delay);
            
            
            controller.AddNode(newNode);
        });
        genericMenu.AddItem(new GUIContent("Add Node/Parameter"), false, () =>
        {
            if(controller.nodes == null) controller.nodes = new List<Node>();
            Node newNode = new Node(mousePosition, 200, 100, style, selectedStyle, inputStyle, outputStyle, OnInputEvent, OnOutputEvent, OnNodeEvent, Node.NodeType.Parameter, false);
            
            controller.nodes[controller.nodes.Count - 1].parameter = new AIController.Parameter();
            if(controller.parameters == null) controller.parameters = new List<AIController.Parameter>();
            controller.parameters.Add(controller.nodes[controller.nodes.Count - 1].parameter);
            
            controller.AddNode(newNode);
        });
        genericMenu.AddItem(new GUIContent("Add Node/Logic/AND"), false, () =>
        {
            if(controller.nodes == null) controller.nodes = new List<Node>();
            Node newNode = new Node(mousePosition, 200, 100, style, selectedStyle, inputStyle, outputStyle, OnInputEvent, OnOutputEvent, OnNodeEvent, Node.NodeType.Logic, false);
            
            newNode.logicType = Node.LogicType.AND;
            
            controller.AddNode(newNode);
        });
        genericMenu.AddItem(new GUIContent("Add Node/Logic/OR"), false, () =>
        {
            if(controller.nodes == null) controller.nodes = new List<Node>();
            Node newNode = new Node(mousePosition, 200, 100, style, selectedStyle, inputStyle, outputStyle, OnInputEvent, OnOutputEvent, OnNodeEvent, Node.NodeType.Logic, false);
            
            newNode.logicType = Node.LogicType.OR;
            
            controller.AddNode(newNode);
        });
        genericMenu.AddItem(new GUIContent("Add Node/Logic/NOT"), false, () =>
        {
            if(controller.nodes == null) controller.nodes = new List<Node>();
            Node newNode = new Node(mousePosition, 200, 100, style, selectedStyle, inputStyle, outputStyle, OnInputEvent, OnOutputEvent, OnNodeEvent, Node.NodeType.Logic, false);
            
            newNode.logicType = Node.LogicType.NOR;
            
            controller.AddNode(newNode);
        });
        genericMenu.AddItem(new GUIContent("Add Node/Logic/XOR"), false, () =>
        {
            if(controller.nodes == null) controller.nodes = new List<Node>();
            Node newNode = new Node(mousePosition, 200, 100, style, selectedStyle, inputStyle, outputStyle, OnInputEvent, OnOutputEvent, OnNodeEvent, Node.NodeType.Logic, false);
            newNode.logicType = Node.LogicType.XOR;
            
            controller.AddNode(newNode);
        });
        genericMenu.AddItem(new GUIContent("Add Node/Comparison"), false, () =>
        {
            if(controller.nodes == null) controller.nodes = new List<Node>();
            Node newNode = new Node(mousePosition, 200, 100, style, selectedStyle, inputStyle, outputStyle, OnInputEvent, OnOutputEvent, OnNodeEvent, Node.NodeType.Comparison, false);
            
            newNode.comparisonType = Node.ComparisonType.Equal;
            
            controller.AddNode(newNode);
        });

        
        genericMenu.ShowAsContext();
    }

    //OnClickInput
    //Parameters:
    //  LinkPoint: The link point that was clicked
    //Description:
    //  This function is called when a link point is clicked.
    private void OnClickInput(LinkPoint linkPoint)
    {
        selectedInput = linkPoint;

        if(selectedOutput != null && selectedInput != selectedOutput)
        {
            if(controller.links == null) controller.links = new List<Link>();
            if(selectedInput.dataType == selectedOutput.dataType)
                MakeLink();
            selectedOutput = null;
            selectedInput = null;
        }
        
        
    }

    //OnClickOutput
    //Parameters:
    //  LinkPoint: The link point that was clicked
    //Description:
    //  This function is called when a link point is clicked.
    private void OnClickOutput(LinkPoint linkPoint)
    {
        selectedOutput = linkPoint;

        if(selectedInput != null && selectedInput != selectedOutput)
        {
            if(controller.links == null) controller.links = new List<Link>();
            if(selectedInput.dataType == selectedOutput.dataType)
                MakeLink();
            selectedInput = null;
            selectedOutput = null;
        }
        
        
    }

    //MakeLink
    //Description:
    //  This function makes a link between the selected input and output.
    private void MakeLink()
    {
        if(selectedOutput != null)
        {
            if(controller.links == null) controller.links = new List<Link>();
            Link newLink = new Link(selectedOutput, selectedInput, OnLinkEvent);
            controller.AddLink(newLink);
            if(selectedInput.linkIDs == null) selectedInput.linkIDs = new List<string>();
            if(selectedOutput.linkIDs == null) selectedOutput.linkIDs = new List<string>();
            selectedInput.linkIDs.Add(newLink.ID);
            selectedOutput.linkIDs.Add(newLink.ID);
            selectedOutput = null;
        }
    }

    //RemoveLink
    //Parameters:
    //  Link: The link to remove
    //Description:
    //  This function removes a link from the controller.
    private void RemoveLink(Link link)
    {
        if(controller.links.Contains(link))
        {
            link.input.linkIDs.Remove(link.ID);
            link.output.linkIDs.Remove(link.ID);
            controller.links.Remove(link);
        }
    }

    //OnRemoveNode
    //Parameters:
    //  Node: The node to remove
    //Description:
    //  This function removes a node from the controller.
    private void OnRemoveNode(Node node)
    {
        if(controller.links != null)
        {
            List<Link> linksToRemove = new List<Link>();
            for(int i = controller.links.Count - 1; i >= 0; i--)
            {
                if(controller.links[i].input.NodeID == node.ID || controller.links[i].output.NodeID == node.ID)
                {
                    linksToRemove.Add(controller.links[i]);
                }
                
            }
            foreach(Link link in linksToRemove)
            {
                RemoveLink(link);
            }
            linksToRemove = null;
        }
        if(controller.nodes.Contains(node))
        {
            if(node.type == Node.NodeType.Parameter)
            {
                controller.parameters.Remove(node.parameter);
            }
            controller.nodes.Remove(node);
        }
        if(controller.nodeDictionary != null && controller.nodeDictionary.ContainsKey(node.ID))
        {
            controller.nodeDictionary.Remove(node.ID);
        }
    }
}

}
#endif

