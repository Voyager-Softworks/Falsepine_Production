using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Events;

namespace NodeAI
{
[System.Serializable]
public class NodeEvent : UnityEvent<Node> { }; //Event for when a node is clicked


// Class: Node
// Description:
//     This class is used to create a node in the NodeAI.
//     It contains a list of links, and a list of parameters.
//     It also contains a list of states, which are used to
//     determine the behavior of the node.
//
[System.Serializable]
public class Node 
{
    public string ID; //Unique ID for the node
    [SerializeField]
    public Rect rect; //Rectangle for the node
    public string title; //Title of the node
    public bool isDragging; //Is the node being dragged?
    public bool selected; //Is the node selected?
    #if UNITY_EDITOR
    [SerializeField]
    public GUIStyle style; //Style of the node
    [SerializeField]
    public GUIStyle defaultStyle; //Default style of the node
    [SerializeField]
    public GUIStyle selectedStyle; //Selected style of the node
    #endif
    [SerializeField]
    public LinkPoint seqInput; //Sequence input
    [SerializeField]
    public LinkPoint seqOutput; //Sequence output
    [SerializeField]
    public LinkPoint miscOutput; //Misc output
    [SerializeField]
    public LinkPoint conditionTrueOutput, conditionFalseOutput; //Condition outputs
    [SerializeField]
    public NodeEvent OnRemove; //Event for when the node is removed
    [SerializeField]
    public List<NodeField> fields; //List of fields

    //Enum defining possible node types
    public enum NodeType 
    {
        State, //State node: Used to control the agent with predefined behaviours
        Condition, //Condition node: Used to determine if a condition is true or false
        Action, //Action node: Used to perform an action defined within the Agent
        Logic, //Logic node: Used to do binary logic on booleans
        Delay, //Delay node: Used to delay the execution of a node
        Parameter, //Parameter node: Used to store a value
        Entry, //Entry node: Used to start the execution
        Comparison //Comparison node: Used to compare two values
    }

    //Class: NodeField
    //Description:
    //     This class is used to create a field in the Node.
    //     A field has an editable value, and optional inputs and outputs.
    //
    [Serializable]
    public class NodeField
    {
        public FieldType type; //Type of the field
        public int index = 0; //Index of the field
        public string name; //Name of the field
        public string svalue; //String value of the field
        public int ivalue; //Integer value of the field
        public float fvalue; //Float value of the field
        public bool bvalue; //Boolean value of the field
        public bool hasInput; //Does the field have an input?
        public bool hasOutput; //Does the field have an output?
        [SerializeField]
        public LinkPoint input; //Input of the field
        [SerializeField]
        public LinkPoint output; //Output of the field

        //NodeField constructor
        //Parameters:
        //      string name: Name of the field
        //      string value: Value of the field
        //      int index: Index of the field
        public NodeField(string name, string value, int index)
        {
            this.name = name;
            this.svalue = value;
            this.type = FieldType.String;
            this.index = index;
            
        }
        #if UNITY_EDITOR
        //NodeField constructor
        //Parameters:
        //      string ID: Unique ID of the parent Node
        //      string name: Name of the field
        //      int value: Value of the field
        //      bool hasInput: Does the field have an input?
        //      LinkPointEvent OnClickInput: Event for when the input is clicked
        //      GUIStyle inputStyle: Style of the input
        //      bool hasOutput: Does the field have an output?
        //      LinkPointEvent OnClickOutput: Event for when the output is clicked
        //      GUIStyle outputStyle: Style of the output
        //      int index: Index of the field
        public NodeField(string ID, string name, int value, bool hasInput, LinkPointEvent OnClickInput, GUIStyle inputStyle, bool hasOutput, LinkPointEvent OnClickOutput, GUIStyle outputStyle, int index)
        {
            this.name = name;
            this.ivalue = value;
            this.type = FieldType.Int;
            this.hasInput = hasInput;
            this.hasOutput = hasOutput;
            this.index = index;
            if(hasInput)
            {
                this.input = new LinkPoint(ID, LinkType.Input, LinkDataType.Int, inputStyle, OnClickInput);
                this.input.fieldIndex = index;
            }
            if(hasOutput)
            {
                this.output = new LinkPoint(ID, LinkType.Output, LinkDataType.Int, outputStyle, OnClickOutput);
                this.output.fieldIndex = index;
            }
        }
        
