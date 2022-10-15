/*
 * Bachelor of Software Engineering
 * Media Design School
 * Auckland
 * New Zealand
 * 
 * (c) 2022 Media Design School
 * 
 * File Name: BlackboardSearchWindow.cs
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
    public class BlackboardSearchWindow : ScriptableObject, ISearchWindowProvider
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
        /// This method is used to create a search window.
        /// </summary>
        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {

            Type[] supportedTypes = { typeof(GameObject), typeof(AudioClip), typeof(Transform), typeof(Animator) };

            supportedTypes = supportedTypes.Concat(NamespaceParser.GetParameterisableTypes()).ToArray();


            var tree = new List<SearchTreeEntry>
            {
                new SearchTreeGroupEntry(new GUIContent("New Parameter"), 0),
                new SearchTreeEntry(new GUIContent("String")){userData = typeof(string), level = 1},
                new SearchTreeEntry(new GUIContent("Int")){userData = typeof(int), level = 1},
                new SearchTreeEntry(new GUIContent("Float")){userData = typeof(float), level = 1},
                new SearchTreeEntry(new GUIContent("Bool")){userData = typeof(bool), level = 1},
                new SearchTreeEntry(new GUIContent("Vector2")){userData = typeof(Vector2), level = 1},
                new SearchTreeEntry(new GUIContent("Vector3")){userData = typeof(Vector3), level = 1},
                new SearchTreeEntry(new GUIContent("Vector4")){userData = typeof(Vector4), level = 1},
                new SearchTreeEntry(new GUIContent("Color")){userData = typeof(Color), level = 1},

            };

            foreach (var t in supportedTypes)
            {
                tree.Add(new SearchTreeEntry(new GUIContent(t.Name)) { userData = t, level = 1 });
            }
            return tree;
        }

        /// <summary>
        ///  This method is called when a selection is made in the search window.
        /// </summary>
        /// <param name="entry">The entry which was selected.</param>
        /// <param name="context">The SearchWindowContext object.</param>
        /// <returns>Whether or not the selection was successful.</returns>
        public bool OnSelectEntry(SearchTreeEntry entry, SearchWindowContext context)
        {
            switch (((Type)entry.userData).Name)
            {
                case "Int32":
                    graphView.AddPropertyToBlackboard(new NodeData.Property<int> { name = "New Property", value = 0 });
                    break;
                case "Single":
                    graphView.AddPropertyToBlackboard(new NodeData.Property<float> { name = "New Property", value = 0.0f });
                    break;
                case "Boolean":
                    graphView.AddPropertyToBlackboard(new NodeData.Property<bool> { name = "New Property", value = false });
                    break;
                case "String":
                    graphView.AddPropertyToBlackboard(new NodeData.Property<String> { name = "New Property", value = "" });
                    break;
                case "Vector2":
                    graphView.AddPropertyToBlackboard(new NodeData.Property<Vector2> { name = "New Property", value = Vector2.zero });
                    break;
                case "Vector3":
                    graphView.AddPropertyToBlackboard(new NodeData.Property<Vector3> { name = "New Property", value = Vector3.zero });
                    break;
                case "Vector4":
                    graphView.AddPropertyToBlackboard(new NodeData.Property<Vector4> { name = "New Property", value = Vector4.zero });
                    break;
                case "Color":
                    graphView.AddPropertyToBlackboard(new NodeData.Property<Color> { name = "New Property", value = Color.white });
                    break;
                default:
                    graphView.AddPropertyToBlackboard(new NodeData.Property { name = "New Property", value = null, type = (Type)entry.userData });
                    break;
            }

            return true;
        }
    }
}
