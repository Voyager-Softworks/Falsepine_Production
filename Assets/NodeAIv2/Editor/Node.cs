using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;

namespace NodeAI
{
    public class Node : UnityEditor.Experimental.GraphView.Node
    {
        public string GUID;
        public NodeData.Type nodeType;

        public RuntimeBase runtimeLogic;
        public Query query;
        public string paramReference;

        public Port inputPort;
        public List<Port> inputPorts = new List<Port>();
        public Port outputPort;
        public List<Port> outputPorts = new List<Port>();

        public List<NodeData.Property> properties;

    }
}