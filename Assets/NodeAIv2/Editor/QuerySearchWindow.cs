using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using System.Reflection;
using System.Linq;
using System;

namespace NodeAI
{
    public class QuerySearchWindow : ScriptableObject, ISearchWindowProvider
    {
        private GraphView graphView;

        

        public void Init(GraphView graphView)
        {
            this.graphView = graphView;
        }

        



        Type[] GetInheritedClasses(Type MyType) 
        {
            return Assembly.GetAssembly(MyType).GetTypes().Where(TheType => TheType.IsClass && !TheType.IsAbstract && TheType.IsSubclassOf(MyType)).ToArray();
        }

        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            // Split inherited classes into lists based on their namespace
            Type[] queries = GetInheritedClasses(typeof(Query));
            
            // Create a list of all namespaces
            var namespaces = new List<string>();
            foreach (var query in queries)
            {
                if (!namespaces.Contains(query.Namespace))
                {
                    namespaces.Add(query.Namespace);
                }
            }

            var tree = new List<SearchTreeEntry>
            {
                new SearchTreeGroupEntry(new GUIContent("New Query"), 0),
            };
            
            foreach (var n in namespaces)
            {
                if(n == "NodeAI")
                {
                    foreach (var query in queries)
                    {
                        if (query.Namespace == n)
                        {
                            tree.Add(new SearchTreeEntry(new GUIContent(query.Name))
                            {
                                userData = query, level = 1
                            });
                        }
                    }
                }
                else
                {
                    tree.Add(new SearchTreeGroupEntry(new GUIContent(n ?? "Custom"), 3));
                    foreach (var query in queries)
                    {
                        if (query.Namespace == n)
                        {
                            tree.Add(new SearchTreeEntry(new GUIContent(query.Name))
                            {
                                userData = query, level = 2
                            });
                        }
                    }
                }
            }
            
            return tree;
        }

        public bool OnSelectEntry(SearchTreeEntry entry, SearchWindowContext context)
        {
            
            graphView.AddElement(graphView.GenerateQueryNode(((Type)entry.userData).Name, (Query)ScriptableObject.CreateInstance(((Type)entry.userData)), context.screenMousePosition));
        
            return true;
        }
    }
}
