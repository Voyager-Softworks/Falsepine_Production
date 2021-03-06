using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeAI;
using UnityEngine.AI;
using System.Linq;
public class EncircleTarget : NodeAI.ActionBase  /// @todo Comment
{
    NavMeshAgent navAgent;
    
    public EncircleTarget()
    {
        AddProperty<GameObject>("Target", null);
        AddProperty<float>("Encircle radius", 1);
        AddProperty<float>("Encircle speed", 1);
        AddProperty<float>("Ally avoidance radius", 1);
    }

    public override void OnInit()
    {
        
    }

    public override NodeData.State Eval(NodeAI_Agent agent, NodeTree.Leaf current)
    {
        if (GetProperty<GameObject>("Target") == null)
        {
            state = NodeData.State.Failure;
            return NodeData.State.Failure;
        }
        if (navAgent == null)
        {
            navAgent = agent.GetComponentInChildren<NavMeshAgent>();
            if (navAgent == null)
            {
                Debug.LogError("No NavMeshAgent found on " + agent.gameObject.name);
                state = NodeData.State.Failure;
                return NodeData.State.Failure;
            }
        }
        // Encircle the target while avoiding allies
        Vector3 targetPosition = GetProperty<GameObject>("Target").transform.position;
        
        
        Collider[] colliders = Physics.OverlapSphere(agent.transform.position, GetProperty<float>("Ally avoidance radius"));
        List<Collider> allies = colliders.Where(c => c.gameObject.GetComponent<NodeAI_Agent>() != null && c.gameObject.GetComponent<NodeAI_Agent>().faction == agent.faction).ToList();
        Vector3 steeringVector = CalculateSteeringVector(
                                                        targetPosition, 
                                                        allies.ToArray(), GetProperty<float>("Ally avoidance radius"), 
                                                        GetProperty<float>("Encircle radius"), 
                                                        targetPosition, 
                                                        agent.transform.position
                                                        );
        
        if(steeringVector.magnitude > 0.01f)
        {
            NavMeshHit nhit;
            NavMesh.SamplePosition(agent.transform.position + (steeringVector.normalized * GetProperty<float>("Encircle speed")), out nhit, 3.0f, NavMesh.AllAreas);
            navAgent.SetDestination(nhit.position);
        }
        else
        {
            agent.GetComponent<RotateTowardsPlayer>().RotateToPlayer(1.0f, 2.0f, 0.1f);
            navAgent.SetDestination(agent.transform.position);
            navAgent.isStopped = true;
            navAgent.velocity = Vector3.zero;
            state = NodeData.State.Success;
            return NodeData.State.Success;
        }

        

        if(Vector3.Distance(agent.transform.position, targetPosition) <= GetProperty<float>("Encircle radius") &&
            Vector3.Distance(agent.transform.position, targetPosition) > GetProperty<float>("Encircle radius") - 2.0f)
        {
            agent.GetComponent<RotateTowardsPlayer>().RotateToPlayer(1.0f, 2.0f, 0.1f);
            navAgent.SetDestination(agent.transform.position);
            navAgent.isStopped = true;
            
            state = NodeData.State.Success;
            return NodeData.State.Success;
        }
        else
        {
            navAgent.isStopped = false;
            navAgent.stoppingDistance = 0.1f;
            state = NodeData.State.Running;
            return NodeData.State.Running;
        }
        
        
    }

    //Calculate Steering Vector to navigate to target position while avoiding allies and the player
    Vector3 CalculateSteeringVector(Vector3 destination, Collider[] allies, float allyAvoidanceRadius, float playerAvoidanceRadius, Vector3 playerPosition, Vector3 agentPosition)
    {
        Vector3 steeringVector = Vector3.zero;
        //Avoid allies
        foreach (Collider ally in allies)
        {
            if (ally.gameObject.GetComponent<NodeAI_Agent>() != null)
            {
                Vector3 allyPosition = ally.gameObject.transform.position;
                Vector3 allyToAgent = (agentPosition + (navAgent.gameObject.transform.forward * GetProperty<float>("Encircle speed"))) - allyPosition;
                if (allyToAgent.magnitude < allyAvoidanceRadius)
                {
                    steeringVector += allyToAgent.normalized * allyAvoidanceRadius;
                }
            }
        }
        //Avoid player
        Vector3 playerToAgent = (agentPosition + (navAgent.gameObject.transform.forward * GetProperty<float>("Encircle speed"))) - playerPosition;
        if (playerToAgent.magnitude < (playerAvoidanceRadius * 2.0f))
        {
            steeringVector += playerToAgent.normalized * playerAvoidanceRadius;
        }
        //Move to destination
        steeringVector += (destination - agentPosition);
        return steeringVector;
    }

    Vector3 GetLeastAllyDensePosition(Vector3 targetPosition, NodeAI_Agent agent)
    {
        Collider[] colliders = Physics.OverlapSphere(agent.transform.position, GetProperty<float>("Ally avoidance radius"));
        List<NodeAI_Agent> allies = colliders.Where(c => c.gameObject.GetComponent<NodeAI_Agent>() != null && c.gameObject.GetComponent<NodeAI_Agent>().faction == agent.faction).Select(c => c.gameObject.GetComponent<NodeAI_Agent>()).ToList();
        if (allies.Count == 0)
        {
            return (agent.transform.position - targetPosition).normalized * GetProperty<float>("Encircle radius") + targetPosition;
        }
        else
        {
            Vector3 directionSum = Vector3.zero;
            foreach (NodeAI_Agent ally in allies)
            {
                directionSum += (targetPosition - ally.transform.position).normalized;
            }
            Vector3 averageDirection = directionSum / allies.Count;
            Vector3 freePosition = (averageDirection.normalized * GetProperty<float>("Encircle radius")) + targetPosition;

            return freePosition;
        }
    }

    public override void DrawGizmos(NodeAI_Agent agent)
    {
        if (GetProperty<GameObject>("Target") == null)
        {
            return;
        }
        
    }
            

    
}
