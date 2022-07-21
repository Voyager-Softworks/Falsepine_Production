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
    /// <summary>
    /// The search window for queries.
    /// </summary>
    public class QuerySearchWindow : ScriptableObject, ISearchWindowProvider
    {
        private GraphView graphView; ///< The GraphView that is currently being searched.

        
        /// <summary>
        ///  This function is called when the search window is opened.
        /// </summary>
        /// <param name="graphView"></param>
        public void Init(GraphView graphView)
        {
            this.graphView = graphView;
        }

        /// <summary>
        ///  Used to get all child classes of a given type.
        /// </summary>
        /// <param name="MyType">The type to check.</param>
        /// <returns>An array of all child types of the provided type.</returns>
        Type[] GetInheritedClasses(Type MyType) 
        {
            return Assembly.GetAssembly(MyType).GetTypes().Where(TheType => TheType.IsClass && !TheType.IsAbstract && TheType.IsSubclassOf(MyType)).ToArray();
        }

        /// <summary>
        ///  This method is used to recursively populate the search window.
        /// </summary>
        /// <param name="searchTree">The SearchTree to populate.</param>
        /// <param name="entry">The Type to use as the Entry point.</param>
        /// <param name="depth">The current recursion depth.</param>
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
        /// <summary>
        ///  This method is used to create a search window.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            // Split inherited classes into lists based on their namespace
            Type[] queries = GetInheritedClasses(typeof(Query));

            var parsedTree = NamespaceParser.TreeFromTypes(queries);

            var tree = new List<SearchTreeEntry>
            {
                new SearchTreeGroupEntry(new GUIContent("New Query"), 0),
            };
            
            PopulateSearchTreeRecursively(tree, parsedTree, 0);
            
            return tree;
        }
        /// <summary>
        ///  This method is called when an entry is selected in the search window.
        /// </summary>
        /// <param name="entry"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public bool OnSelectEntry(SearchTreeEntry entry, SearchWindowContext context)
        {
            
            graphView.AddElement(graphView.GenerateQueryNode(((Type)entry.userData).Name, (Query)ScriptableObject.CreateInstance(((Type)entry.userData)), context.screenMousePosition));
        
            return true;
        }
    }
}
