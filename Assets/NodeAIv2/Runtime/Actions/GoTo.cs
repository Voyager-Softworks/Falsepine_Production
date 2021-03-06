/*
 * Bachelor of Software Engineering
 * Media Design School
 * Auckland
 * New Zealand
 * 
 * (c) 2022 Media Design School
 * 
 * File Name: GoTo.cs
 * Description: 
 * Author: Nerys Thamm
 * Mail: nerysthamm@gmail.com
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace NodeAI
{
    /// <summary>
    /// A node which moves the agent to a given position.
    /// </summary>
    public class GoTo : ActionBase
    {
        NavMeshAgent navAgent;
        public GoTo()
        {
            AddProperty<Transform>("Position", null);
            AddProperty<float>("Stopping distance", 0.5f);
            AddProperty<float>("Speed", 1);
            AddProperty<float>("Acceleration", 15);
            AddProperty<bool>("Interrupt", false);
        }
        public override void OnInit()
        {
            SetProperty<bool>("Interrupt", false);
        }
        public override NodeData.State Eval(NodeAI_Agent agent, NodeTree.Leaf current)
        {
            if(navAgent == null)
            {
                navAgent = agent.GetComponent<NavMeshAgent>();
                if(navAgent == null)
                {
                    Debug.LogError("No NavMeshAgent found on " + agent.gameObject.name);
                    state = NodeData.State.Failure;
                    return NodeData.State.Failure;
                }
            }
            if(GetProperty<bool>("Interrupt"))
            {
                navAgent.isStopped = true;
                state = NodeData.State.Failure;
                return NodeData.State.Failure;
            }
            if(navAgent.isOnNavMesh && GetProperty<Transform>("Position"))
            {
                if(GetProperty<float>("Stopping distance") > 0)
                {
                    navAgent.stoppingDistance = GetProperty<float>("Stopping distance");
                }
                //navAgent.SetDestination(GetProperty<Transform>("Position").position);
                if(Vector3.Distance(agent.transform.position, GetProperty<Transform>("Position").position) <= navAgent.stoppingDistance + 0.1f)
                {
                    navAgent.isStopped = true;
                    state = NodeData.State.Success;
                    return NodeData.State.Success;
                }
                if(navAgent.SetDestination(GetProperty<Transform>("Position").position))
                {
                    navAgent.isStopped = false;
                    navAgent.speed = GetProperty<float>("Speed");
                    navAgent.acceleration = GetProperty<float>("Acceleration");
                    if(GetProperty<float>("Stopping distance") > 0)
                    {
                        navAgent.stoppingDistance = GetProperty<float>("Stopping distance");
                    }
                    state = NodeData.State.Running;
                    return NodeData.State.Running;
                }
                else
                {
                    navAgent.isStopped = true;
                    state = NodeData.State.Failure;
                    return NodeData.State.Failure;
                }
            }
            else
            {
                state = NodeData.State.Failure;
                return NodeData.State.Failure;
            }
        }
    }
}