        //NodeField constructor
        //Parameters:
        //      string ID: Unique ID of the parent Node
        //      string name: Name of the field
        //      float value: Value of the field
        //      bool hasInput: Does the field have an input?
        //      LinkPointEvent OnClickInput: Event for when the input is clicked
        //      GUIStyle inputStyle: Style of the input
        //      bool hasOutput: Does the field have an output?
        //      LinkPointEvent OnClickOutput: Event for when the output is clicked
        //      GUIStyle outputStyle: Style of the output
        //      int index: Index of the field
        public NodeField(string ID, string name, float value, bool hasInput, LinkPointEvent OnClickInput, GUIStyle inputStyle, bool hasOutput, LinkPointEvent OnClickOutput, GUIStyle outputStyle, int index)
        {
            this.name = name;
            this.fvalue = value;
            this.type = FieldType.Float;
            this.hasInput = hasInput;
            this.hasOutput = hasOutput;
            this.index = index;
            if(hasInput)
            {
                this.input = new LinkPoint(ID, LinkType.Input, LinkDataType.Float, inputStyle, OnClickInput);
                this.input.fieldIndex = index;
            }
            if(hasOutput)
            {
                this.output = new LinkPoint(ID, LinkType.Output, LinkDataType.Float, outputStyle, OnClickOutput);
                this.output.fieldIndex = index;
            }
        }

        //NodeField constructor
        //Parameters:
        //      string ID: Unique ID of the parent Node
        //      string name: Name of the field
        //      bool value: Value of the field
        //      bool hasInput: Does the field have an input?
        //      LinkPointEvent OnClickInput: Event for when the input is clicked
        //      GUIStyle inputStyle: Style of the input
        //      bool hasOutput: Does the field have an output?
        //      LinkPointEvent OnClickOutput: Event for when the output is clicked
        //      GUIStyle outputStyle: Style of the output
        //      int index: Index of the field
        public NodeField(string ID, string name, bool value, bool hasInput, LinkPointEvent OnClickInput, GUIStyle inputStyle, bool hasOutput, LinkPointEvent OnClickOutput, GUIStyle outputStyle, int index)
        {
            this.name = name;
            this.bvalue = value;
            this.type = FieldType.Bool;
            this.hasInput = hasInput;
            this.hasOutput = hasOutput;
            this.index = index;
            if(hasInput)
            {
                this.input = new LinkPoint(ID, LinkType.Input, LinkDataType.Bool, inputStyle, OnClickInput);
                this.input.fieldIndex = index;
            }
            if(hasOutput)
            {
                this.output = new LinkPoint(ID, LinkType.Output, LinkDataType.Bool, outputStyle, OnClickOutput);
                this.output.fieldIndex = index;
            }
        }
        #endif
        //RelinkEvents
        //Parameters:
        //      LinkPointEvent OnClickInput: Event for when the input is clicked
        //      LinkPointEvent OnClickOutput: Event for when the output is clicked
        //Description:
        //      This function relinks the events of the input and output.
        public void RelinkEvents(LinkPointEvent OnClickInput, LinkPointEvent OnClickOutput)
        {
            this.input.ReconnectEvents(OnClickInput);
            this.output.ReconnectEvents(OnClickOutput);
        }

