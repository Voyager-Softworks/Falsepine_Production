/*
 * Bachelor of Software Engineering
 * Media Design School
 * Auckland
 * New Zealand
 * 
 * (c) 2022 Media Design School
 * 
 * File Name: NodeAI_Agent_Editor.cs
 * Description: Editor script for the NodeAI_Agent.
 * Author: Nerys Thamm
 * Mail: nerysthamm@gmail.com
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace NodeAI
{
    /// <summary>
    ///  Custom Editor for the NodeAI_Agent.
    /// </summary>
    [CustomEditor(typeof(NodeAI_Agent))]
    public class NodeAI_Agent_Editor : Editor
    {
        NodeAI_Agent agent; ///< Reference to the agent.
        SerializedObject serializedAgent; ///< Reference to the serialized agent.
        SerializedProperty behaviour; ///< Reference to the behaviour property.
        bool paramFoldOut = true; ///< Whether the parameters foldout is open.
        /// <summary>
        ///  Called when the inspector is created.
        /// </summary>
        public void OnEnable()
        {
            agent = (NodeAI_Agent)target;
            serializedAgent = new SerializedObject(agent);
            behaviour = serializedAgent.FindProperty("AI_Behaviour");
            if(agent.AI_Behaviour != null)
            {
                if(agent.inspectorProperties == null) agent.inspectorProperties = new List<NodeData.SerializableProperty>();
                foreach(NodeData.SerializableProperty p in agent.AI_Behaviour.exposedProperties)
                {
                    if(!agent.inspectorProperties.Any(x => x.GUID == p.GUID))
                    {
                        agent.inspectorProperties.Add(p);
                    }
                }
                foreach(NodeData.SerializableProperty p in agent.inspectorProperties)
                {
                    if(!agent.AI_Behaviour.exposedProperties.Any(x => x.GUID == p.GUID))
                    {
                        agent.inspectorProperties.Remove(p);
                    }
                }
            }
        }
        /// <summary>
        ///  Called when the inspector is drawn.
        /// </summary>
        public override void OnInspectorGUI()
        {
            
            
            EditorGUILayout.PropertyField(behaviour);
            serializedAgent.ApplyModifiedProperties();
            //Field for editing faction
            agent.faction = EditorGUILayout.TextField("Faction", agent.faction);
            if(agent.behaviour)
            {
                paramFoldOut = EditorGUILayout.BeginFoldoutHeaderGroup(paramFoldOut, "Parameters");
                if(paramFoldOut)
                {
                    EditorGUI.indentLevel++;
                    foreach(var property in agent.behaviour.exposedProperties)
                    {
                        if(property.type == typeof(int))
                        {
                            property.ivalue = EditorGUILayout.IntField(property.name, property.ivalue);
                        }
                        else if(property.type == typeof(float))
                        {
                            property.fvalue = EditorGUILayout.FloatField(property.name, property.fvalue);
                        }
                        else if(property.type == typeof(bool))
                        {
                            property.bvalue = EditorGUILayout.Toggle(property.name, property.bvalue);
                        }
                        else if(property.type == typeof(string))
                        {
                            property.svalue = EditorGUILayout.TextField(property.name, property.svalue);
                        }
                        else if(property.type == typeof(Vector2))
                        {
                            property.v2value = EditorGUILayout.Vector2Field(property.name, property.v2value);
                        }
                        else if(property.type == typeof(Vector3))
                        {
                            property.v3value = EditorGUILayout.Vector3Field(property.name, property.v3value);
                        }
                        else if(property.type == typeof(Vector4))
                        {
                            property.v4value = EditorGUILayout.Vector4Field(property.name, property.v4value);
                        }
                        else if(property.type == typeof(Color))
                        {
                            property.cvalue = EditorGUILayout.ColorField(property.name, property.cvalue);
                        }
                        else
                        {
                            property.ovalue = EditorGUILayout.ObjectField(property.name, property.ovalue, property.type, true);
                        }
                    }
                    //if a value has changed, propogate the changes to the behaviour
                    if(GUI.changed)
                    {
                        agent.nodeTree.PropogateExposedProperties(agent.behaviour.exposedProperties);
                    }
                    EditorGUI.indentLevel--;
                }
                
            }
            else if(agent.inspectorProperties != null)
            {
                paramFoldOut = EditorGUILayout.BeginFoldoutHeaderGroup(paramFoldOut, "Parameters");
                if(paramFoldOut)
                {
                    EditorGUI.indentLevel++;
                    foreach(var property in agent.inspectorProperties)
                    {
                        if(property.type == typeof(int))
                        {
                            property.ivalue = EditorGUILayout.IntField(property.name, property.ivalue);
                        }
                        else if(property.type == typeof(float))
                        {
                            property.fvalue = EditorGUILayout.FloatField(property.name, property.fvalue);
                        }
                        else if(property.type == typeof(bool))
                        {
                            property.bvalue = EditorGUILayout.Toggle(property.name, property.bvalue);
                        }
                        else if(property.type == typeof(string))
                        {
                            property.svalue = EditorGUILayout.TextField(property.name, property.svalue);
                        }
                        else if(property.type == typeof(Vector2))
                        {
                            property.v2value = EditorGUILayout.Vector2Field(property.name, property.v2value);
                        }
                        else if(property.type == typeof(Vector3))
                        {
                            property.v3value = EditorGUILayout.Vector3Field(property.name, property.v3value);
                        }
                        else if(property.type == typeof(Vector4))
                        {
                            property.v4value = EditorGUILayout.Vector4Field(property.name, property.v4value);
                        }
                        else if(property.type == typeof(Color))
                        {
                            property.cvalue = EditorGUILayout.ColorField(property.name, property.cvalue);
                        }
                        else
                        {
                            property.ovalue = EditorGUILayout.ObjectField(property.name, property.ovalue, property.type, true);
                        }
                    }
                    EditorGUI.indentLevel--;
                    if(GUI.changed) EditorUtility.SetDirty(agent);
                }
            }
            else if(agent.AI_Behaviour != null)
            {
                agent.inspectorProperties = new List<NodeData.SerializableProperty>();
                agent.AI_Behaviour.exposedProperties.ForEach(property => agent.inspectorProperties.Add(new NodeData.SerializableProperty(property)));
            }
            
            serializedAgent.ApplyModifiedProperties();
        }
    }
}
