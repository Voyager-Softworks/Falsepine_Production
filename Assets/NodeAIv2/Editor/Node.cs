/*
 * Bachelor of Software Engineering
 * Media Design School
 * Auckland
 * New Zealand
 * 
 * (c) 2022 Media Design School
 * 
 * File Name: Node.cs
 * Description: 
 * Author: Nerys Thamm
 * Mail: nerysthamm@gmail.com
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;

namespace NodeAI
{
    /// <summary>
    /// The Node class is used to store data for a node in a NodeAI Graph.
    /// </summary>
    public class Node : UnityEditor.Experimental.GraphView.Node
    {
        public string GUID; ///< The GUID of the node.
        public NodeData.Type nodeType; ///< The type of the node.

        public RuntimeBase runtimeLogic; ///< The runtime logic of the node.
        public Query query; ///< The query of the node.
        public string paramReference; ///< The reference to the parameter of the node.

        public Port inputPort; ///< The sequential input port of the node.
        public List<Port> inputPorts = new List<Port>(); ///< The data input ports of the node.
        public Port outputPort; ///< The sequential output port of the node.
        public List<Port> outputPorts = new List<Port>(); ///< The data output ports of the node.

        public List<NodeData.Property> properties; ///< The properties of the node.

    }
}