        //Enum defining the type of the field
        public enum FieldType
        {
            String,
            Int,
            Float,
            Bool
        }
        #if UNITY_EDITOR
        //Draw
        //Parameters:
        //      int line: Which line the field is on
        //      Rect rect: Rectangle of the field
        //Description:
        //      This function draws the field.
        public void Draw(int line, Rect rect)
        {
            EditorGUI.LabelField(new Rect(rect.x + 15 , rect.y + 5 + ( (EditorGUIUtility.singleLineHeight + 2) * (1+line)), 80, 20), name + ":");
                
            if(type == FieldType.String)
            {
                svalue = EditorGUI.TextField(new Rect(rect.x + 85, rect.y + 5 + ( (EditorGUIUtility.singleLineHeight + 2) * (1+line)), rect.width - 100, 20), svalue);
            }
            else if(type == FieldType.Int)
            {
                ivalue = EditorGUI.IntField(new Rect(rect.x + 85, rect.y + 5 + ( (EditorGUIUtility.singleLineHeight + 2) * (1+line)),rect.width - 100, 20), ivalue);
            }
            else if(type == FieldType.Float)
            {
                fvalue = EditorGUI.FloatField(new Rect(rect.x + 85, rect.y + 5 + ( (EditorGUIUtility.singleLineHeight + 2) * (1+line)),rect.width - 100, 20), fvalue);
            }
            else if(type == FieldType.Bool)
            {
                bvalue = EditorGUI.Toggle(new Rect(rect.x + 85, rect.y + 5 + ( (EditorGUIUtility.singleLineHeight + 2) * (1+line)),rect.width - 100, 20), bvalue);
            }

            if(input != null)
            {
                input.Draw(line+1, rect);
            }
            if(output != null)
            {
                output.Draw(line+1, rect);
            }
        }
        #endif
    }


    //TYPE
    public NodeType type;

    //TYPE SPECIFIC VARS:

    //Parameter Node:
    public AIController.Parameter parameter;


    //Logic Node:
    public enum LogicType
    {
        AND,
        OR,
        NOR,
        XOR
    }
    public LogicType logicType;

    //Comparison Node:
    public enum ComparisonType
    {
        Equal,
        NotEqual,
        Greater,
        Less,
        GreaterEqual,
        LessEqual
    }
    public ComparisonType comparisonType;
    

    //State Node:
    public enum StateType
    {
        Idle,
        Seek,
        Flee,
        Wander,
        Custom
    }

    public AIController.StateVars stateVars;

    public StateType stateType;

    //RelinkEvents
    //Parameters:
    //      LinkPointEvent OnClickInput: Event for when the input is clicked
    //      LinkPointEvent OnClickOutput: Event for when the output is clicked
    //      NodeEvent OnClickRemove: Event for removing the node
    //Description:
    //      This function relinks the events of the input and output.
    public void RelinkEvents(LinkPointEvent OnClickInput, LinkPointEvent OnClickOutput, NodeEvent OnClickRemove)
    {
        if(seqInput != null) seqInput.ReconnectEvents(OnClickInput);
        if(seqOutput != null) seqOutput.ReconnectEvents(OnClickOutput);
        if(miscOutput != null) miscOutput.ReconnectEvents(OnClickOutput);
        if(conditionTrueOutput != null) conditionTrueOutput.ReconnectEvents(OnClickOutput);
        if(conditionFalseOutput != null) conditionFalseOutput.ReconnectEvents(OnClickOutput);
        foreach(NodeField field in fields)
        {
            field.RelinkEvents(OnClickInput, OnClickOutput);
        }
        OnRemove = OnClickRemove;
    }

