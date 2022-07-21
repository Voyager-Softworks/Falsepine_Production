/*
 * Bachelor of Software Engineering
 * Media Design School
 * Auckland
 * New Zealand
 * 
 * (c) 2022 Media Design School
 * 
 * File Name: NodeAI_Agent.cs
 * Description: 
 * Author: Nerys Thamm
 * Mail: nerysthamm@gmail.com
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace NodeAI
{
    /// <summary>
    ///  The Component responsible for enabling NodeAI_Behaviour files to be used to control the behaviour of GameObjects.
    /// </summary>
    /// <para>
    /// This component is attached to GameObjects.
    /// </para>
    public class NodeAI_Agent : MonoBehaviour
    {
        public NodeAI_Behaviour AI_Behaviour; ///< The NodeAI_Behaviour that this Agent is using.
        NodeAI_Behaviour _behaviour;
        public string faction = ""; ///< The faction that this Agent belongs to.

        [SerializeField]
        public List<NodeData.SerializableProperty> inspectorProperties; ///< List of all exposed properties in the behaviour.

        Dictionary<NodeData.SerializableProperty, List<NodeData.SerializableProperty>> _propertyMap; ///< Dictionary of all exposed properties in the behaviour.

        public NodeAI_Behaviour behaviour{ ///< The NodeAI_Behaviour that this Agent is using.
            get{
                return _behaviour;
            }
        }
        public NodeTree nodeTree = null; ///< The NodeTree that this Agent is using.

        const float tickRate = 0.01f; ///< The rate at which the Agent will tick.
        float tickTimer = 0f; ///< The timer for the Agent's tick.

        // Start is called before the first frame update
        void Start()
        {
            if(AI_Behaviour == null)
            {
                Debug.LogError("No AI_Behaviour assigned to NodeAI_Agent " + gameObject.name);
                return;
            }
            _behaviour = Instantiate(AI_Behaviour);
            
            _behaviour.nodeData.Where(x => !x.noLogic).All(x => x.runtimeLogic = (RuntimeBase)ScriptableObject.Instantiate(x.runtimeLogic));
            _behaviour.nodeData.Where(x => !x.noQuery).All(x => x.query = (Query)ScriptableObject.Instantiate(x.query));
            _behaviour.queries.Clear();
            _behaviour.queries.AddRange(_behaviour.nodeData.Where(x => !x.noQuery).Select(x => x.query));
            _behaviour.nodeData.Where(x => !x.noLogic).ToList().ForEach(x => x.runtimeLogic.state = NodeData.State.Idle);
            foreach(NodeData.SerializableProperty p in inspectorProperties)
            {
                NodeData.SerializableProperty  other = _behaviour.exposedProperties.Where(x => x.GUID == p.GUID).First();
                other.ivalue = p.ivalue;
                other.fvalue = p.fvalue;
                other.svalue = p.svalue;
                other.bvalue = p.bvalue;
                other.ovalue = p.ovalue;
                other.v2value = p.v2value;
                other.v3value = p.v3value;
                other.v4value = p.v4value;
                other.cvalue = p.cvalue;
            }
            nodeTree = NodeTree.CreateFromNodeData(_behaviour.nodeData.Find(x => x.nodeType == NodeData.Type.EntryPoint), _behaviour.nodeData);;
            nodeTree.rootLeaf.nodeData.runtimeLogic.Init(nodeTree.rootLeaf);
            nodeTree.PropogateExposedProperties(_behaviour.exposedProperties);

            _propertyMap = new Dictionary<NodeData.SerializableProperty, List<NodeData.SerializableProperty>>();
            foreach(NodeData.SerializableProperty p in _behaviour.exposedProperties)
            {
                if(!_propertyMap.ContainsKey(p))
                {
                    _propertyMap[p] = new List<NodeData.SerializableProperty>();
                    
                }
                _propertyMap[p].AddRange(nodeTree.nodes.Where(x => !x.noLogic).SelectMany(x => x.runtimeLogic.GetPropertiesWhereParamReference(p.GUID)));
                _propertyMap[p].AddRange(nodeTree.nodes.Where(x => !x.noQuery).SelectMany(x => x.query.GetPropertiesWhereParamReference(p.GUID)));
                
                
            }
            foreach(Query q in _behaviour.queries)
            {
                q.GetSerializableProperties().Where(x => x.output).ToList().ForEach(x => 
                {
                    if(!_propertyMap.ContainsKey(x))
                    {
                        _propertyMap[x] = new List<NodeData.SerializableProperty>();
                        
                    }
                    _propertyMap[x].AddRange(nodeTree.nodes.Where(n => !n.noQuery).SelectMany(n => n.query.GetPropertiesWhereParamReference(x.GUID)));
                    _propertyMap[x].AddRange(nodeTree.nodes.Where(n => !n.noLogic).SelectMany(n => n.runtimeLogic.GetPropertiesWhereParamReference(x.GUID)));
                    _propertyMap[x].AddRange(_behaviour.queries.SelectMany(n => n.GetPropertiesWhereParamReference(x.GUID)));
                    
                });
            }
            

        }

    

        // Update is called once per frame
        void Update()
        {
            if(_behaviour == null)
            {
                return;
            }
            tickTimer += Time.deltaTime;
            
            foreach (var query in _behaviour.queries)
            {
                query.GetNewValues(this);
            }
            _propertyMap?.Keys.ToList().ForEach(x =>
            {
                foreach(NodeData.SerializableProperty p in _propertyMap[x])
                {
                    p.ivalue = x.ivalue;
                    p.fvalue = x.fvalue;
                    p.svalue = x.svalue;
                    p.bvalue = x.bvalue;
                    p.ovalue = x.ovalue;
                    p.v2value = x.v2value;
                    p.v3value = x.v3value;
                    p.v4value = x.v4value;
                    p.cvalue = x.cvalue;
                }
            });
            if (tickTimer > tickRate)
            {
                tickTimer = 0f;
                nodeTree.rootNode.Eval(this, nodeTree.rootLeaf);
            }
        }

        /// <summary>
        ///  Sets the value of a parameter.
        /// </summary>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="value">The value the parameter should be set to.</param>
        /// <typeparam name="T">The datatype of the parameter.</typeparam>
        public void SetParameter<T>(string name, T value)
        {
            if(_behaviour == null)
            {
                return;
            }
            foreach (var property in _behaviour.exposedProperties)
            {
                if (property.name == name && property.type == typeof(T))
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
                            break;
                    }
                    nodeTree.PropogateExposedProperties(_behaviour.exposedProperties);
                    return;
                }
            }
            
        }

        /// <summary>
        ///  Gets the value of a parameter.
        /// </summary>
        /// <param name="name">The name of the parameter.</param>
        /// <typeparam name="T">The datatype of the parameter.</typeparam>
        /// <returns>The current value of the parameter with the given name and type</returns>
        /// <remarks>
        /// If the parameter does not exist, the default value of the parameter's type is returned.
        /// </remarks>
        public T GetParameter<T>(string name)
        {
            if(_behaviour == null)
            {
                return default(T);
            }
            foreach (var property in _behaviour.exposedProperties)
            {
                if (property.name == name && property.type == typeof(T))
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
                            break;
                    }
                }
            }
            return default(T);
        }

        /// <summary>
        /// Draws node Gizmos
        /// </summary>
        void OnDrawGizmos()
        {
            Gizmos.DrawIcon(transform.position, "Profiler.Memory", true, Color.magenta);
            if (_behaviour != null)
            {
                nodeTree.DrawGizmos(this);
            }
        }
    }
}
