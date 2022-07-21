/*
 * Bachelor of Software Engineering
 * Media Design School
 * Auckland
 * New Zealand
 * 
 * (c) 2022 Media Design School
 * 
 * File Name: SearchWindow.cs
 * Description: Search window for creating new nodes
 * Author: Nerys Thamm
 * Mail: nerysthamm@gmail.com
 */

using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using System.Reflection;
using System.Linq;
using System;

namespace NodeAI
{
    /// <summary>
    /// A search window provider for the NodeAI graph.
    /// </summary>
    public class SearchWindow : ScriptableObject, ISearchWindowProvider
    {
        private GraphView graphView;

        private Node selectedNode;

        /// <summary>
        /// Initializes the search window provider.
        /// </summary>
        public void Init(GraphView graphView)
        {
            this.graphView = graphView;
        }

        /// <summary>
        /// Sets the selected node.
        /// </summary>
        public void SetSelectedNode(Node node)
        {
            selectedNode = node;
        }


        /// <summary>
        /// Gets all child classes of the given type.
        /// </summary>
        /// <param name="MyType">The type to get the child classes of.</param>
        /// <returns>A list of child classes.</returns>
        Type[] GetInheritedClasses(Type MyType) 
        {
            return Assembly.GetAssembly(MyType).GetTypes().Where(TheType => TheType.IsClass && !TheType.IsAbstract && TheType.IsSubclassOf(MyType)).ToArray();
        }

        /// <summary>
        /// Creates the search tree for the search window.
        /// </summary>
        /// <param name="context">The search window context.</param>
        /// <returns>The search tree.</returns>
        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            // Split inherited classes into lists based on their namespace
            Type[] decorators = GetInheritedClasses(typeof(DecoratorBase));
            Type[] actions = GetInheritedClasses(typeof(ActionBase));
            Type[] conditions = GetInheritedClasses(typeof(ConditionBase));

            // Create a list of all namespaces
            var namespaces = new List<string>();
            foreach (var decorator in decorators)
            {
                if (!namespaces.Contains(decorator.Namespace))
                {
                    namespaces.Add(decorator.Namespace);
                }
            }
            foreach (var action in actions)
            {
                if (!namespaces.Contains(action.Namespace))
                {
                    namespaces.Add(action.Namespace);
                }
            }
            foreach (var condition in conditions)
            {
                if (!namespaces.Contains(condition.Namespace))
                {
                    namespaces.Add(condition.Namespace);
                }
            }

            // Create the tree
            var tree = new List<SearchTreeEntry>
            {
                new SearchTreeGroupEntry(new GUIContent("New Node"), 0),
                new SearchTreeGroupEntry(new GUIContent("Control Node"), 1),
                new SearchTreeEntry(new GUIContent("Sequence"))
                {
                    userData = typeof(Sequence),
                    level = 2
                },
                new SearchTreeEntry(new GUIContent("Selector"))
                {
                    userData = typeof(Selector),
                    level = 2
                },
                new SearchTreeEntry(new GUIContent("Parallel"))
                {
                    userData = typeof(Parallel),
                    level = 2
                },
                new SearchTreeGroupEntry(new GUIContent("Decorators"), 2)
            };
            foreach (var n in namespaces)
            {
                if(n == "NodeAI")
                {
                    foreach (var decorator in decorators)
                    {
                        if (decorator.Namespace == n)
                        {
                            tree.Add(new SearchTreeEntry(new GUIContent(decorator.Name))
                            {
                                userData = decorator, level = 3
                            });
                        }
                    }
                }
                else
                {
                    tree.Add(new SearchTreeGroupEntry(new GUIContent(n ?? "Custom"), 3));
                    foreach (var decorator in decorators)
                    {
                        if (decorator.Namespace == n)
                        {
                            tree.Add(new SearchTreeEntry(new GUIContent(decorator.Name))
                            {
                                userData = decorator, level = 4
                            });
                        }
                    }
                }
            }
            tree.Add(new SearchTreeGroupEntry(new GUIContent("Action"), 1));
            foreach (var n in namespaces)
            {
                if(n == "NodeAI")
                {
                    foreach (var action in actions)
                    {
                        if (action.Namespace == n)
                        {
                            tree.Add(new SearchTreeEntry(new GUIContent(action.Name))
                            {
                                userData = action, level = 2
                            });
                        }
                    }
                }
                else
                {
                    tree.Add(new SearchTreeGroupEntry(new GUIContent(n ?? "Custom"), 2));
                    foreach (var action in actions)
                    {
                        if (action.Namespace == n)
                        {
                            tree.Add(new SearchTreeEntry(new GUIContent(action.Name))
                            {
                                userData = action, level = 3
                            });
                        }
                    }
                }
            }
            tree.Add(new SearchTreeGroupEntry(new GUIContent("Condition"), 1));
            foreach (var n in namespaces)
            {
                if(n == "NodeAI")
                {
                    foreach (var condition in conditions)
                    {
                        if (condition.Namespace == n)
                        {
                            tree.Add(new SearchTreeEntry(new GUIContent(condition.Name))
                            {
                                userData = condition, level = 2
                            });
                        }
                    }
                }
                else
                {
                    tree.Add(new SearchTreeGroupEntry(new GUIContent(n ?? "Custom"), 2));
                    foreach (var condition in conditions)
                    {
                        if (condition.Namespace == n)
                        {
                            tree.Add(new SearchTreeEntry(new GUIContent(condition.Name))
                            {
                                userData = condition, level = 3
                            });
                        }
                    }
                }
            }
            return tree;
        }

        /// <summary>
        /// Creates a new node.
        /// </summary>
        /// <param name="context">The search window context.</param>
        /// <returns>Boolean indicating if the node was created.</returns>
        public bool OnSelectEntry(SearchTreeEntry entry, SearchWindowContext context)
        {
            
            if (((Type)entry.userData) == typeof(Sequence))
            {
                graphView.ContextCreateNode(selectedNode, NodeData.Type.Sequence, ((Type)entry.userData).Name, ScriptableObject.CreateInstance<Sequence>());
                
            }
            else if(((Type)entry.userData) == typeof(Selector))
            {
                graphView.ContextCreateNode(selectedNode, NodeData.Type.Selector, ((Type)entry.userData).Name, ScriptableObject.CreateInstance<Selector>());
                
            }
            else if(((Type)entry.userData) == typeof(Parallel))
            {
                graphView.ContextCreateNode(selectedNode, NodeData.Type.Parallel, ((Type)entry.userData).Name, ScriptableObject.CreateInstance<Parallel>());
                
            }
            else if(((Type)entry.userData).BaseType == typeof(DecoratorBase))
            {
                graphView.ContextCreateNode(selectedNode, NodeData.Type.Decorator, ((Type)entry.userData).Name, (DecoratorBase)ScriptableObject.CreateInstance(((Type)entry.userData)));
                
            }
            else if(((Type)entry.userData).BaseType == typeof(ActionBase))
            {
                graphView.ContextCreateNode(selectedNode, NodeData.Type.Action, ((Type)entry.userData).Name, (RuntimeBase)ScriptableObject.CreateInstance(((Type)entry.userData)));
                
            }
            else if(((Type)entry.userData).BaseType == typeof(ConditionBase))
            {
                graphView.ContextCreateNode(selectedNode, NodeData.Type.Condition, ((Type)entry.userData).Name, (RuntimeBase)ScriptableObject.CreateInstance(((Type)entry.userData)));
            }
            return true;
        }
    }
}