    //ReconnectLinks
    //Parameters:
    //      AIController controller: The controller to connect to
    //Description:
    //      This function reconnects the links of the node to the controller.
    public void ReconnectLinks(AIController controller)
    {
        if(seqInput != null) seqInput.ReconnectLinks(controller);
        if(seqOutput != null) seqOutput.ReconnectLinks(controller);
        if(miscOutput != null) miscOutput.ReconnectLinks(controller);
        if(conditionTrueOutput != null) conditionTrueOutput.ReconnectLinks(controller);
        if(conditionFalseOutput != null) conditionFalseOutput.ReconnectLinks(controller);
        foreach(NodeField field in fields)
        {
            if(field.input != null) field.input.ReconnectLinks(controller);
            if(field.output != null) field.output.ReconnectLinks(controller);
        }
    }
    #if UNITY_EDITOR
    //Node Constructor
    //Parameters:
    //        Vector2 position: Position of the node
    //        float width: Width of the node
    //        float height: Height of the node
    //        GUIStyle nodeStyle: Style of the node
    //        GUIStyle selectedStyle: Style of the node when selected
    //        GUIStyle inputStyle: Style of the input point
    //        GUIStyle outputStyle: Style of the output point
    //        LinkPointEvent OnClickInput: Event for when the input is clicked
    //        LinkPointEvent OnClickOutput: Event for when the output is clicked
    //        NodeEvent OnClickRemove: Event for removing the node
    //        NodeType type: Type of the node
    //        bool hasSequenceLinks: Whether the node has sequence links
    //Description:
    //      This function creates a node.
    public Node(
        Vector2 position, 
        float width, 
        float height, 
        GUIStyle nodeStyle, 
        GUIStyle selectedStyle,
        GUIStyle inputStyle, 
        GUIStyle outputStyle, 
        LinkPointEvent OnClickInput, 
        LinkPointEvent OnClickOutput,
        NodeEvent OnClickRemove,
        NodeType type,
        bool hasSequenceLinks = true
        )
    {
        rect = new Rect(position.x, position.y, width, height);
        style = nodeStyle;
        this.ID = AIController.GenerateRandomString(20);
        if(hasSequenceLinks)
        {
            seqInput = new LinkPoint(ID, LinkType.Input, LinkDataType.Sequence, inputStyle, OnClickInput);
            seqOutput = new LinkPoint(ID, LinkType.Output,  LinkDataType.Sequence, outputStyle, OnClickOutput);
        }
        
        switch(type)
        {
            case NodeType.State:
                this.type = NodeType.State;
                this.title = "State";
                this.stateVars = new AIController.StateVars();
                break;
            case NodeType.Condition:
                this.type = NodeType.Condition;
                this.title = "Condition";
                conditionTrueOutput = new LinkPoint(this.ID, LinkType.Output, LinkDataType.Sequence, outputStyle, OnClickOutput);
                conditionFalseOutput = new LinkPoint(this.ID, LinkType.Output, LinkDataType.Sequence, outputStyle, OnClickOutput);
                seqOutput = null;
                this.fields = new List<NodeField>()
                {
                    new NodeField(this.ID, "Input", false, true, OnClickInput, inputStyle, false, OnClickOutput, outputStyle, 0)
                };
                break;
            case NodeType.Action:
                this.type = NodeType.Action;
                this.title = "Action";
                this.fields = new List<NodeField>()
                {
                    new NodeField("Name", "New Action", 0)
                };
                break;
            case NodeType.Logic:
                this.type = NodeType.Logic;
                this.title = "Logic";
                this.fields = new List<NodeField>()
                {
                    new NodeField(this.ID, "Input A", false, true, OnClickInput, inputStyle, false, OnClickOutput, outputStyle, 0),
                    new NodeField(this.ID, "Input B", false, true, OnClickInput, inputStyle, false, OnClickOutput, outputStyle, 1)
                };
                this.miscOutput = new LinkPoint(this.ID, LinkType.Output, LinkDataType.Bool, outputStyle, OnClickOutput);
                break;
            case NodeType.Delay:
                this.type = NodeType.Delay;
                this.title = "Delay";
                this.fields = new List<NodeField>()
                {
                    new NodeField(this.ID, "Delay", 0.0f, true, OnClickInput, inputStyle, false, OnClickOutput, outputStyle, 0)
                };
                break;
            case NodeType.Parameter:
                if(this.parameter == null)
                {
                    this.parameter = new AIController.Parameter();
                    this.parameter.name = "Parameter";
                    this.parameter.type = AIController.Parameter.ParameterType.Float;
                    this.parameter.fvalue = 0;
                }
                this.seqOutput = null;
                this.seqInput = null;
                this.type = NodeType.Parameter;
                this.miscOutput = new LinkPoint(this.ID, LinkType.Output, (LinkDataType)this.parameter.type, outputStyle, OnClickOutput);
                this.title = "Parameter";
                this.parameter.name = "NewParam";
                break;
            case NodeType.Entry:
                this.type = NodeType.Entry;
                this.title = "Entry";
                break;
            case NodeType.Comparison:
                this.type = NodeType.Comparison;
                this.title = "Comparison";
                this.fields = new List<NodeField>()
                {
                    new NodeField(this.ID, "Input A", 0.0f, true, OnClickInput, inputStyle, false, OnClickOutput, outputStyle, 0),
                    new NodeField(this.ID, "Input B", 0.0f, true, OnClickInput, inputStyle, false, OnClickOutput, outputStyle, 1)
                };
                this.miscOutput = new LinkPoint(this.ID, LinkType.Output, LinkDataType.Bool, outputStyle, OnClickOutput);
                this.comparisonType = ComparisonType.Equal;
                break;
        }

        defaultStyle = nodeStyle;
        this.selectedStyle = selectedStyle;
        this.type = type;
        
        OnRemove = OnClickRemove;
    }

