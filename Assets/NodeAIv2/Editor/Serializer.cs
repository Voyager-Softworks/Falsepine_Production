/*
 * Bachelor of Software Engineering
 * Media Design School
 * Auckland
 * New Zealand
 * 
 * (c) 2022 Media Design School
 * 
 * File Name: Serializer.cs
 * Description: Responsible for serializing and deserializing NodeAI Graphs
 * Author: Nerys Thamm
 * Mail: nerysthamm@gmail.com
 */


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEditor;
using System.Linq;

namespace NodeAI
{
    public class Serializer
    {
        private GraphView target; // The graph to serialize

        private List<Edge> edges => target.edges.ToList(); // The edges of the graph
        private List<Node> nodes => target.nodes.ToList().ConvertAll(x => x as Node); // The nodes of the graph

        
        /// <summary>
        /// Initializes the serializer.
        /// </summary>
        public static Serializer GetInstance(GraphView target) 
        {
            return new Serializer
            {
                target = target
            };
            
        }

        /// <summary>
        /// Serializes the graph.
        /// </summary>
        public void Serialize(NodeAI_Behaviour nodeAI_Behaviour)
        {
            nodeAI_Behaviour.nodeData = new List<NodeData>(); // Initialize the node data list
            nodeAI_Behaviour.queries = new List<Query>(); // Initialize the query list

            // Serialize the nodes
            foreach (var node in nodes)
            {
                // Create a new node data
                var nodeData = new NodeData 
                {
                    GUID = node.GUID, // Set the GUID
                    nodeType = node.nodeType, // Set the node type
                    position = node.GetPosition().position, // Set the position
                    childGUIDs = new List<string>(), // Initialize the child GUID list
                    title = node.title, // Set the title
                    runtimeLogic = (node.runtimeLogic ?? null), // Set the runtime logic
                    query = (node.query ?? null) // Set the query
                };

                // Setup ScriptableObject components if they are not null
                if(nodeData.runtimeLogic != null)
                {
                    nodeData.runtimeLogic.state = NodeData.State.Idle;
                }
                if(nodeData.query != null)
                {
                    nodeAI_Behaviour.queries.Add(nodeData.query);
                }

                // Store the parameter reference
                if(node.nodeType == NodeData.Type.Parameter) nodeData.parentGUID = node.paramReference;

                // Serialise connections
                foreach (var input in node.inputPorts)
                {
                    if(nodeData.runtimeLogic != null) //If this node has runtime logic
                    {
                        if (input.connections.Count() > 0) //If this node is connected to anything
                        {
                            if(((Node)input.connections.First().output.node).nodeType == NodeData.Type.Parameter) // If the connected node is a parameter
                            {
                                // Set the properties parameter reference as the nodes' parameter reference
                                nodeData.runtimeLogic.SetPropertyParamReference(input.portName, ((Node)input.connections.First().output.node).paramReference);
                            }
                            else
                            {
                                // Set the properties parameter reference as the nodes' GUID
                                nodeData.runtimeLogic.SetPropertyParamReference(input.portName, ((Node)input.connections.First().output.node).query.GetProperties().Find(x => x.name == input.connections.First().output.portName).GUID);
                            }
                            nodeData.runtimeLogic.SetPropertyGUID(input.portName, ((Node)input.connections.First().output.node).GUID); // Set the properties GUID
                        }
                        else
                        {
                            nodeData.runtimeLogic.SetPropertyParamReference(input.portName, "null"); // Set the properties parameter reference to null
                            nodeData.runtimeLogic.SetPropertyGUID(input.portName, "null"); // Set the properties GUID to null
                        }
                    }
                    else if(nodeData.query != null) 
                    {
                        if (input.connections.Count() > 0) 
                        {
                            if(((Node)input.connections.First().output.node).nodeType == NodeData.Type.Parameter) //If a parameter is connected
                            {
                                // Set the properties parameter reference as the nodes' parameter reference
                                nodeData.query.SetPropertyParamReference(input.portName, ((Node)input.connections.First().output.node).paramReference);
                            }
                            else
                            {
                                // Set the properties parameter reference as the nodes' GUID
                                nodeData.query.SetPropertyParamReference(input.portName, ((Node)input.connections.First().output.node).query.GetProperties().Find(x => x.name == input.connections.First().output.portName).GUID);
                            }
                            // Set the properties GUID
                            nodeData.query.SetPropertyGUID(input.portName, ((Node)input.connections.First().output.node).GUID);
                        }
                        else
                        {
                            nodeData.query.SetPropertyParamReference(input.portName, "null"); // Set the properties parameter reference to null
                            nodeData.query.SetPropertyGUID(input.portName, "null"); // Set the properties GUID to null
                        }
                        
                    }
                }
                nodeAI_Behaviour.nodeData.Add(nodeData); // Add the node data to the list
            }

            foreach (var edge in edges) 
            {
                if(((Node)edge.output.node).nodeType == NodeData.Type.Parameter) //Parameter specific logic
                {
                    ((Node)edge.input.node).runtimeLogic?.SetPropertyGUID(edge.input.portName, ((Node)edge.output.node).GUID); 
                    ((Node)edge.input.node).query?.SetPropertyGUID(edge.input.portName, ((Node)edge.output.node).GUID);
                }
                else if(((Node)edge.output.node).nodeType == NodeData.Type.Query) //Query specific logic
                {
                    ((Node)edge.input.node).runtimeLogic?.SetPropertyParamReference(edge.input.portName, ((Node)edge.output.node).query.GetProperties().Find(x => x.name == edge.output.portName).GUID);
                    ((Node)edge.input.node).query?.SetPropertyParamReference(edge.input.portName, ((Node)edge.output.node).query.GetProperties().Find(x => x.name == edge.output.portName).GUID);
                    ((Node)edge.input.node).runtimeLogic?.SetPropertyGUID(edge.input.portName, ((Node)edge.output.node).GUID); 
                    ((Node)edge.input.node).query?.SetPropertyGUID(edge.input.portName, ((Node)edge.output.node).GUID);
                }
                else
                {
                    var inputNodeData = nodeAI_Behaviour.nodeData.Find(x => x.GUID == ((Node)edge.input.node).GUID);
                    var outputNodeData = nodeAI_Behaviour.nodeData.Find(x => x.GUID == ((Node)edge.output.node).GUID);

                    inputNodeData.parentGUID = outputNodeData.GUID;
                    outputNodeData.childGUIDs.Add(inputNodeData.GUID);
                }
            }
            
            
            //Repopulate exposed properties
            nodeAI_Behaviour.exposedProperties.Clear();
            foreach(var p in target.exposedProperties)
            {
                nodeAI_Behaviour.exposedProperties.Add(p);
            }

            //Serialise Groups
            if(nodeAI_Behaviour.nodeGroups == null) nodeAI_Behaviour.nodeGroups = new List<NodeData.NodeGroup>();
            nodeAI_Behaviour.nodeGroups.Clear();
            foreach(var g in target.graphElements.ToList().OfType<Group>())
            {
                NodeData.NodeGroup nodeGroup = new NodeData.NodeGroup
                {
                    title = g.title,
                    childGUIDs = new List<string>()
                };
                foreach(var n in g.containedElements.ToList().OfType<Node>())
                {
                    nodeGroup.childGUIDs.Add(n.GUID);
                }
                nodeAI_Behaviour.nodeGroups.Add(nodeGroup);
            }
            
            
        }


