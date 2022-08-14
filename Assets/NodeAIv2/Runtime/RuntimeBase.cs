/*
 * Bachelor of Software Engineering
 * Media Design School
 * Auckland
 * New Zealand
 * 
 * (c) 2022 Media Design School
 * 
 * File Name: RuntimeBase.cs
 * Description: 
 * Author: Nerys Thamm
 * Mail: nerysthamm@gmail.com
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NodeAI
{
    /// <summary>
    /// Base class for all runtime nodes.
    /// </summary>
    [System.Serializable]
    public class RuntimeBase : ScriptableObject
    {
        public void OnEnable() 
        {
            hideFlags = HideFlags.HideAndDontSave;
        }
        [SerializeField]
        public NodeData.State state; ///< The state of the node.

        public string tooltip = ""; ///< The tooltip of the node.
        [SerializeField]
        List<NodeData.SerializableProperty> properties = new List<NodeData.SerializableProperty>(); ///< The exposed properties of the node.

        /// <summary>
        /// Repopulates properties that may have gotted destroyed in serialisation.
        /// </summary>
        /// <param name="properties">The properties to use to repopulate the node</param>
        public void RepopulateProperties(List<NodeData.SerializableProperty> properties)
        {
            this.properties = properties;
        }

        
        /// <summary>
        ///  Adds a property field to the node.
        /// </summary>
        /// <param name="name">The name of the property field.</param>
        /// <param name="initialValue">The value the property field will have in a newly created node.</param>
        /// <typeparam name="T">The datatype the property will store.</typeparam>
        /// <example>
        /// <code>
        /// AddProperty<GameObject>("My GameObject", null);
        /// </code>
        /// </example>
        public void AddProperty<T>(string name, T initialValue)
        {
            if(properties == null)
            {
                properties = new List<NodeData.SerializableProperty>();
            }
            foreach (NodeData.SerializableProperty property in properties)
            {
                if (property.name == name.ToUpper())
                {
                    Debug.LogError("Property with name " + name + " already exists");
                    return;
                }
            }
            NodeData.SerializableProperty newProp = new NodeData.SerializableProperty()
            {
                name = name.ToUpper(),
                serializedTypename = typeof(T).AssemblyQualifiedName,
            };
            switch(newProp.type.Name)
                    {
                        case "Int32":
                            newProp.ivalue = (int)(object)initialValue;
                            break;
                        case "Single":
                            newProp.fvalue = (float)(object)initialValue;
                            break;
                        case "Boolean":
                            newProp.bvalue = (bool)(object)initialValue;
                            break;
                        case "String":
                            newProp.svalue = (string)(object)initialValue;
                            break;
                        case "Vector2":
                            newProp.v2value = (Vector2)(object)initialValue;
                            break;
                        case "Vector3":
                            newProp.v3value = (Vector3)(object)initialValue;
                            break;
                        case "Vector4":
                            newProp.v4value = (Vector4)(object)initialValue;
                            break;
                        case "Color":
                            newProp.cvalue = (Color)(object)initialValue;
                            break;
                        default:
                            newProp.ovalue = (Object)(object)initialValue;
                            break;
                    }
                    
            properties.Add(newProp);
        }

        /// <summary>
        ///  Sets the value of a property field.
        /// </summary>
        /// <param name="name">The name of the property to set.</param>
        /// <param name="value">The new value the property should be set to.</param>
        /// <typeparam name="T">The datatype of the property</typeparam>
        /// <example>
        /// <code>
        /// SetProperty<bool>("MyBool", true);
        /// </code>
        /// </example>
        public void SetProperty<T>(string name, T value)
        {
            foreach (NodeData.SerializableProperty property in properties)
            {
                if (property.name == name.ToUpper() && (property).type == typeof(T))
                {
                    switch(property.type.Name)
                    {
                        case "Int32":
                            property.ivalue = (int)(object)value;
                            break;
                        case "Single":
                            property.fvalue = (float)(object)value;
                            break;
                        case "Boolean":
                            property.bvalue = (bool)(object)value;
                            break;
                        case "String":
                            property.svalue = (string)(object)value;
                            break;
                        case "Vector2":
                            property.v2value = (Vector2)(object)value;
                            break;
                        case "Vector3":
                            property.v3value = (Vector3)(object)value;
                            break;
                        case "Vector4":
                            property.v4value = (Vector4)(object)value;
                            break;
                        case "Color":
                            property.cvalue = (Color)(object)value;
                            break;
                        default:
                            property.ovalue = (Object)(object)value;
                            break;
                    }
                    return;
                }
            }
            Debug.LogError("Property with name " + name + " does not exist");
        }

        /// <summary>
        /// Sets the property of a value field as an Object type.
        /// </summary>
        /// <param name="name">The name of the property to set.</param>
        /// <param name="value">The value the property should be set to, cast as an Object type.</param>
        /// <param name="type">The actual datatype of the property.</param>
        /// <remarks>
        /// This is used to set the value of a property field as an Object type, it exists because Unity does not support serializing reference types, and thus a workaround must be used.
        /// </remarks>
        /// <example>
        /// <code>
        /// SetProperty("My Gameobject", (object)myGameObject, typeof(GameObject));
        /// </code>
        /// </example>
        public void SetProperty(string name, Object value, System.Type type)
        {
            foreach (NodeData.SerializableProperty property in properties)
            {
                if (property.name == name.ToUpper() && (property).type == type)
                {
                    switch(property.type.Name)
                    {
                        case "Int32":
                            property.ivalue = (int)(object)value;
                            break;
                        case "Single":
                            property.fvalue = (float)(object)value;
                            break;
                        case "Boolean":
                            property.bvalue = (bool)(object)value;
                            break;
                        case "String":
                            property.svalue = (string)(object)value;
                            break;
                        case "Vector2":
                            property.v2value = (Vector2)(object)value;
                            break;
                        case "Vector3":
                            property.v3value = (Vector3)(object)value;
                            break;
                        case "Vector4":
                            property.v4value = (Vector4)(object)value;
                            break;
                        case "Color":
                            property.cvalue = (Color)(object)value;
                            break;
                        default:
                            property.ovalue = (Object)(object)value;
                            break;
                    }
                    return;
                }
            }
            Debug.LogError("Property with name " + name + " does not exist");
        }

        /// <summary>
        /// Sets the GUID of a property field.
        /// </summary>
        /// <param name="name">The name of the property to set.</param>
        /// <param name="guid">The new GUID the property should be set to.</param>
        /// <remarks>
        /// This is a method used internally for serialisation and deserialisation, and should not be used by end users.
        /// </remarks>
        public void SetPropertyGUID(string name, string GUID)
        {
            foreach (NodeData.SerializableProperty property in properties)
            {
                if (property.name == name.ToUpper())
                {
                    (property).GUID = GUID;
                    return;
                }
            }
            Debug.LogError("Property with name " + name + " does not exist");
        }

        /// <summary>
        /// Sets the Parameter reference of a Property
        /// </summary>
        /// <param name="name">The name of the property to set.</param>
        /// <param name="parameter">The new Parameter reference the property should be set to.</param>
        /// <remarks>
        /// This is a method used internally for serialisation and deserialisation, and should not be used by end users.
        /// </remarks>
        public void SetPropertyParamReference(string name, string paramReference)
        {
            foreach (NodeData.SerializableProperty property in properties)
            {
                if (property.name == name.ToUpper())
                {
                    (property).paramReference = paramReference;
                    return;
                }
            }
            Debug.LogError("Property with name " + name + " does not exist");
        }

        /// <summary>
        /// Gets the value of a property field.
        /// </summary>
        /// <param name="name">The name of the property to get.</param>
        /// <typeparam name="T">The datatype of the property</typeparam>
        /// <returns>The value of the property field.</returns>
        /// <example>
        /// <code>
        /// bool myBool = GetProperty<bool>("MyBool");
        /// </code>
        /// </example>
        public T GetProperty<T>(string name)
        {
            if(properties == null)
            {
                return default(T);
            }
            foreach (NodeData.SerializableProperty property in properties)
            {
                if (property.name == name.ToUpper() && property.serializedTypename == typeof(T).AssemblyQualifiedName)
                {
                    
                    switch(property.type.Name)
                    {
                        case "Int32":
                            return (T)(object)property.ivalue;
                            
                        case "Single":
                            return (T)(object)property.fvalue;
                            
                        case "Boolean":
                            return (T)(object)property.bvalue;
                            
                        case "String":
                            return (T)(object)property.svalue;
                            
                        case "Vector2":
                            return (T)(object)property.v2value;
                            
                        case "Vector3":
                            return (T)(object)property.v3value;
                            
                        case "Vector4":
                            return (T)(object)property.v4value;
                            
                        case "Color":
                            return (T)(object)property.cvalue;
                        default:
                            if(property.ovalue != null)
                            {
                                return (T)(object)property.ovalue;
                            }
                            else
                            {
                                return default(T);
                            }
                            
                            
                    }
                }
            }
            return default(T);
        }

        /// <summary>
        /// Gets all the properties of a node.
        /// </summary>
        /// <returns>A list of all the properties of a node.</returns>
        /// <remarks>
        /// This is a method used internally for serialisation and deserialisation, and should not be used by end users.
        /// </remarks>
        /// <example>
        /// <code>
        /// List<NodeData.SerializableProperty> properties = GetProperties();
        /// </code>
        /// </example>
        public List<NodeData.Property> GetProperties()
        {
            if(properties == null) properties = new List<NodeData.SerializableProperty>();
            return properties.ConvertAll(x => (NodeData.Property)x);
        }

        /// <summary>
        /// Gets all properties of a node as SerializableProperties.
        /// </summary>
        /// <returns>A list of all the properties of a node.</returns>
        /// <remarks>
        /// This is a method used internally for serialisation and deserialisation, and should not be used by end users.
        /// </remarks>
        /// <example>
        /// <code>
        /// List<NodeData.SerializableProperty> properties = GetProperties();
        /// </code>
        /// </example>
        public List<NodeData.SerializableProperty> GetSerializableProperties()
        {
            if (properties == null) properties = new List<NodeData.SerializableProperty>();
            return properties;
        }

        /// <summary>
        /// Gets all properties with a specific parameter reference.
        /// </summary>
        /// <param name="paramReference">The parameter reference to get properties for.</param>
        /// <returns>A list of all the properties with the specified parameter reference.</returns>
        /// <remarks>
        /// This is a method used internally for serialisation and deserialisation, and should not be used by end users.
        /// </remarks>
        public List<NodeData.SerializableProperty> GetPropertiesWhereParamReference(string paramReference)
        {
            if (properties == null) properties = new List<NodeData.SerializableProperty>();
            return properties.FindAll(x => (x).paramReference == paramReference);
        }

        /// <summary>
        ///  Eval is called when the node is evaluated, and contains all of the logic for the node.
        /// </summary>
        /// <param name="agent">The NodeAI_Agent that is currently running the node.</param>
        /// <param name="current">The current node which is being run.</param>
        /// <returns>The current or updated state of the node.</returns>
        /// <list type="table">
            /// <listheader>
                /// <term>Possible return states</term>
                /// <description>When a node is evaluated it may return one of four states based on whether it is currently running, or if it has succeeded.</description>
            /// </listheader>
            /// <item>
            /// <term>Running</term>
            /// <description>The node is currently running and is still evaluating.</description>
            /// </item>
            /// <item>
            /// <term>Success</term>
            /// <description>The node has succeeded and has finished evaluating.</description>
            /// </item>
            /// <item>
            /// <term>Failure</term>
            /// <description>The node has failed and has finished evaluating.</description>
            /// </item>
            /// <item>
            /// <term>Idle</term>
            /// <description>The node is not currently running and is not evaluating.</description>
            /// </item>
        /// </list>
        public virtual NodeData.State Eval(NodeAI_Agent agent, NodeTree.Leaf current) => NodeData.State.Failure;

        /// <summary>
        ///  Draw Gizmos is called when the Agent draws gizmos.
        /// </summary>
        /// <param name="agent">The Agent currently processing the node.</param>
        /// <remarks>
        /// This method is only called when the node is currently running.
        /// </remarks>
        /// <example>
        /// <code>
        /// override void DrawGizmos(NodeAI_Agent agent)
        /// {
        ///    Gizmos.color = Color.red;
        ///    Gizmos.DrawSphere(agent.transform.position, 0.5f);
        /// }
        /// </code>
        /// </example>
        public virtual void DrawGizmos(NodeAI_Agent agent){}  // Draw Gizmos for this node;

        /// <summary>
        /// OnInit is called whenever a node begins running.
        /// </summary>
        /// <remarks>
        /// This method is called both when the node is first started, and when the node is restarted after succeeding or failing.
        /// </remarks>
        public virtual void OnInit(){}
        
        /// <summary>
        /// Initialises the node and its children.
        /// </summary>
        public void Init(NodeTree.Leaf current) 
        {
            state = NodeData.State.Running;
            OnInit();
            
            foreach (NodeTree.Leaf child in current.children)
            {
                if(child.nodeData == null)
                {
                    Debug.LogError("nodedata is null for node");
                    continue;
                }
                child.nodeData.runtimeLogic.state = NodeData.State.Idle;
            }
        }
    }

    /// <summary>
    ///  The base class for Action Nodes.
    /// </summary>
    /// <para>
    /// Action Nodes are the base class for all nodes that perform actions.
    /// Actions are performed by the agent and involve some sort of logic that determines the behaviour of the agent or interacts with the environment.
    /// </para>
    /// <example>
    /// <code>
    /// public class MyActionNode : ActionNode
    /// {
    ///     public MyActionNode()
    ///     {
    ///        tooltip = "An Example Action Node";
    ///        AddProperty<float>("MyProperty", "My Property", 0.0f);
    ///    }
    /// 
    ///   public override NodeData.State Eval(NodeAI_Agent agent, NodeTree.Leaf current)
    ///  {
    ///     
    ///    if(...) // Check if the node has failed
    ///    {
    ///       state = NodeData.State.Failure;  
    ///       return NodeData.State.Failure;    
    ///    }
    ///    else if(...) // Check if the node has succeeded
    ///    {
    ///      state = NodeData.State.Success;
    ///      return NodeData.State.Success;
    ///    }
    ///    else // The node is still running
    ///    {
    ///      state = NodeData.State.Running;
    ///      return NodeData.State.Running;
    ///    }  
    ///  }
    /// }
    /// </code>
    /// </example>
    [System.Serializable]
    public class ActionBase : RuntimeBase
    {
        public override NodeData.State Eval(NodeAI_Agent agent, NodeTree.Leaf current)
        {
            return NodeData.State.Success;
        }
    }
    /// <summary>
    ///  The base class for Condition Nodes.
    /// </summary>
    /// <para>
    /// Condition Nodes are the base class for all nodes that check conditions.
    /// Nodes of this type determine if their condition is true or false.
    /// If the condition is true, the node will succeed, otherwise it will fail.
    /// Condition nodes are used as part of the control flow of the node tree.
    /// </para>
    /// <example>
    /// <code>
    /// public class MyConditionNode : ConditionBase
    /// {
    ///    public MyConditionNode()
    ///   {
    ///      tooltip = "This is an example condition node.";
    ///      AddProperty<Vector3>("Position", Vector3.zero);
    ///      AddProperty<float>("Distance", 0.0f);
    ///  }
    /// 
    ///   public override NodeData.State Eval(NodeAI_Agent agent, NodeTree.Leaf current)
    ///  {
    ///     if (Vector3.Distanct(agent.transform.position, Position1) <= GetProperty<float>("Distance"))
    ///     {
    ///       state = NodeData.State.Success; 
    ///       return NodeData.State.Success;
    ///     }
    ///     else
    ///     {
    ///      state = NodeData.State.Failure;
    ///      return NodeData.State.Failure;
    ///    }
    /// }
    /// }
    /// </code>
    /// </example>
    [System.Serializable]
    public class ConditionBase : RuntimeBase
    {
        public override NodeData.State Eval(NodeAI_Agent agent, NodeTree.Leaf current)
        {
            return NodeData.State.Success;
        }
    }

    /// <summary>
    /// Checks if an inputted boolean parameter is true.
    /// </summary>
    /// <remarks>
    /// This node checks if the boolean parameter is true, and succeeds if it is.
    /// Otherwise it fails.
    /// </remarks>
    [System.Serializable]
    public class IfTrue : ConditionBase
    {
        public IfTrue()
        {
            AddProperty<bool>("Condition", false);
            tooltip = "Succeeds if the condition is true, fails otherwise";
        }

        public override NodeData.State Eval(NodeAI_Agent agent, NodeTree.Leaf current)
        {
            if (GetProperty<bool>("Condition"))
            {
                state = NodeData.State.Success;
                return NodeData.State.Success;
            }
            state = NodeData.State.Failure;
            return NodeData.State.Failure;
        }
    }
    
    /// <summary>
    ///  The base class for Decorator Nodes.
    /// </summary>
    /// <para>
    /// Decorator Nodes may have one single child node, and they work by modifying the return of the child node, or otherwise changing how it runs.
    /// An example of a decorator node is the Inverter node, which inverts the return of the child node. If the child of the inverter node returns success, the inverter node will fail, and vice versa.
    /// Another example of a decorator node is the Repeater node, which repeats the child node multiple times.
    /// </para>
    /// <example>
    /// <code>
    /// public class MyDecoratorNode : DecoratorNode
    /// {
    ///    public MyDecoratorNode() // Constructor
    ///    {   
    ///      tooltip = "An Example Decorator Node";
    ///      AddProperty<float>("MyProperty", "My Property", 0.0f);
    ///    }
    ///     
    ///     public override NodeData.State ApplyDecorator(NodeAI_Agent agent, NodeTree.Leaf child) // Override the ApplyDecorator method
    ///    {
    ///       switch(child.nodeData.runtimeLogic.state) // Check the state of the child node
    ///       {
    ///         case NodeData.State.Success: // If the child node succeeded
    ///             state = NodeData.State.Failure;
    ///             return NodeData.State.Failure;
    ///         case NodeData.State.Failure: // If the child node failed
    ///             state = NodeData.State.Success;
    ///             return NodeData.State.Success;
    ///         default: // The child node is still running
    ///             state = NodeData.State.Running;
    ///             return NodeData.State.Running;
    ///      }
    ///   }
    /// }
    /// </code>
    /// </example>    
    [System.Serializable]
    public class DecoratorBase : RuntimeBase
    {
        public virtual NodeData.State ApplyDecorator(NodeAI_Agent agent, NodeTree.Leaf child) => child.nodeData.Eval(agent, child);
        public override NodeData.State Eval(NodeAI_Agent agent, NodeTree.Leaf current)
        {
            if(current.children.Count == 0)
            {
                return NodeData.State.Failure;
            }
            if(current.children[0].nodeData.runtimeLogic.state != NodeData.State.Running)
            {
                current.children[0].nodeData.Init(current.children[0]);
            }
            state = ApplyDecorator(agent, current.children[0]);
            return state;
        }

        
    }
    /// <summary>
    /// Inverts the return of the child node.
    /// </summary>
    [System.Serializable]
    public class Inverter : DecoratorBase
    {
        public override NodeData.State ApplyDecorator(NodeAI_Agent agent, NodeTree.Leaf child)
        {
            NodeData.State childState = child.nodeData.Eval(agent, child);
            if (childState == NodeData.State.Success)
            {
                return NodeData.State.Failure;
            }
            else if (childState == NodeData.State.Failure)
            {
                return NodeData.State.Success;
            }
            else
            {
                return childState;
            }
        }

        
    }
    /// <summary>
    /// If the child node completes, then the Decorator will return success regardless of what the child node returns.
    /// </summary>
    [System.Serializable]
    public class Succeeder : DecoratorBase
    {
        public Succeeder()
        {
            tooltip = "Always returns success regardless of the child's state.";
        }
        public override NodeData.State ApplyDecorator(NodeAI_Agent agent, NodeTree.Leaf child)
        {
            if (child.nodeData.runtimeLogic.Eval(agent, child) == NodeData.State.Running)
            {
                return NodeData.State.Running;
            }
            else
            {
                return NodeData.State.Success;
            }
            
        }

        
    }
    /// <summary>
    /// Runs the child node multiple times.
    /// </summary>
    [System.Serializable]
    public class Repeater : DecoratorBase
    {
        int repeatCount = 0;

        public Repeater()
        {
            AddProperty<int>("RepeatCount", 1);
            AddProperty<bool>("RepeatForever", false);
            tooltip = "Repeats the child node a set number of times, or forever if RepeatForever is true";
        }

        public override void OnInit()
        {
            repeatCount = 0;
        }
        public override NodeData.State ApplyDecorator(NodeAI_Agent agent, NodeTree.Leaf child)
        {
            NodeData.State childState = child.nodeData.Eval(agent, child);
            if (childState != NodeData.State.Running && repeatCount < GetProperty<int>("RepeatCount"))
            {
                repeatCount++;
                child.nodeData.Init(child);
                return NodeData.State.Running;
            }
            else if (childState != NodeData.State.Running && GetProperty<bool>("RepeatForever"))
            {
                child.nodeData.Init(child);
                return NodeData.State.Running;
            }
            else
            {
                return childState;
            }
            

        }

        
    }
    /// <summary>
    /// Runs the child node multiple times until it fails.
    /// </summary>
    public class RepeatUntilFail : DecoratorBase
    {
        public override NodeData.State ApplyDecorator(NodeAI_Agent agent, NodeTree.Leaf child)
        {
            NodeData.State childState = child.nodeData.Eval(agent, child);
            if (childState == NodeData.State.Running)
            {
                return NodeData.State.Running;
            }
            else if (childState == NodeData.State.Failure)
            {
                return NodeData.State.Failure;
            }
            else
            {
                child.nodeData.Init(child);
                return NodeData.State.Running;
            }
        }

        
    }
    /// <summary>
    /// Runs the child node multiple times until it succeeds.
    /// </summary>
    public class RepeatUntilSuccess : DecoratorBase
    {
        public override NodeData.State ApplyDecorator(NodeAI_Agent agent, NodeTree.Leaf child)
        {
            NodeData.State childState = child.nodeData.Eval(agent, child);
            if (childState == NodeData.State.Running)
            {
                return NodeData.State.Running;
            }
            else if (childState == NodeData.State.Success)
            {
                return NodeData.State.Success;
            }
            else
            {
                child.nodeData.Init(child);
                return NodeData.State.Running;
            }
        }

        
    }
    /// <summary>
    /// Has a chance to run the child node.
    /// </summary>
    /// <remarks>
    /// The chance is determined by the chance property.
    /// When the node is initialised it will use RNG to determine whether or not it will run the child node.
    /// If it runs the child node, it will return whatever the child node returns.
    /// Otherwise, if it fails the chance test, it will return failure.
    /// </remarks>
    public class Chance : DecoratorBase
    {
        float randValue = 0;
        public Chance()
        {
            AddProperty<float>("Chance", 0.5f);
            tooltip = "Has a chance of running its child node (0.0 - 1.0), or immediately failing.";
        }

        public override NodeData.State ApplyDecorator(NodeAI_Agent agent, NodeTree.Leaf child)
        {
            if (randValue < GetProperty<float>("Chance"))
            {
                return child.nodeData.Eval(agent, child);
            }
            else
            {
                return NodeData.State.Failure;
            }
        }

        public override NodeData.State Eval(NodeAI_Agent agent, NodeTree.Leaf current)
        {
            if(current.children[0].nodeData.runtimeLogic.state != NodeData.State.Running && randValue < GetProperty<float>("Chance"))
            {
                current.children[0].nodeData.Init(current.children[0]);
            }
            state = ApplyDecorator(agent, current.children[0]);
            return state;
        }

        

        public override void OnInit()
        {
            randValue = UnityEngine.Random.value;
        }
    }

    /// <summary>
    /// Runs all of the child nodes in sequence.
    /// </summary>
    /// <remarks>
    /// If the child node fails, the sequence will stop and return failure.
    /// If the child node succeeds, the sequence will continue.
    /// </remarks>
    public class Sequence : RuntimeBase
    {
        public Sequence()
        {
            tooltip = "Runs all children in order (top to bottom), until one fails, or all have succeeded.";
        }
        public override NodeData.State Eval(NodeAI_Agent agent, NodeTree.Leaf current)
        {
            
            int successCount = 0;
            foreach (NodeTree.Leaf child in current.children)
            {
                if(child.nodeData.runtimeLogic.state != NodeData.State.Running)
                {
                    if(child.nodeData.runtimeLogic.state != NodeData.State.Idle)
                    {
                        if(child.nodeData.runtimeLogic.state == NodeData.State.Success)
                        {
                            successCount++;
                        }
                        else
                        {
                            state = NodeData.State.Failure;
                            return NodeData.State.Failure;
                        }
                        continue;
                    }
                    else
                    {
                        child.nodeData.Init(child);
                    }
                }
                
                switch (child.nodeData.Eval(agent, child))
                {
                    case NodeData.State.Failure:
                        state = NodeData.State.Failure;
                        return state;
                    case NodeData.State.Success:
                        continue;
                    case NodeData.State.Running:
                        state = NodeData.State.Running;
                        return state;
                    default:
                        state = NodeData.State.Success;
                        return state;
                }
            }

            if (successCount == current.children.Count)
            {
                state = NodeData.State.Success;
            }
            else
            {
                state = NodeData.State.Running;
            }
            return state;
        }
    }

    /// <summary>
    /// Runs all of the child nodes in parallel.
    /// </summary>
    /// <remarks>
    /// If any child node succeeds, the parallel will return success.
    /// Otherwise, if all child nodes fail, the parallel will return failure.
    /// </remarks>
    public class Parallel : RuntimeBase
    {
        public Parallel()
        {
            tooltip = "Runs all children simultaneously, until all have failed or one has succeeded.";
        }
        public override NodeData.State Eval(NodeAI_Agent agent, NodeTree.Leaf current)
        {
            int failCount = 0;
            bool success = false;
            foreach (NodeTree.Leaf child in current.children)
            {
                if(child.nodeData.runtimeLogic.state == NodeData.State.Idle) {
                    child.nodeData.Init(child);
                }
                switch (child.nodeData.Eval(agent, child))
                {
                    case NodeData.State.Failure:
                        failCount++;
                        continue;
                    case NodeData.State.Success:
                        success = true;
                        continue;
                    case NodeData.State.Running:
                        state = NodeData.State.Running;
                        continue;
                    default:
                        state = NodeData.State.Success;
                        return state;
                }
            }
            if (failCount == current.children.Count)
            {
                state = NodeData.State.Failure;
            }
            else if(success)
            {
                state = NodeData.State.Success;
            }
            else
            {
                state = NodeData.State.Running;
            }
            return state;
        }
    }

    /// <summary>
    /// Runs child nodes in sequence until one succeeds.
    /// </summary>
    /// <remarks>
    /// If a child node succeeds, the sequence will stop and return success.
    /// If a child node fails, the sequence will continue.
    /// If all child nodes fail, the sequence will return failure.
    /// </remarks>
    public class Selector : RuntimeBase
    {
        public Selector()
        {
            tooltip = "Runs all children in order (top to bottom), until one succeeds, or all have failed.";
        }
        public override NodeData.State Eval(NodeAI_Agent agent, NodeTree.Leaf current)
        {
            foreach (NodeTree.Leaf child in current.children)
            {
                if(child.nodeData.runtimeLogic.state == NodeData.State.Idle) {
                    child.nodeData.Init(child);
                }
                if(child.nodeData.runtimeLogic.state == NodeData.State.Failure)
                {
                    continue;
                }
                switch (child.nodeData.Eval(agent, child))
                {
                    case NodeData.State.Failure:
                        continue;
                    case NodeData.State.Success:
                        state = NodeData.State.Success;
                        return state;
                    case NodeData.State.Running:
                        state = NodeData.State.Running;
                        return state;
                    default:
                        state = NodeData.State.Success;
                        return state;
                }
            }
            state = NodeData.State.Failure;
            return state;
        }
    }

    



    


}