    //Node Constructor
    //Parameters:
    //        Vector2 position: Position of the node
    //        float width: Width of the node
    //        float height: Height of the node
    //        GUIStyle nodeStyle: Style of the node
    //        GUIStyle selectedStyle: Style of the node when selected
    //        GUIStyle inputStyle: Style of the input point
    //        GUIStyle outputStyle: Style of the output point
    //        LinkPointEvent OnClickOutput: Event for when the output is clicked
    //Description:
    //       Alternate constructor for the Entry node.
    public Node(
        Vector2 position, 
        float width, 
        float height, 
        GUIStyle nodeStyle, 
        GUIStyle selectedStyle, 
        GUIStyle outputStyle,  
        LinkPointEvent OnClickOutput
        )
    {
        rect = new Rect(position.x, position.y, width, height);
        style = nodeStyle;
        this.ID = "EntryNode";
        seqInput = null;
        seqOutput = new LinkPoint(this.ID, LinkType.Output, LinkDataType.Sequence, outputStyle, OnClickOutput);

        defaultStyle = nodeStyle;
        this.selectedStyle = selectedStyle;
        title = "Entry";
        
        type = NodeType.Entry;
        OnRemove = null;
    }
    
    //Move
    //Parameters:
    //        Vector2 delta: The amount to move the node
    //Description:
    //       Moves the node by the delta amount
    public void Move(Vector2 delta)
    {
        rect.position += delta;
    }


