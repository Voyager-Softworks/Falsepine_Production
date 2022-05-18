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
            if(navAgent.isOnNavMesh)
            {
                navAgent.SetDestination(GetProperty<Transform>("Position").position);
                if(navAgent.remainingDistance <= navAgent.stoppingDistance)
                {
                    navAgent.isStopped = true;
                    state = NodeData.State.Success;
                    return NodeData.State.Success;
                }
                if(navAgent.SetDestination(GetProperty<Transform>("Position").position))
                {
                    navAgent.isStopped = false;
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
