using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeAI;
using UnityEngine.AI;

/// <summary>
/// The first boss in the game.
/// </summary>
namespace Boss.Brightmaw
{
    /// <summary>
    ///  A node for the Brightmaw bosses boulder throw attack.
    /// </summary>
    public class ThrowBoulder : NodeAI.ActionBase
    {
        GameObject[] boulders;
        GameObject closest;
        bool initialised = false;
        bool reachedBoulder = false;
        RotateTowards rotateTowards;
        NavMeshAgent navAgent;
        public ThrowBoulder()
        {
            AddProperty<Transform>("Target", null);
            AddProperty<string>("Boulder Tag", "");
            AddProperty<float>("Range", 100f);
        }
        public override NodeData.State Eval(NodeAI_Agent agent, NodeTree.Leaf current)
        {
            if (!initialised)
            {
                rotateTowards = agent.GetComponent<RotateTowards>();
                navAgent = agent.GetComponent<NavMeshAgent>();
                if(navAgent == null)
                {
                    Debug.LogError("ThrowBoulder: NavMeshAgent not found on agent");
                    state = NodeData.State.Failure;
                    return NodeData.State.Failure;
                }
                if (rotateTowards == null)
                {
                    Debug.LogError("ThrowBoulder: RotateTowards not found on agent");
                    state = NodeData.State.Failure;
                    return NodeData.State.Failure;
                }
                boulders = GameObject.FindGameObjectsWithTag(GetProperty<string>("Boulder Tag"));
                initialised = true;
            
                if (boulders.Length <= 0)
                {
                    state = NodeData.State.Failure;
                    return NodeData.State.Failure;
                }
                if (closest == null)
                {
                    closest = boulders[0];
                }
                foreach (var boulder in boulders)
                {
                    var distance = Vector3.Distance(agent.transform.position, boulder.transform.position);
                    if (distance < Vector3.Distance(agent.transform.position, closest.transform.position))
                    {
                        closest = boulder;
                    }
                }
            }
            if (Vector3.Distance(agent.transform.position, closest.transform.position) > GetProperty<float>("Range"))
            {
                navAgent.SetDestination(
                    closest.transform.position + 
                    ((closest.transform.position - GetProperty<Transform>("Target").position).normalized 
                    * GetProperty<float>("Range") 
                    * 0.9f)
                    );  
                state = NodeData.State.Running;
                return NodeData.State.Running;
            }
            else
            {
                if(!reachedBoulder)
                {
                    rotateTowards.RotateToObject(closest, 0.3f, 5.0f, 0.0f);
                    reachedBoulder = true;
                }
                state = NodeData.State.Success;
                return NodeData.State.Success;
            }
        }

        public override void OnInit()
        {
            initialised = false;
            reachedBoulder = false;
        }
    }
}