    //Draw
    //Description:
    //       Draws the node
    public void Draw()
    {
        if(seqInput != null)
            seqInput.Draw(0, rect);
        if(seqOutput != null)
            seqOutput.Draw(0, rect);
        GUI.Box(rect, "", style);
        EditorGUI.LabelField(new Rect(rect.x + 15, rect.y + 5, 80, 20), title);
        switch(type)
        {
            case NodeType.State:
                switch(this.stateType)
                {
                    case StateType.Wander:
                        title = "State: Wander";
                        EditorGUI.LabelField(new Rect(rect.x + 15 , rect.y + 5 + (EditorGUIUtility.singleLineHeight * 1), 80, 20), "Radius:");
                        stateVars.radius = EditorGUI.FloatField(new Rect(rect.x + 85, rect.y + 5 + ( EditorGUIUtility.singleLineHeight * 1), rect.width - 100, 20),  stateVars.radius);
                        EditorGUI.LabelField(new Rect(rect.x + 15 , rect.y + 5 + (EditorGUIUtility.singleLineHeight * 2), 80, 20), "Speed:");
                        stateVars.speed = EditorGUI.FloatField(new Rect(rect.x + 85, rect.y + 5 + ( EditorGUIUtility.singleLineHeight * 2), rect.width - 100, 20),  stateVars.speed);
                        break;
                    case StateType.Flee:
                        title = "State: Flee";
                        EditorGUI.LabelField(new Rect(rect.x + 15 , rect.y + 5 + (EditorGUIUtility.singleLineHeight * 1), 80, 20), "Radius:");
                        stateVars.radius = EditorGUI.FloatField(new Rect(rect.x + 85, rect.y + 5 + ( EditorGUIUtility.singleLineHeight * 1), rect.width - 100, 20),  stateVars.radius);
                        EditorGUI.LabelField(new Rect(rect.x + 15 , rect.y + 5 + (EditorGUIUtility.singleLineHeight * 2), 80, 20), "Speed:");
                        stateVars.speed = EditorGUI.FloatField(new Rect(rect.x + 85, rect.y + 5 + ( EditorGUIUtility.singleLineHeight * 2), rect.width - 100, 20),  stateVars.speed);
                        EditorGUI.LabelField(new Rect(rect.x + 15 , rect.y + 5 + (EditorGUIUtility.singleLineHeight * 3), 80, 20), "Tag:");
                        stateVars.tag = EditorGUI.TextField(new Rect(rect.x + 85, rect.y + 5 + ( EditorGUIUtility.singleLineHeight * 3), rect.width - 100, 20),  stateVars.tag);
                        break;
                    case StateType.Idle:
                        title = "State: Idle";
                        break;
                    case StateType.Seek:
                        title = "State: Seek";
                        EditorGUI.LabelField(new Rect(rect.x + 15 , rect.y + 5 + (EditorGUIUtility.singleLineHeight * 1), 80, 20), "Radius:");
                        stateVars.radius = EditorGUI.FloatField(new Rect(rect.x + 85, rect.y + 5 + ( EditorGUIUtility.singleLineHeight * 1), rect.width - 100, 20),  stateVars.radius);
                        EditorGUI.LabelField(new Rect(rect.x + 15 , rect.y + 5 + (EditorGUIUtility.singleLineHeight * 2), 80, 20), "Speed:");
                        stateVars.speed = EditorGUI.FloatField(new Rect(rect.x + 85, rect.y + 5 + ( EditorGUIUtility.singleLineHeight * 2), rect.width - 100, 20),  stateVars.speed);
                        EditorGUI.LabelField(new Rect(rect.x + 15 , rect.y + 5 + (EditorGUIUtility.singleLineHeight * 3), 80, 20), "Tag:");
                        stateVars.tag = EditorGUI.TextField(new Rect(rect.x + 85, rect.y + 5 + ( EditorGUIUtility.singleLineHeight * 3), rect.width - 100, 20),  stateVars.tag);
                        break;
                    case StateType.Custom:
                        title = "State: Custom";
                        EditorGUI.LabelField(new Rect(rect.x + 15 , rect.y + 5 + (EditorGUIUtility.singleLineHeight * 1), 80, 20), "Name:");
                        stateVars.name = EditorGUI.TextField(new Rect(rect.x + 85, rect.y + 5 + ( EditorGUIUtility.singleLineHeight * 1), rect.width - 100, 20),  stateVars.name);
                        break;
                }
                break;
            case NodeType.Parameter:
                
                EditorGUI.LabelField(new Rect(rect.x + 15 , rect.y + 5 + EditorGUIUtility.singleLineHeight, 80, 20), "Type:");
                parameter.type = (AIController.Parameter.ParameterType)EditorGUI.EnumPopup(new Rect(rect.x + 85, rect.y + 5 + EditorGUIUtility.singleLineHeight, rect.width - 100, 20), parameter.type);
                miscOutput.dataType = (LinkDataType)parameter.type;
                EditorGUI.LabelField(new Rect(rect.x + 15, rect.y + 5 + ( EditorGUIUtility.singleLineHeight * 2), 80, 20), "Name:");
                parameter.name = EditorGUI.TextField(new Rect(rect.x + 85, rect.y + 5 + ( EditorGUIUtility.singleLineHeight * 2), rect.width - 100, 20), parameter.name);
                EditorGUI.LabelField(new Rect(rect.x + 15, rect.y + 5 + ( EditorGUIUtility.singleLineHeight * 3), 80, 20), "Value:");
                switch(parameter.type)
                {
                    case AIController.Parameter.ParameterType.Float:
                        parameter.fvalue = EditorGUI.FloatField(new Rect(rect.x + 85, rect.y + 5 + ( EditorGUIUtility.singleLineHeight * 3), rect.width - 100, 20), parameter.fvalue);
                        break;
                    case AIController.Parameter.ParameterType.Int:
                        parameter.ivalue = EditorGUI.IntField(new Rect(rect.x + 85, rect.y + 5 + ( EditorGUIUtility.singleLineHeight * 3), rect.width - 100, 20), parameter.ivalue);
                        break;
                    case AIController.Parameter.ParameterType.Bool:
                        parameter.bvalue = EditorGUI.Toggle(new Rect(rect.x + 85, rect.y + 5 + ( EditorGUIUtility.singleLineHeight * 3), rect.width - 100, 20), parameter.bvalue);
                        break;
                }
                miscOutput.Draw(2, rect);
                break;
            case NodeType.Logic:
                miscOutput.Draw(fields.Count, rect);
                switch(logicType)
                {
                    case LogicType.AND:
                    title = "AND | Logic";
                    break;
                    case LogicType.OR:
                    title = "OR | Logic";
                    break;
                    case LogicType.NOR:
                    title = "NOR | Logic";
                    break;
                    case LogicType.XOR:
                    title = "XOR | Logic";
                    break;
                }
                break;
            case NodeType.Comparison:
                EditorGUI.LabelField(new Rect(rect.x + 15 , rect.y + 10 + (EditorGUIUtility.singleLineHeight * 3), 80, 20), "Type:");
                comparisonType = (ComparisonType)EditorGUI.EnumPopup(new Rect(rect.x + 85, rect.y + 10 + (EditorGUIUtility.singleLineHeight * 3), rect.width - 100, 20), comparisonType);
                miscOutput.Draw(fields.Count, rect);
                break;
            case NodeType.Condition:
                title = "IF | Condition";
                conditionTrueOutput.Draw(2, rect);
                EditorGUI.LabelField(new Rect(rect.x + (rect.width - 45), (rect.y - 5) + ( EditorGUIUtility.singleLineHeight * 3), 80, 20), "True:");
                conditionFalseOutput.Draw(3, rect);
                EditorGUI.LabelField(new Rect(rect.x + (rect.width - 45), (rect.y - 5) + ( EditorGUIUtility.singleLineHeight * 4), 80, 20), "False:");
                break;
        }
        if(fields != null)
        {
            for(int i = 0; i < fields.Count; i++)
            {
                fields[i].Draw(i, rect);
            }
        }
        
    }

