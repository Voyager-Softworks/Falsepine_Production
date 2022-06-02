/*
 * Bachelor of Software Engineering
 * Media Design School
 * Auckland
 * New Zealand
 * 
 * (c) 2022 Media Design School
 * 
 * File Name: QuerySearchWindow.cs
 * Description: 
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

        public void PopulateSearchTreeRecursively(List<SearchTreeEntry> searchTree, NamespaceParser.NamespaceEntry entry, int depth = 0)
        {
            if(entry.Namespace == "" || entry.Namespace == "NodeAI") depth = 0;
            else searchTree.Add(new SearchTreeGroupEntry(new GUIContent(entry.Namespace), depth));
            foreach (var subnamespace in entry.Subnamespaces)
            {
                PopulateSearchTreeRecursively(searchTree, subnamespace, depth + 1);
            }

            foreach (var type in entry.Classes)
            {
                searchTree.Add(new SearchTreeEntry(new GUIContent(type.Name))
                            {
                                userData = type, level = depth + 1
                            });
            }
        }
        
        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            // Split inherited classes into lists based on their namespace
            Type[] queries = GetInheritedClasses(typeof(Query));

            var parsedTree = NamespaceParser.TreeFromTypes(queries);
            
            // Create a list of all namespaces
            // var namespaces = new List<string>();
            // foreach (var query in queries)
            // {
            //     if (!namespaces.Contains(query.Namespace))
            //     {
            //         namespaces.Add(query.Namespace);
            //     }
            // }

            var tree = new List<SearchTreeEntry>
            {
                new SearchTreeGroupEntry(new GUIContent("New Query"), 0),
            };
            

            PopulateSearchTreeRecursively(tree, parsedTree, 0);



            // foreach (var n in namespaces)
            // {
            //     if(n.Contains("NodeAI"))
            //     {
            //         foreach (var query in queries)
            //         {
            //             if (query.Namespace == "NodeAI")
            //             {
            //                 tree.Add(new SearchTreeEntry(new GUIContent(query.Name))
            //                 {
            //                     userData = query, level = 1
            //                 });
            //             }
            //         }
            //     }
            //     else
            //     {
            //         tree.Add(new SearchTreeGroupEntry(new GUIContent(n ?? "Custom"), 1));
            //         foreach (var query in queries)
            //         {
            //             if (query.Namespace == n)
            //             {
            //                 tree.Add(new SearchTreeEntry(new GUIContent(query.Name))
            //                 {
            //                     userData = query, level = 2
            //                 });
            //             }
            //         }
            //     }
            // }
            
            return tree;
        }

        public bool OnSelectEntry(SearchTreeEntry entry, SearchWindowContext context)
        {
            
            graphView.AddElement(graphView.GenerateQueryNode(((Type)entry.userData).Name, (Query)ScriptableObject.CreateInstance(((Type)entry.userData)), context.screenMousePosition));
        
            return true;
        }
    }
}
