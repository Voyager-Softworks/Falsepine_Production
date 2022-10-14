/*
 * Bachelor of Software Engineering
 * Media Design School
 * Auckland
 * New Zealand
 * 
 * (c) 2022 Media Design School
 * 
 * File Name: NamespaceParser.cs
 * Description: 
 * Author: Nerys Thamm
 * Mail: nerysthamm@gmail.com
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace NodeAI
{
    /// <summary>
    ///  This class is used to parse the Namespaces of types into a tree that can be used to create a Search Window.
    /// </summary>
    public class NamespaceParser
    {
        public class NamespaceEntry
        {
            public string Namespace;
            public List<System.Type> Classes;
            public List<NamespaceEntry> Subnamespaces;
        }

        /// <summary>
        ///  Parses the namespace and returns a tree.
        /// </summary>
        /// <param name="types"></param>
        /// <returns></returns>
        public static NamespaceEntry TreeFromTypes(System.Type[] types)
        {
            var tree = new NamespaceEntry();
            tree.Namespace = "";
            tree.Classes = new List<System.Type>();
            tree.Subnamespaces = new List<NamespaceEntry>();

            foreach (var type in types)
            {

                string[] parts = type.Namespace.Split('.');
                NamespaceEntry current = tree;
                foreach (var part in parts)
                {
                    bool found = false;
                    foreach (var subnamespace in current.Subnamespaces)
                    {
                        if (subnamespace.Namespace == part)
                        {
                            current = subnamespace;
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                    {
                        var newNamespace = new NamespaceEntry();
                        newNamespace.Namespace = part;
                        newNamespace.Classes = new List<System.Type>();
                        newNamespace.Subnamespaces = new List<NamespaceEntry>();
                        current.Subnamespaces.Add(newNamespace);
                        current = newNamespace;
                    }
                }

                //place the type in the correct namespace
                current.Classes.Add(type);
            }



            tree.Subnamespaces.Sort((a, b) => a.Namespace.CompareTo(b.Namespace));
            tree.Classes.Sort((a, b) => a.Name.CompareTo(b.Name));

            return tree;
        }

        public static System.Type[] GetParameterisableTypes()
        {
            var types = System.AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => p.IsClass && p.GetCustomAttributes(typeof(NodeAI.Parameterisable), false).Length > 0)
                .ToArray();

            return types;
        }

    }




}