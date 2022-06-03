using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeAI;
using UnityEngine.AI;
using System.Linq;
public class EncircleTarget : NodeAI.ActionBase
{
    NavMeshAgent navAgent;
    bool initialized = false;
    Vector3 velocity = Vector3.zero;
    float timeInitialized = 0;
    public EncircleTarget()
    {
        AddProperty<GameObject>("Target", null);
        AddProperty<float>("Encircle radius", 1);
        AddProperty<float>("Encircle speed", 1);
        AddProperty<float>("Ally avoidance radius", 1);
    }

    public override void OnInit()
    {
        initialized = false;
        timeInitialized = Time.time;
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
        
        velocity += steeringVector.normalized * GetProperty<float>("Encircle speed");
        velocity = Vector3.ClampMagnitude(velocity, GetProperty<float>("Encircle speed"));
        //navAgent.SetDestination(agent.transform.position + (velocity.normalized * 3.0f));
        //Rotate towards velocity smoothly
        agent.transform.rotation = Quaternion.Slerp(agent.transform.rotation, Quaternion.LookRotation(velocity), Time.deltaTime * 1.0f);
        //Move forward
        agent.transform.position += agent.transform.forward * Time.deltaTime * GetProperty<float>("Encircle speed");
        

        if(Vector3.Distance(agent.transform.position, targetPosition) <= GetProperty<float>("Encircle radius") &&
            Vector3.Distance(agent.transform.position, targetPosition) > GetProperty<float>("Encircle radius") - 1.0f &&
            Time.time - timeInitialized > 0.1f)
        {
            agent.GetComponent<RotateTowardsPlayer>().RotateToPlayer(1.0f, 2.0f, 0.1f);
            //navAgent.SetDestination(agent.transform.position);
            //navAgent.isStopped = true;
            state = NodeData.State.Success;
            return NodeData.State.Success;
        }
        else
        {
            // navAgent.isStopped = false;
            // navAgent.stoppingDistance = 0.1f;
            // navAgent.speed = GetProperty<float>("Encircle speed");
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
                Vector3 allyToAgent = agentPosition - allyPosition;
                if (allyToAgent.magnitude < allyAvoidanceRadius)
                {
                    steeringVector += allyToAgent.normalized * allyAvoidanceRadius;
                }
            }
        }
        //Avoid player
        Vector3 playerToAgent = agentPosition - playerPosition;
        if (playerToAgent.magnitude < playerAvoidanceRadius)
        {
            steeringVector += (playerToAgent * (playerAvoidanceRadius - playerToAgent.magnitude));
        }
        //Move to destination
        steeringVector += (destination - agentPosition);
        return steeringVector.normalized * GetProperty<float>("Encircle speed");
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
        Gizmos.color = Color.red;
        Gizmos.DrawRay(agent.transform.position, velocity);
    }
            

    
}
