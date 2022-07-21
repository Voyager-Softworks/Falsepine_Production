/*
 * Bachelor of Software Engineering
 * Media Design School
 * Auckland
 * New Zealand
 * 
 * (c) 2022 Media Design School
 * 
 * File Name: Query.cs
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
    /// Base class for all query nodes.
    /// </summary>
    /// <para>
    /// A query node is a node which executes some form of logic at runtime which is not directly part of the Agent's behaviour.
    /// Query nodes can have any number of input and output properties, and can be used to either process said inputs in some way and return the result as an output, collect information from the scene, or both.
    /// </para>
    /// <para>
    /// The following snippet shows how to create a query node that takes a Color property and returns its intensity as an output:
    /// </para>   
    /// <example>
    /// <code>
    /// public class ColorIntensity : Query
    /// {
    ///    public ColorIntensity()
    ///    {
    ///       AddProperty<Color>("Color", Color.white, false);
    ///       AddProperty<float>("Intensity", 0f, true); 
    ///    }
    ///    public override void GetNewValues(NodeAI_Agent agent)
    ///    {
    ///      float r = GetProperty<Color>("Color").r;
    ///      float g = GetProperty<Color>("Color").g;
    ///      float b = GetProperty<Color>("Color").b;
    ///      float intensity = (0.21f × r) + (0.72f × g) + (0.07f × b);
    ///      SetProperty<float>("Intensity", intensity);
    ///    }
    /// }
    /// </code>
    /// </example>
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

        /// <summary>
        /// The tooltip that is displayed when the mouse is over the node.
        /// </summary>
        public string tooltip = ""; // < Tooltip to display in the inspector
        
        [SerializeField]
        List<NodeData.SerializableProperty> properties = new List<NodeData.SerializableProperty>(); // Properties

        /// <summary>
        ///  Repopulates the properties list.
        /// </summary>
        /// <param name="properties"></param>
        public void RepopulateProperties(List<NodeData.SerializableProperty> properties) // Repopulates properties when they are lost in serialisation
        {
            this.properties = properties;
        }

        /// <summary>
        ///  Adds a property to the Query.
        /// </summary>
        /// <param name="name">The name of the property to add.</param>
        /// <param name="initialValue">The initial value of the property.</param>
        /// <param name="isOutput">Whether this property is an output or not.</param>
        /// <typeparam name="T">The datatype of this property</typeparam>
        /// <example>
        /// <code>
        /// AddProperty<int>("MyInt", 0, false); // Adds a property called "MyInt" with a value of 0 which is an input
        /// AddProperty<bool>("MyBool", false, true); // Adds a property called "MyBool" with a value of false which is an output
        /// //Whether a property is an input or an output determines whether it recieves an input port or an output port on the Node.
        /// </code>
        /// </example>
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

        /// <summary>
        /// Sets the value of a property.
        /// </summary>
        /// <param name="name">The name of the property to set.</param>
        /// <param name="value">The value to set the property to.</param>
        /// <typeparam name="T">The datatype of the property.</typeparam>
        /// <example>
        /// <code>
        /// SetProperty<int>("MyInt", 5); // Sets the value of the property called "MyInt" to 5
        /// </code>
        /// </example>
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

        /// <summary>
        /// Sets the value of a property.
        /// </summary>
        /// <param name="name">The name of the property to set.</param>
        /// <param name="value">The value to set the property to.</param>
        /// <typeparam name="T">The datatype of the property.</typeparam>
        /// <example>
        /// <code>
        /// SetProperty("MyInt", (Object)5, typeof(int)); // Sets the value of the property called "MyInt" to 5
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
        ///  Sets the GUID of a property.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <param name="GUID">The GUID to set.</param>
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
        ///  Sets the Parameter reference of a property.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <param name="paramReference">The GUID of the parameter.</param>
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
        /// Gets the value of a property.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <typeparam name="T">The datatype of the property.</typeparam>
        /// <returns>The value of the property</returns>
        /// <remarks>
        /// <para>If the property does not exist, the method will return the default value of the property's datatype.</para>
        /// </remarks>
        /// <example>
        /// <code>
        /// int myInt = GetProperty<int>("MyInt"); // Gets the value of the property called "MyInt"
        /// </code>
        /// </example>
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

        /// <summary>
        ///  Gets all the properties of a node.
        /// </summary>
        /// <returns>A list of all properties.</returns>
        public List<NodeData.Property> GetProperties()
        {
            if(properties == null) properties = new List<NodeData.SerializableProperty>();
            return properties.ConvertAll(x => (NodeData.Property)x);
        }
        /// <summary>
        ///  Gets all properties of a node as serializable properties.
        /// </summary>
        /// <returns></returns>
        public List<NodeData.SerializableProperty> GetSerializableProperties()
        {
            if(properties == null) properties = new List<NodeData.SerializableProperty>();
            return properties;
        }
        /// <summary>
        ///  Gets all the properties of a node as serializable properties, where the parameter reference of the property is equal to the provided parameter reference.
        /// </summary>
        /// <param name="paramReference"></param>
        /// <returns></returns>
        public List<NodeData.SerializableProperty> GetPropertiesWhereParamReference(string paramReference)
        {
            if (properties == null) properties = new List<NodeData.SerializableProperty>();
            return properties.FindAll(x => (x).paramReference == paramReference);
        }

        /// <summary>
        /// This method is called when the Query is updated, and should be overriden to define custom behaviour.
        /// </summary>
        /// <param name="agent">The agent processing the Query.</param>
        /// <example>
        /// <code>
        /// public override void GetNewValues(NodeAI_Agent agent)
        /// {
        ///    // Get the value of the property called "MyInt"
        ///     int myInt = GetProperty<int>("MyInt");
        ///     // Set the value of the property called "MyInt" to the value of the property called "MyInt2"
        ///    SetProperty("MyInt", GetProperty<int>("MyInt2"));
        /// }
        /// </code>
        /// </example>   
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