/*
 * Bachelor of Software Engineering
 * Media Design School
 * Auckland
 * New Zealand
 * 
 * (c) 2022 Media Design School
 * 
 * File Name: SeekClosestWithTag.cs
 * Description: 
 * Author: Nerys Thamm
 * Mail: nerysthamm@gmail.com
 */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace NodeAI
{
    /// <summary>
    /// A Node that seeks the closest object with a certain tag.
    /// </summary>
    public class SeekClosestWithTag : ActionBase
    {
        NavMeshAgent navAgent;
        public SeekClosestWithTag()
        {
            AddProperty<string>("Tag", "");
            AddProperty<float>("Range", 100f);
        }
        public override NodeData.State Eval(NodeAI_Agent agent, NodeTree.Leaf current)
        {
            if(navAgent == null)
            {
                navAgent = agent.GetComponent<NavMeshAgent>();
                if(navAgent == null)
                {
                    Debug.LogError("SeekClosestWithTag: NavMeshAgent not found on agent");
                    return NodeData.State.Failure;
                }
            }
            var tag = GetProperty<string>("Tag");
            var range = GetProperty<float>("Range");

            var targets = GameObject.FindGameObjectsWithTag(tag);
            var closest = targets[0];

            foreach (var target in targets)
            {
                var distance = Vector3.Distance(agent.transform.position, target.transform.position);
                if (distance < Vector3.Distance(agent.transform.position, closest.transform.position))
                {
                    closest = target;
                }
            }
            if(Vector3.Distance(agent.transform.position, closest.transform.position) < range)
            {
                navAgent.SetDestination(closest.transform.position);
                if(navAgent.stoppingDistance > Vector3.Distance(agent.transform.position, closest.transform.position))
                {
                    navAgent.isStopped = true;
                    return NodeData.State.Success;
                }
                else
                {
                    navAgent.isStopped = false;
                    return NodeData.State.Running;
                }
                
            }
            return NodeData.State.Failure;
        }

        public override void DrawGizmos(NodeAI_Agent agent)
        {
            if(state != NodeData.State.Running)
            {
                return;
            }
            var tag = GetProperty<string>("Tag");
            var range = GetProperty<float>("Range");
            var targets = GameObject.FindGameObjectsWithTag(tag);
            foreach (var target in targets)
            {
                var distance = Vector3.Distance(agent.transform.position, target.transform.position);
                if (distance < range)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawLine(agent.transform.position, target.transform.position);
                    Gizmos.DrawIcon(target.transform.position, "Download-Available", true, Color.green);
                }
            }
        }
    }
}