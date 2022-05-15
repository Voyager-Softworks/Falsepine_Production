using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NodeAI
{
    [System.Serializable]
    public class NodeAI_Behaviour : ScriptableObject
    {
        [SerializeField]
        [HideInInspector]public List<NodeData> nodeData;
        [SerializeField]
        [HideInInspector]public List<NodeData.SerializableProperty> exposedProperties = new List<NodeData.SerializableProperty>();

        [SerializeField]
        [HideInInspector]public List<NodeData.NodeGroup> nodeGroups;

        
    }
}