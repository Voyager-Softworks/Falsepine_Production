using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace NodeAI
{
    [CustomEditor(typeof(NodeAI_Agent))]
    public class NodeAI_Agent_Editor : Editor
    {
        NodeAI_Agent agent;
        bool paramFoldOut = true;
        public void OnEnable()
        {
            agent = (NodeAI_Agent)target;
        }
        public override void OnInspectorGUI()
        {
            
            agent.AI_Behaviour = (NodeAI_Behaviour)EditorGUILayout.ObjectField("Behaviour", agent.AI_Behaviour, typeof(NodeAI_Behaviour), false);
            
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
                    }
                    //if a value has changed, propogate the changes to the behaviour
                    if(GUI.changed)
                    {
                        agent.nodeTree.PropogateExposedProperties(agent.behaviour.exposedProperties);
                    }
                    EditorGUI.indentLevel--;
                }
                
            }
        }
    }
}