        /// <summary>
        /// Deserializes the graph.
        /// </summary>
        public void Deserialize(NodeAI_Behaviour nodeAI_Behaviour)
        {
            // Clear the graph
            target.exposedProperties.Clear();
            target.blackboard.Clear();

            // Deserialise exposed properties
            foreach (var property in nodeAI_Behaviour.exposedProperties)
            {
                target.AddPropertyToBlackboard(property);
            }
            
            // Clear nodes and edges
            foreach (var node in nodes)
            {
                target.RemoveElement(node);
            }
            foreach (var edge in edges)
            {
                target.RemoveElement(edge);
            }

            // Deserialise nodes
            foreach (var nodeData in nodeAI_Behaviour.nodeData)
            {
                var node = target.GenerateNode(nodeData);
                target.AddElement(node);
                nodes.Add(node);
            }

            // Deserialise edges
            foreach (var nodeData in nodeAI_Behaviour.nodeData)
            {
                var node = nodes.Find(x => x.GUID == nodeData.GUID);

                // If the node can have sequence inputs, then apply them
                if (nodeData.nodeType != NodeData.Type.EntryPoint && 
                    nodeData.nodeType != NodeData.Type.Parameter && 
                    nodeData.nodeType != NodeData.Type.Query)
                {
                    var parent = nodes.Find(x => x.GUID == nodeData.parentGUID);
                    var edge = parent.outputPort.ConnectTo(node.inputPort);
                    edges.Add(edge);
                    target.AddElement(edge);
                }
                else if(nodeData.nodeType == NodeData.Type.Parameter) // If the node is a parameter then deserialize its parameter reference
                {
                    node.paramReference = nodeData.parentGUID;
                    
                }
                foreach(Port input in node.inputPorts) //Reconnect edges
                {
                    if(nodeData.runtimeLogic != null)
                    {
                        NodeData.Property property = nodeData.runtimeLogic.GetProperties().Find(x => x.name == input.portName);
                        string connGUID = property.GUID;
                        var conn = nodes.Find(x => x.GUID == connGUID);
                        if (conn != null)
                        {
                            if(conn.nodeType == NodeData.Type.Query)
                            {
                                var outputPort = conn.outputPorts.Find(x => x.portName == conn.query.GetProperties().Find(y => y.GUID == property.paramReference).name);
                                var edge = outputPort.ConnectTo(input);
                                edges.Add(edge);
                                target.AddElement(edge);
                            }
                            else
                            {
                                var edge = conn.outputPort.ConnectTo(input);
                                edges.Add(edge);
                                target.AddElement(edge);
                            }
                            
                        }
                    }
                    else if(nodeData.query != null)
                    {
                        NodeData.Property property = nodeData.query.GetProperties().Find(x => x.name == input.portName);
                        string connGUID = property.GUID;
                        var conn = nodes.Find(x => x.GUID == connGUID);
                        if (conn != null)
                        {
                            if(conn.nodeType == NodeData.Type.Query)
                            {
                                var outputPort = conn.outputPorts.Find(x => x.portName == conn.query.GetProperties().Find(y => y.GUID == property.paramReference).name);
                                var edge = outputPort.ConnectTo(input);
                                edges.Add(edge);
                                target.AddElement(edge);
                            }
                            else
                            {
                                var edge = conn.outputPort.ConnectTo(input);
                                edges.Add(edge);
                                target.AddElement(edge);
                            }
                        }
                    }
                }
                
            }
            target.graphElements.ToList().OfType<Group>().ToList().ForEach(x => target.RemoveElement(x));
            foreach (var nodeGroup in nodeAI_Behaviour.nodeGroups)
            {
                List<Node> nodesToAdd = new List<Node>();
                foreach (var nodeGUID in nodeGroup.childGUIDs)
                {
                    var node = nodes.Find(x => x.GUID == nodeGUID);
                    nodesToAdd.Add(node);
                }

                target.CreateGroup(nodeGroup.title, nodesToAdd);
                
            }
            
            

            
        }

        

        
    }
}
