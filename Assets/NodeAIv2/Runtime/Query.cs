using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NodeAI
{
    [System.Serializable]
    public class Query : ScriptableObject
    {
        public void OnEnable()
        {
            hideFlags = HideFlags.HideAndDontSave;
        }

        public string tooltip = "";
        [SerializeField]
        List<NodeData.SerializableProperty> properties = new List<NodeData.SerializableProperty>();

        public void RepopulateProperties(List<NodeData.SerializableProperty> properties)
        {
            this.properties = properties;
        }

        public void AddProperty<T>(string name, T initialValue, bool isOutput)
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

        public List<NodeData.SerializableProperty> GetPropertiesWhereParamReference(string paramReference)
        {
            if (properties == null) properties = new List<NodeData.SerializableProperty>();
            return properties.FindAll(x => (x).paramReference == paramReference);
        }

        public virtual void GetNewValues(NodeAI_Agent agent){}
    }

    public class TestQuery : Query
    {
        public TestQuery()
        {
            AddProperty<float>("TestFloat", 0.0f, true);
        }
        public override void GetNewValues(NodeAI_Agent agent)
        {
            SetProperty("TestFloat", 4f);
        }
    }
}