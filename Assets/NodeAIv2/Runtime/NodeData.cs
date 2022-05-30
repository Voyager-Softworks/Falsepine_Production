using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NodeAI
{
    [System.Serializable]
    public class NodeData 
    {
        
        public string GUID;
        public string parentGUID;
        public string title;
        public List<string> childGUIDs;
        public Type nodeType;
        public Vector2 position;
        

        public enum Type
            {
                EntryPoint,
                Action,
                Condition,
                Decorator,
                Sequence,
                Selector,
                Parallel,
                Parameter,
                Query
            }

        public enum State
        {
            Running,
            Success,
            Failure,
            Idle,
        }

        public State Eval(NodeAI_Agent agent, NodeTree.Leaf current) => runtimeLogic.Eval(agent, current);

        public void Init(NodeTree.Leaf current) => runtimeLogic.Init(current);
        [SerializeField]
        RuntimeBase runtime;
        [SerializeField]
        Query runtimequery;
        
        [SerializeField]
        private List<SerializableProperty> properties;
        
        [SerializeField]
        private string runtimeLogicType;
        [SerializeField]
        private string queryType;

        [SerializeField]
        public bool noLogic = false;
        [SerializeField]
        public bool noQuery = false;

        public void Reset()
        {
            if(noLogic) return;
            runtime = (RuntimeBase)ScriptableObject.CreateInstance(System.Type.GetType(runtimeLogicType));
            runtime.RepopulateProperties(properties == null ? new List<SerializableProperty>() : properties);
        }
        public RuntimeBase runtimeLogic
        {
            get
            {
                if (runtime == null && !noLogic)
                {
                    runtime = (RuntimeBase)ScriptableObject.CreateInstance(System.Type.GetType(runtimeLogicType));
                    runtime.RepopulateProperties(properties == null ? new List<SerializableProperty>() : properties);
                }
                return runtime;
            }
            set
            {
                noLogic = (value == null);
                runtime = value;
                if(!noLogic)
                {
                    properties = runtime.GetProperties().ConvertAll(x => (SerializableProperty)x);
                    runtimeLogicType = runtime.GetType().AssemblyQualifiedName;
                }
            }
        }

        public Query query
        {
            get
            {
                if (runtimequery == null && !noQuery)
                {
                    runtimequery = (Query)ScriptableObject.CreateInstance(System.Type.GetType(queryType));
                    runtimequery.RepopulateProperties(properties == null ? new List<SerializableProperty>() : properties);
                }
                return runtimequery;
            }
            set
            {
                noQuery = (value == null);
                runtimequery = value;
                if (runtimequery != null)
                {
                    properties = runtimequery.GetProperties().ConvertAll(x => (SerializableProperty)x);
                    queryType = runtimequery.GetType().AssemblyQualifiedName;
                }
            }
        }
        

        [System.Serializable]
        public class Property
        {
            public Property()
            {
                name = "";
                value = "";
                GUID = System.Guid.NewGuid().ToString();
            }
            public Property(string name, object value, System.Type type)
            {
                this.name = name;
                this.value = value;
                this.type = type;
                GUID = System.Guid.NewGuid().ToString();
            }
            [SerializeField]
            public string name;
            [SerializeField]
            public string GUID;
            [SerializeField]
            public string paramReference;
            [SerializeField]
            public System.Type type;

            public object value;

            public bool output = false;
        }

        [System.Serializable]
        public class Property<T> : Property
        {
            public Property()
            {
                type = typeof(T);
                GUID = System.Guid.NewGuid().ToString();
                value = default(T);
            }
            public T Value => (T)this.value;
            
        }
        [System.Serializable]
        public class SerializableProperty
        {
            public SerializableProperty(){ GUID = System.Guid.NewGuid().ToString(); }
            
            public SerializableProperty(SerializableProperty copy)
            {
                name = copy.name;
                GUID = copy.GUID;
                paramReference = copy.paramReference;
                serializedTypename = copy.serializedTypename;
                ivalue = copy.ivalue;
                fvalue = copy.fvalue;
                bvalue = copy.bvalue;
                svalue = copy.svalue;
                v2value = copy.v2value;
                v3value = copy.v3value;
                v4value = copy.v4value;
                cvalue = copy.cvalue;
                ovalue = copy.ovalue;
                output = copy.output;
            }
            public static implicit operator SerializableProperty(Property property)
            {
                var serializableProperty = new SerializableProperty();
                serializableProperty.name = property.name;
                serializableProperty.GUID = property.GUID;
                serializableProperty.paramReference = property.paramReference;
                serializableProperty.serializedTypename = property.type.AssemblyQualifiedName;
                serializableProperty.output = property.output;
                switch(property.type.Name)
                    {
                        case "Int32":
                            serializableProperty.ivalue = (int)(object)property.value;
                            break;
                        case "Single":
                            serializableProperty.fvalue = (float)(object)property.value;
                            break;
                        case "Boolean":
                            serializableProperty.bvalue = (bool)(object)property.value;
                            break;
                        case "String":
                            serializableProperty.svalue = (string)(object)property.value;
                            break;
                        case "Vector2":
                            serializableProperty.v2value = (Vector2)(object)property.value;
                            break;
                        case "Vector3":
                            serializableProperty.v3value = (Vector3)(object)property.value;
                            break;
                        case "Vector4":
                            serializableProperty.v4value = (Vector4)(object)property.value;
                            break;
                        case "Color":
                            serializableProperty.cvalue = (Color)(object)property.value;
                            break;
                        default:
                            serializableProperty.ovalue = (UnityEngine.Object)property.value;
                            break;
                    }
                return serializableProperty;
            }

            public static implicit operator Property(SerializableProperty serializableProperty)
            {
                var property = new Property();
                property.name = serializableProperty.name;
                property.GUID = serializableProperty.GUID;
                property.type = System.Type.GetType(serializableProperty.serializedTypename);
                property.paramReference = serializableProperty.paramReference;
                property.output = serializableProperty.output;
                switch(property.type.Name)
                    {
                        case "Int32":
                            property.value = serializableProperty.ivalue;
                            break;
                        case "Single":
                            property.value = serializableProperty.fvalue;
                            break;
                        case "Boolean":
                            property.value = serializableProperty.bvalue;
                            break;
                        case "String":
                            property.value = serializableProperty.svalue;
                            break;
                        case "Vector2":
                            property.value = serializableProperty.v2value;
                            break;
                        case "Vector3":
                            property.value = serializableProperty.v3value;
                            break;
                        case "Vector4":
                            property.value = serializableProperty.v4value;
                            break;
                        case "Color":
                            property.value = serializableProperty.cvalue;
                            break;
                        default:
                            property.value = serializableProperty.ovalue;
                            break;
                    }
                return property;
            }
            public string name;
            public string GUID;
            [SerializeField]
            public string paramReference;
            public string serializedTypename;
            public System.Type type => System.Type.GetType(serializedTypename);
            public string svalue;
            public int ivalue;
            public float fvalue;
            public bool bvalue;
            public Color cvalue;
            public Vector2 v2value;
            public Vector3 v3value;
            public Vector4 v4value;
            [SerializeReference]public UnityEngine.Object ovalue;
            public bool output = false;
        }
        [System.Serializable]
        public struct NodeGroup
        {
            [SerializeField]
            public string title;
            [SerializeField]
            public List<string> childGUIDs;
        }
        
    }


}
