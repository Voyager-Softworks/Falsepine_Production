/*
 * Bachelor of Software Engineering
 * Media Design School
 * Auckland
 * New Zealand
 * 
 * (c) 2022 Media Design School
 * 
 * File Name: NodeData.cs
 * Description: 
 * Author: Nerys Thamm
 * Mail: nerysthamm@gmail.com
 */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NodeAI
{
    /// <summary>
    ///  This class is used to store the data of a node.
    /// </summary>
    /// <remarks>
    /// It is used both as part of serialisation and at runtime in order to determine AI Agent behaviour.
    /// </remarks>
    [System.Serializable]
    public class NodeData 
    {
        /// <summary>
        ///  The Unique ID of the node.
        /// </summary>
        public string GUID;
        /// <summary>
        ///  The GUID of the node's parent.
        /// </summary>
        public string parentGUID;
        /// <summary>
        ///  The name of the node.
        /// </summary>
        public string title;
        /// <summary>
        ///  The GUIDs of the node's children.
        /// </summary>
        public List<string> childGUIDs;
        /// <summary>
        ///  The type of the node.
        /// </summary>
        public Type nodeType;
        /// <summary>
        /// The position of the node in the node graph editor window.
        /// </summary>
        public Vector2 position;
        
        /// <summary>
        ///  Contains all possible types of node.
        /// </summary>
        public enum Type
            {
                EntryPoint, ///< The entry point of the node graph.
                Action, ///< An action node.
                Condition, ///< A condition node.
                Decorator, ///< A decorator node.
                Sequence, ///< A sequence node.
                Selector, ///< A selector node.
                Parallel, ///< A parallel node.
                Parameter, ///< A parameter node.
                Query ///< A query node.
            }

        /// <summary>
        /// Possible states of the node.
        /// </summary>
        public enum State
        {
            Running, ///< The node is running.
            Success, ///< The node has succeeded.
            Failure, ///< The node has failed.
            Idle, ///< The node is not running.
        }

        /// <summary>
        /// Evaluates the node.
        /// </summary>
        public State Eval(NodeAI_Agent agent, NodeTree.Leaf current) => runtimeLogic.Eval(agent, current);

        /// <summary>
        ///  Initialises the node.
        /// </summary>
        /// <param name="current"></param>
        public void Init(NodeTree.Leaf current) => runtimeLogic.Init(current);

        /// <summary>
        /// Contains the runtime logic for the node.
        /// </summary>
        [SerializeField]
        RuntimeBase runtime;
        /// <summary>
        /// Contains the query logic for the node.
        /// </summary>
        [SerializeField]
        Query runtimequery;
        /// <summary>
        /// Contains all properties of the node.
        /// </summary>
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
        /// <summary>
        /// Resets the node.
        /// </summary>
        public void Reset()
        {
            if(!noLogic)
            {
                runtime = (RuntimeBase)ScriptableObject.CreateInstance(System.Type.GetType(runtimeLogicType));
                runtime.RepopulateProperties(properties == null ? new List<SerializableProperty>() : properties);
            }
            else if(!noQuery)
            {
                runtimequery = (Query)ScriptableObject.CreateInstance(System.Type.GetType(queryType));
                runtimequery.RepopulateProperties(properties == null ? new List<SerializableProperty>() : properties);
            }

        }
        /// <summary>
        /// Getter and setter for the runtime logic of the node.
        /// </summary>
        /// <value></value>
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
        /// <summary>
        ///  Getter and setter for the query logic of the node.
        /// </summary>
        /// <value></value>
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
        
        /// <summary>
        /// The property class is used to store and access data in a node both in code and in the editor.
        /// </summary>
        [System.Serializable]
        public class Property
        {
            /// <summary>
            /// Constructor for the property class.
            /// </summary>
            public Property()
            {
                name = "";
                value = "";
                GUID = System.Guid.NewGuid().ToString();
            }
            /// <summary>
            ///  Constructor for the property class.
            /// </summary>
            /// <param name="name">The name of the property.</param>
            /// <param name="value">The initial value of the property.</param>
            /// <param name="type">The datatype of the property.</param>
            public Property(string name, object value, System.Type type)
            {
                this.name = name;
                this.value = value;
                this.type = type;
                GUID = System.Guid.NewGuid().ToString();
            }
            
            [SerializeField]
            public string name; ///< The name of the property.

            [SerializeField]
            public string GUID; ///< The GUID of the property.
            [SerializeField]
            public string paramReference; ///< The GUID of the parameter that the property is referencing. If this is not null, the property is a reference to a parameter exposed in the inspector.
            [SerializeField]
            public System.Type type; ///< The datatype of the property.

            public object value; ///< The value of the property.

            public bool output = false; ///< Whether the property is an output parameter or not. This is used to determine whether the property should be given an input port or an output port when shown in the graph editor.
        }

        /// <summary>
        /// A generic template version of the property class.
        /// </summary>
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
        /// <summary>
        ///  An adapted version of the Property class which is made to work with Unity Serialization.
        /// </summary>
        /// <para>
        /// This class can be converted to and from the Property class, for the purpose of serialization.
        /// </para>
        [System.Serializable]
        public class SerializableProperty
        {
            public SerializableProperty(){ GUID = System.Guid.NewGuid().ToString(); } ///< Constructor for the SerializableProperty class.
            
            public SerializableProperty(SerializableProperty copy) ///< Copy constructor for the SerializableProperty class.
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
            public void CopyValues(SerializableProperty from) ///< Copies the values of the SerializableProperty from the given SerializableProperty.
            {
                ivalue = from.ivalue;
                fvalue = from.fvalue;
                bvalue = from.bvalue;
                svalue = from.svalue;
                v2value = from.v2value;
                v3value = from.v3value;
                v4value = from.v4value;
                cvalue = from.cvalue;
                ovalue = from.ovalue;
            }
            public static implicit operator SerializableProperty(Property property) ///< Implicit conversion operator for the SerializableProperty class.
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

            public static implicit operator Property(SerializableProperty serializableProperty) ///< Implicit conversion operator for the SerializableProperty class.
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
            public string name; ///< The name of the property.
            public string GUID; ///< The GUID of the property.
            [SerializeField]
            public string paramReference; ///< The GUID of the parameter that the property is referencing. If this is not null, the property is a reference to a parameter exposed in the inspector.
            public string serializedTypename; ///< The AssemblyQualifiedName of the datatype of the property.
            public System.Type type => System.Type.GetType(serializedTypename); ///< The datatype of the property.
            public string svalue; ///< The string value of the property.
            public int ivalue; ///< The integer value of the property.
            public float fvalue; ///< The float value of the property.
            public bool bvalue; ///< The boolean value of the property.
            public Color cvalue; ///< The color value of the property.
            public Vector2 v2value; ///< The vector2 value of the property.
            public Vector3 v3value; ///< The vector3 value of the property.
            public Vector4 v4value; ///< The vector4 value of the property.
            [SerializeReference]public UnityEngine.Object ovalue; ///< The object reference value of the property.
            public bool output = false; ///< Whether the property is an output property.
        }
        /// <summary>
        /// A class which stores a collection of nodes as a group.
        /// </summary>
        [System.Serializable]
        public struct NodeGroup
        {
            [SerializeField]
            public string title; ///< The title of the group.
            [SerializeField]
            public List<string> childGUIDs; ///< The GUIDs of the nodes in the group.
        }
        
    }


}