    //ProcessEvents
    //Parameters:
    //  Event e: The event to process
    //Returns:
    //  bool
    //Description:
    //  Processes the event passed in
    public bool ProcessEvents(Event e)
    {
        if(e.type == EventType.MouseDown)
        {
            if(e.button == 0)
            {
                if(rect.Contains(e.mousePosition))
                {
                    isDragging = true;
                    GUI.changed = true;
                    selected = true;
                    style = selectedStyle;
                }
                else
                {
                    GUI.changed = true;
                    selected = false;
                    style = defaultStyle;
                }
            }
            if(e.button == 1 && rect.Contains(e.mousePosition))
            {
                ProcessContextMenu();
                e.Use();
            }
        }
        else if(e.type == EventType.MouseUp && e.button == 0)
        {
            isDragging = false;
        }
        else if(e.type == EventType.MouseDrag && e.button == 0 && isDragging)
        {
            Move(e.delta);
            e.Use();
            return true;
        }
            
        
        return false;
    }

    //ProcessContextMenu
    //Parameters:
    //  None
    //Returns:
    //  None
    //Description:
    //  Processes the context menu for the node
    private void ProcessContextMenu()
    {
        GenericMenu genericMenu = new GenericMenu();
        genericMenu.AddItem(new GUIContent("Remove node"), false, OnRemoveNode);
        genericMenu.ShowAsContext();
    }

    //OnRemoveNode
    //Parameters:
    //  None
    //Returns:
    //  None
    //Description:
    //  Removes the node from the graph
    private void OnRemoveNode()
    {
        if(OnRemove != null)
        {
            OnRemove.Invoke(this);
        }
    }

    #endif

    
}
}
