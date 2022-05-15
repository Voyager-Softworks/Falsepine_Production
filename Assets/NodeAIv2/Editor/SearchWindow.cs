using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using System.Reflection;
using System.Linq;
using System;

namespace NodeAI
{
    public class SearchWindow : ScriptableObject, ISearchWindowProvider
    {
        private GraphView graphView;

        private Node selectedNode;

        public void Init(GraphView graphView)
        {
            this.graphView = graphView;
        }

        public void SetSelectedNode(Node node)
        {
            selectedNode = node;
        }



        Type[] GetInheritedClasses(Type MyType) 
        {
            return Assembly.GetAssembly(MyType).GetTypes().Where(TheType => TheType.IsClass && !TheType.IsAbstract && TheType.IsSubclassOf(MyType)).ToArray();
        }

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


            var tree = new List<SearchTreeEntry>
            {
                new SearchTreeGroupEntry(new GUIContent("New Node"), 0),
                new SearchTreeGroupEntry(new GUIContent("Control Node"), 1),
                new SearchTreeEntry(new GUIContent("Sequence"))
                {
                    userData = typeof(Sequence), level = 2
                },
                new SearchTreeEntry(new GUIContent("Selector"))
                {
                    userData = typeof(Selector), level = 2
                },
                new SearchTreeEntry(new GUIContent("Parallel"))
                {
                    userData = typeof(Parallel), level = 2
                },
                
            };
            tree.Add(new SearchTreeGroupEntry(new GUIContent("Decorators"), 2));
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
                    tree.Add(new SearchTreeGroupEntry(new GUIContent(n == null ? "Custom" : n), 3));
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
                    tree.Add(new SearchTreeGroupEntry(new GUIContent(n == null ? "Custom" : n), 2));
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
                    tree.Add(new SearchTreeGroupEntry(new GUIContent(n == null ? "Custom" : n), 2));
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

        public bool OnSelectEntry(SearchTreeEntry entry, SearchWindowContext context)
        {
            Node newNode = null;
            if(((Type)entry.userData) == typeof(Sequence))
            {
                newNode = graphView.ContextCreateNode(selectedNode, NodeData.Type.Sequence, ((Type)entry.userData).Name, ScriptableObject.CreateInstance<Sequence>());
                
            }
            else if(((Type)entry.userData) == typeof(Selector))
            {
                newNode = graphView.ContextCreateNode(selectedNode, NodeData.Type.Selector, ((Type)entry.userData).Name, ScriptableObject.CreateInstance<Selector>());
                
            }
            else if(((Type)entry.userData) == typeof(Parallel))
            {
                newNode = graphView.ContextCreateNode(selectedNode, NodeData.Type.Parallel, ((Type)entry.userData).Name, ScriptableObject.CreateInstance<Parallel>());
                
            }
            else if(((Type)entry.userData).BaseType == typeof(DecoratorBase))
            {
                newNode = graphView.ContextCreateNode(selectedNode, NodeData.Type.Decorator, ((Type)entry.userData).Name, (DecoratorBase)ScriptableObject.CreateInstance(((Type)entry.userData)));
                
            }
            else if(((Type)entry.userData).BaseType == typeof(ActionBase))
            {
                newNode = graphView.ContextCreateNode(selectedNode, NodeData.Type.Action, ((Type)entry.userData).Name, (RuntimeBase)ScriptableObject.CreateInstance(((Type)entry.userData)));
                
            }
            else if(((Type)entry.userData).BaseType == typeof(ConditionBase))
            {
                newNode = graphView.ContextCreateNode(selectedNode, NodeData.Type.Condition, ((Type)entry.userData).Name, (RuntimeBase)ScriptableObject.CreateInstance(((Type)entry.userData)));
            }
            return true;
        }
    }
}
