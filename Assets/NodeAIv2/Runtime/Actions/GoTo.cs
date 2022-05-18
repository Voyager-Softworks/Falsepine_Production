using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace NodeAI
{
    public class GoTo : ActionBase
    {
        NavMeshAgent navAgent;
        public GoTo()
        {
            AddProperty<Transform>("Position", null);
            AddProperty<bool>("Interrupt", false);
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
            if(navAgent.isOnNavMesh)
            {
                navAgent.SetDestination(GetProperty<Transform>("Position").position);
                if(Vector3.Distance(agent.transform.position, GetProperty<Transform>("Position").position) <= navAgent.stoppingDistance + 1.0f)
                {
                    navAgent.isStopped = true;
                    state = NodeData.State.Success;
                    return NodeData.State.Success;
                }
                if(navAgent.SetDestination(GetProperty<Transform>("Position").position))
                {
                    navAgent.isStopped = false;
                    navAgent.speed = 3.5f;
                    navAgent.angularSpeed = 120;
                    navAgent.acceleration = 15;
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
                Debug.LogError("NavMeshAgent is not on NavMesh");
                state = NodeData.State.Failure;
                return NodeData.State.Failure;
            }
        }
    }
}
