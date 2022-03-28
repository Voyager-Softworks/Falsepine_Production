using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace NodeAI
{
[CustomEditor(typeof(NodeAI_Agent))]
public class NodeAI_AgentEditor : Editor
{
    NodeAI_Agent agent;
    SerializedObject serializedAgent;
    AIController controller;

    bool showParameters = false;

    GUIStyle textStyle;

    // Use this for initialization
    void OnEnable()
    {
        agent = (NodeAI_Agent)target;
        controller = agent.controller;
        serializedAgent = new SerializedObject(agent);
        if(controller != null)
        {
            controller.parameters.Clear();
            foreach (Node n in controller.nodes)
            {
                if(n.type == Node.NodeType.Parameter)
                {
                    controller.parameters.Add(n.parameter);
                }
            }
        }
        textStyle = new GUIStyle();
        textStyle.normal.textColor = Color.white;
        textStyle.fontSize = 20;
        textStyle.alignment = TextAnchor.MiddleCenter;
        textStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1.png") as Texture2D;
        
        textStyle.border = new RectOffset(30, 30, 30, 30);
        
        
    }

    //OnInspectorGUI
    //Description:
    //This function is called when the inspector is drawn.
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI(); //Draws the default inspector.
        agent.NodeAIController = EditorGUILayout.ObjectField(agent.NodeAIController, typeof(AIController), true) as AIController;
        switch(agent.currentState)
        {
            case Node.StateType.Flee:
                EditorGUILayout.LabelField("Current State: Flee");
                break;
            case Node.StateType.Idle:
                EditorGUILayout.LabelField("Current State: Idle");
                break;
            case Node.StateType.Seek:
                EditorGUILayout.LabelField("Current State: Seek");
                break;
            case Node.StateType.Wander:
                EditorGUILayout.LabelField("Current State: Wander");
                break;
        }
        
        if(controller != null)
        {   

            //Display every parameter in a dropdown menu
            showParameters = EditorGUILayout.BeginFoldoutHeaderGroup(showParameters, "Parameters");
            if(showParameters && controller.parameters != null)
            {
                foreach (AIController.Parameter parameter in controller.parameters)
            {
                EditorGUILayout.LabelField("Name: \"" + parameter.name + "\"");
                switch(parameter.type)
                {
                    case AIController.Parameter.ParameterType.Bool:
                        parameter.bvalue = EditorGUILayout.Toggle(parameter.bvalue);
                        break;
                    case AIController.Parameter.ParameterType.Float:
                        parameter.fvalue = EditorGUILayout.FloatField(parameter.fvalue);
                        break;
                    case AIController.Parameter.ParameterType.Int:
                        parameter.ivalue = EditorGUILayout.IntField(parameter.ivalue);
                        break;
                }
                
            }
            }
            
            EditorGUILayout.EndFoldoutHeaderGroup();
        }
    }

    private void OnSceneGUI() {
        
            Handles.color = Color.red;
            
            if(!agent || !agent.agent) return;
            Handles.Label(agent.transform.position - Vector3.up, "State: " + agent.currentState.ToString(), textStyle);
            Handles.DrawSolidDisc(agent.transform.position, Vector3.up, agent.agent.radius);
            Handles.DrawLine(agent.transform.position, agent.transform.position + agent.agent.velocity);
            Handles.DrawLine(agent.transform.position, agent.transform.position +  (Vector3.up * agent.agent.height));
            
            Handles.SphereHandleCap(0, agent.transform.position +  (Vector3.up * agent.agent.height), Quaternion.identity, agent.agent.radius, EventType.Repaint);
            
            
            
    }
}
}
