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
            // string[] assemblies = {"UnityEngine.AudioModule", "UnityEngine.CoreModule", "UnityEngine.AnimationModule" };
            // var classes = System.AppDomain.CurrentDomain.GetAssemblies().Where(assembly => assemblies.Contains(assembly.GetName().Name))
            //             .SelectMany(assembly => assembly.GetTypes())
            //             .Where(type => type.IsClass && type.IsSubclassOf(typeof(MonoBehaviour)));

            // // Create a list of all namespaces
            // var namespaces = new List<string>();
            
            // foreach (var c in classes)
            // {
            //     if (!namespaces.Contains(c.Namespace))
            //     {
            //         namespaces.Add(c.Namespace);
            //     }
            // }

            Type[] supportedTypes = {typeof(GameObject), typeof(AudioClip), typeof(Transform), typeof(Animator)};


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
            
            // foreach (var n in namespaces)
            // {
            //     tree.Add(new SearchTreeGroupEntry(new GUIContent(n), 1));
            //     foreach (var c in classes)
            //     {
            //         if (c.Namespace == n)
            //         {
            //             tree.Add(new SearchTreeEntry(new GUIContent(c.Name)){userData = c, level = 2});
            //         }
            //     }
            // }

            foreach (var t in supportedTypes)
            {
                tree.Add(new SearchTreeEntry(new GUIContent(t.Name)){userData = t, level = 1});
            }
            return tree;
        }

        public bool OnSelectEntry(SearchTreeEntry entry, SearchWindowContext context)
        {
            switch(((Type)entry.userData).Name)
                    {
                        case "Int32":
                            graphView.AddPropertyToBlackboard(new NodeData.Property<int>{ name = "New Property", value = 0 });
                            break;
                        case "Single":
                            graphView.AddPropertyToBlackboard(new NodeData.Property<float>{ name = "New Property", value = 0.0f });
                            break;
                        case "Boolean":
                            graphView.AddPropertyToBlackboard(new NodeData.Property<bool>{ name = "New Property", value = false });
                            break;
                        case "String":
                            graphView.AddPropertyToBlackboard(new NodeData.Property<String>{ name = "New Property", value = "" });
                            break;
                        case "Vector2":
                            graphView.AddPropertyToBlackboard(new NodeData.Property<Vector2>{ name = "New Property", value = Vector2.zero });
                            break;
                        case "Vector3":
                            graphView.AddPropertyToBlackboard(new NodeData.Property<Vector3>{ name = "New Property", value = Vector3.zero });
                            break;
                        case "Vector4":
                            graphView.AddPropertyToBlackboard(new NodeData.Property<Vector4>{ name = "New Property", value = Vector4.zero });
                            break;
                        case "Color":
                            graphView.AddPropertyToBlackboard(new NodeData.Property<Color>{ name = "New Property", value = Color.white });
                            break;
                        default:
                            graphView.AddPropertyToBlackboard(new NodeData.Property { name = "New Property", value = null, type = (Type)entry.userData });
                            break;
                    }
            
            return true;
        }
    }
}
