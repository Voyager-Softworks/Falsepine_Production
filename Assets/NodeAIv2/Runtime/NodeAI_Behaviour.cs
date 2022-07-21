/*
 * Bachelor of Software Engineering
 * Media Design School
 * Auckland
 * New Zealand
 * 
 * (c) 2022 Media Design School
 * 
 * File Name: NodeAI_Behaviour.cs
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
    ///  This class is used to store Behaviour Data for Agents.
    /// </summary>
    /// <para>
    /// These objects are created by the NodeAI Graph editor and are used by NodeAI_Agent to determine their behaviour.
    /// </para>
    [System.Serializable]
    public class NodeAI_Behaviour : ScriptableObject
    {
        [SerializeField]
        [HideInInspector]public List<NodeData> nodeData; ///< List of all nodes in the behaviour.
        [SerializeField]
        [HideInInspector]public List<NodeData.SerializableProperty> exposedProperties = new List<NodeData.SerializableProperty>(); ///< List of all exposed properties in the behaviour.

        [SerializeField]
        [HideInInspector]public List<NodeData.NodeGroup> nodeGroups; ///< List of all node groups in the behaviour.

        [SerializeField]
        [HideInInspector]public List<Query> queries = new List<Query>(); ///< List of all queries in the behaviour.

        
    }
}