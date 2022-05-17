using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NodeAI
{
    [System.Serializable]
    public class RuntimeBase : ScriptableObject
    {

        public void OnEnable()
        {
            hideFlags = HideFlags.HideAndDontSave;
        }
        [SerializeField]
        public NodeData.State state;

        public string tooltip = "";
        [SerializeField]
        List<NodeData.SerializableProperty> properties = new List<NodeData.SerializableProperty>();

        public void RepopulateProperties(List<NodeData.SerializableProperty> properties)
        {
            this.properties = properties;
        }

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
            properties.Add(new NodeData.Property()
            {
                name = name.ToUpper(),
                type = typeof(T),
                value = initialValue
            });
        }

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
                            return (T)(object)property.ovalue;
                            
                    }
                }
            }
            return default(T);
        }


        public List<NodeData.Property> GetProperties()
        {
            if(properties == null) properties = new List<NodeData.SerializableProperty>();
            return properties.ConvertAll(x => (NodeData.Property)x);
        }

        public List<NodeData.SerializableProperty> GetPropertiesWhereParamReference(string paramReference)
        {
            if (properties == null) properties = new List<NodeData.SerializableProperty>();
            return properties.FindAll(x => (x).paramReference == paramReference);
        }


        public virtual NodeData.State Eval(NodeAI_Agent agent, NodeTree.Leaf current) => NodeData.State.Failure;

        public virtual void DrawGizmos(NodeAI_Agent agent){}  // Draw Gizmos for this node;

        public virtual void OnInit(){}
        
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

    
    [System.Serializable]
    public class ActionBase : RuntimeBase
    {
        public override NodeData.State Eval(NodeAI_Agent agent, NodeTree.Leaf current)
        {
            return NodeData.State.Success;
        }
    }
    [System.Serializable]
    public class ConditionBase : RuntimeBase
    {
        public override NodeData.State Eval(NodeAI_Agent agent, NodeTree.Leaf current)
        {
            return NodeData.State.Success;
        }
    }

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
    
    [System.Serializable]
    public class DecoratorBase : RuntimeBase
    {
        public virtual NodeData.State ApplyDecorator(NodeAI_Agent agent, NodeTree.Leaf child) => child.nodeData.Eval(agent, child);
        public override NodeData.State Eval(NodeAI_Agent agent, NodeTree.Leaf current)
        {
            if(current.children[0].nodeData.runtimeLogic.state != NodeData.State.Running)
            {
                current.children[0].nodeData.Init(current.children[0]);
            }
            state = ApplyDecorator(agent, current.children[0]);
            return state;
        }

        
    }
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
    [System.Serializable]
    public class Succeeder : DecoratorBase
    {
        public Succeeder()
        {
            tooltip = "Always returns success regardless of the child's state.";
        }
        public override NodeData.State ApplyDecorator(NodeAI_Agent agent, NodeTree.Leaf child)
        {
            return NodeData.State.Success;
        }

        
    }
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

        

        public override void OnInit()
        {
            randValue = Random.value;
        }
    }

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
                state = NodeData.State.Failure;
            }
            return state;
        }
    }

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

