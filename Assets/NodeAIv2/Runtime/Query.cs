using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NodeAI
{
    /// <summary>
    /// A query is a node that can take inputs and produce outputs based on the scene.
    /// </summary>
    [System.Serializable]
    public class Query : ScriptableObject
    {
        /// <summary>
        /// Executes when object is enabled
        /// </summary>
        public void OnEnable()
        {
            hideFlags = HideFlags.HideAndDontSave; // Stop it from resetting when the inspector serialises
        }

        public string tooltip = ""; // Tooltip to display in the inspector
        
        [SerializeField]
        List<NodeData.SerializableProperty> properties = new List<NodeData.SerializableProperty>(); // Properties

        public void RepopulateProperties(List<NodeData.SerializableProperty> properties) // Repopulates properties when they are lost in serialisation
        {
            this.properties = properties;
        }

        public void AddProperty<T>(string name, T initialValue, bool isOutput) //Adds a property
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
                output = isOutput,
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

        public void SetProperty<T>(string name, T value) //Sets the value of a property
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
                return default;
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
                                return default;
                            }
                            
                            
                    }
                }
            }
            return default;
        }

        public List<NodeData.Property> GetProperties()
        {
            if(properties == null) properties = new List<NodeData.SerializableProperty>();
            return properties.ConvertAll(x => (NodeData.Property)x);
        }

        public List<NodeData.SerializableProperty> GetSerializableProperties()
        {
            if(properties == null) properties = new List<NodeData.SerializableProperty>();
            return properties;
        }

        public List<NodeData.SerializableProperty> GetPropertiesWhereParamReference(string paramReference)
        {
            if (properties == null) properties = new List<NodeData.SerializableProperty>();
            return properties.FindAll(x => (x).paramReference == paramReference);
        }

        public virtual void GetNewValues(NodeAI_Agent agent){}
    }

    
    namespace Math
    {
        public class Add : Query
        {
            public Add()
            {
                AddProperty<float>("Float1", 0.0f, false);
                AddProperty<float>("Float2", 0.0f, false);
                AddProperty<float>("Result", 0.0f, true);
            }
            public override void GetNewValues(NodeAI_Agent agent)
            {
                
                SetProperty("Result", GetProperty<float>("Float1") + GetProperty<float>("Float2"));
            }
        }

        public class Subtract : Query
        {
            public Subtract()
            {
                AddProperty<float>("Float1", 0.0f, false);
                AddProperty<float>("Float2", 0.0f, false);
                AddProperty<float>("Result", 0.0f, true);
            }
            public override void GetNewValues(NodeAI_Agent agent)
            {
                
                SetProperty("Result", GetProperty<float>("Float1") - GetProperty<float>("Float2"));
            }
        }

        public class Multiply : Query
        {
            public Multiply()
            {
                AddProperty<float>("Float1", 0.0f, false);
                AddProperty<float>("Float2", 0.0f, false);
                AddProperty<float>("Result", 0.0f, true);
            }
            public override void GetNewValues(NodeAI_Agent agent)
            {
                
                SetProperty("Result", GetProperty<float>("Float1") * GetProperty<float>("Float2"));
            }
        }

        public class Divide : Query
        {
            public Divide()
            {
                AddProperty<float>("Float1", 0.0f, false);
                AddProperty<float>("Float2", 0.0f, false);
                AddProperty<float>("Result", 0.0f, true);
            }
            public override void GetNewValues(NodeAI_Agent agent)
            {
                
                SetProperty("Result", GetProperty<float>("Float1") / GetProperty<float>("Float2"));
            }
        }

        public class GreaterThan : Query
        {
            public GreaterThan()
            {
                AddProperty<float>("Float1", 0.0f, false);
                AddProperty<float>("Float2", 0.0f, false);
                AddProperty<bool>("Result", false, true);
            }
            public override void GetNewValues(NodeAI_Agent agent)
            {
                
                SetProperty("Result", GetProperty<float>("Float1") > GetProperty<float>("Float2"));
            }
        }

        public class LessThan : Query
        {
            public LessThan()
            {
                AddProperty<float>("Float1", 0.0f, false);
                AddProperty<float>("Float2", 0.0f, false);
                AddProperty<bool>("Result", false, true);
            }
            public override void GetNewValues(NodeAI_Agent agent)
            {
                
                SetProperty("Result", GetProperty<float>("Float1") < GetProperty<float>("Float2"));
            }
        }

        public class EqualTo : Query
        {
            public EqualTo()
            {
                AddProperty<float>("Float1", 0.0f, false);
                AddProperty<float>("Float2", 0.0f, false);
                AddProperty<bool>("Result", false, true);
            }
            public override void GetNewValues(NodeAI_Agent agent)
            {
                
                SetProperty("Result", GetProperty<float>("Float1") == GetProperty<float>("Float2"));
            }
        }

        public class NotEqualTo : Query
        {
            public NotEqualTo()
            {
                AddProperty<float>("Float1", 0.0f, false);
                AddProperty<float>("Float2", 0.0f, false);
                AddProperty<bool>("Result", false, true);
            }
            public override void GetNewValues(NodeAI_Agent agent)
            {
                
                SetProperty("Result", GetProperty<float>("Float1") != GetProperty<float>("Float2"));
            }
        }

        public class GreaterThanOrEqualTo : Query
        {
            public GreaterThanOrEqualTo()
            {
                AddProperty<float>("Float1", 0.0f, false);
                AddProperty<float>("Float2", 0.0f, false);
                AddProperty<bool>("Result", false, true);
            }
            public override void GetNewValues(NodeAI_Agent agent)
            {
                
                SetProperty("Result", GetProperty<float>("Float1") >= GetProperty<float>("Float2"));
            }
        }

    }
    namespace Logic
    {
        public class And : Query
        {
            public And()
            {
                AddProperty<bool>("Bool1", false, false);
                AddProperty<bool>("Bool2", false, false);
                AddProperty<bool>("Result", false, true);
            }
            public override void GetNewValues(NodeAI_Agent agent)
            {
                SetProperty("Result", GetProperty<bool>("Bool1") && GetProperty<bool>("Bool2"));
            }
        }

        public class Or : Query
        {
            public Or()
            {
                AddProperty<bool>("Bool1", false, false);
                AddProperty<bool>("Bool2", false, false);
                AddProperty<bool>("Result", false, true);
            }
            public override void GetNewValues(NodeAI_Agent agent)
            {
                SetProperty("Result", GetProperty<bool>("Bool1") || GetProperty<bool>("Bool2"));
            }
        }

        public class Not : Query
        {
            public Not()
            {
                AddProperty<bool>("Bool", false, false);
                AddProperty<bool>("Result", false, true);
            }
            public override void GetNewValues(NodeAI_Agent agent)
            {
                SetProperty("Result", !GetProperty<bool>("Bool"));
            }
        }

        public class Xor : Query
        {
            public Xor()
            {
                AddProperty<bool>("Bool1", false, false);
                AddProperty<bool>("Bool2", false, false);
                AddProperty<bool>("Result", false, true);
            }
            public override void GetNewValues(NodeAI_Agent agent)
            {
                SetProperty("Result", GetProperty<bool>("Bool1") ^ GetProperty<bool>("Bool2"));
            }
        }

        public class Nand : Query
        {
            public Nand()
            {
                AddProperty<bool>("Bool1", false, false);
                AddProperty<bool>("Bool2", false, false);
                AddProperty<bool>("Result", false, true);
            }
            public override void GetNewValues(NodeAI_Agent agent)
            {
                SetProperty("Result", !(GetProperty<bool>("Bool1") && GetProperty<bool>("Bool2")));
            }
        }

        public class Nor : Query
        {
            public Nor()
            {
                AddProperty<bool>("Bool1", false, false);
                AddProperty<bool>("Bool2", false, false);
                AddProperty<bool>("Result", false, true);
            }
            public override void GetNewValues(NodeAI_Agent agent)
            {
                SetProperty("Result", !(GetProperty<bool>("Bool1") || GetProperty<bool>("Bool2")));
            }
        }

        public class Xnor : Query
        {
            public Xnor()
            {
                AddProperty<bool>("Bool1", false, false);
                AddProperty<bool>("Bool2", false, false);
                AddProperty<bool>("Result", false, true);
            }
            public override void GetNewValues(NodeAI_Agent agent)
            {
                SetProperty("Result", !(GetProperty<bool>("Bool1") ^ GetProperty<bool>("Bool2")));
            }
        }


    }
    
    namespace Utility
    {
        public class GetObjectTransform : Query
        {
            public GetObjectTransform()
            {
                AddProperty<GameObject>("Object", null, false);
                AddProperty<Transform>("Transform", null, true);
            }
            public override void GetNewValues(NodeAI_Agent agent)
            {
                SetProperty("Transform", GetProperty<GameObject>("Object")?.transform);
            }
        }
    }

}