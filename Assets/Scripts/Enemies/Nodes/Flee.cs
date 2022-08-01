using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeAI;
using UnityEngine.AI;

/// <summary>
///  Node for fleeing from a target.
/// </summary>
public class Flee : NodeAI.ActionBase
{

    bool initialised = false;
    NavMeshAgent navAgent;

    public Flee()
    {
        tooltip = "Flee from a target.";

        
        AddProperty<Transform>("Target", null);
        AddProperty<float>("Speed", 6);
        AddProperty<float>("Distance", 13);
        AddProperty<float>("Acceleration", 15);



    }
    /// <summary>
    ///  Evaluates the node
    /// </summary>
    /// <param name="agent"></param>
    /// <param name="current"></param>
    /// <returns></returns>
    public override NodeData.State Eval(NodeAI_Agent agent, NodeTree.Leaf current)
    {
        if (!initialised)
        {
            initialised = true;
            navAgent = agent.GetComponent<NavMeshAgent>();
            if(navAgent == null)
            {
                Debug.LogError("Flee node requires a NavMeshAgent component.");
                state = NodeData.State.Failure;
                return NodeData.State.Failure;
            }
            navAgent.speed = GetProperty<float>("Speed");
            navAgent.acceleration = GetProperty<float>("Acceleration");
            navAgent.isStopped = false;

            // Set destination to be opposite direction to the target, by the distance.
            Vector3 targetPos = GetProperty<Transform>("Target").position;
            Vector3 destination = targetPos + (targetPos - agent.transform.position).normalized * GetProperty<float>("Distance");
            navAgent.SetDestination(destination);
        }
        if (navAgent.remainingDistance <= navAgent.stoppingDistance)
        {
            navAgent.isStopped = true;
            state = NodeData.State.Success;
            return NodeData.State.Success;
        }
        else
        {
            state = NodeData.State.Running;
            return NodeData.State.Running;
        }
    }
    /// <summary>
    /// Initialises the node.
    /// </summary>
    public override void OnInit()
    {
        initialised = false;
    }
}
