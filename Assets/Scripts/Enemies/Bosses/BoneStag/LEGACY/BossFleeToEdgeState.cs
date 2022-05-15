using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeAI;
#if false // Old code, not used anymore but kept for reference
[CreateAssetMenu(fileName = "New Custom State", menuName = "NodeAI/Custom State/BossFleeToEdgeState")]
public class BossFleeToEdgeState : NodeAI.CustomState
{
    Transform agentTransform;
    GameObject agentGameObject;
    BossArenaController arenaController;

    Vector3 arenaEdgeGoalPosition;
    public override void DoCustomState(NodeAI_Agent agent)
    {
        Debug.Log("DoCustomState");
        
        if(agent.agent.remainingDistance < 0.1f && !agent.agent.isStopped)
        {
            agent.SetBool("Arrived", true);
            agent.agent.isStopped = true;
            agent.agent.velocity = Vector3.zero;
            agent.agent.SetDestination(GameObject.FindGameObjectWithTag("Player").transform.position);
        }
    }

    public override void OnStateEnter(NodeAI_Agent agent)
    {
        Debug.Log("OnStateEnter");
        agent.SetBool("Arrived", false);
        agentTransform = agent.transform;
        agentGameObject = agent.gameObject;
        
        if(arenaController == null)
            arenaController = FindObjectOfType<BossArenaController>();
        Vector3 playerPos = GameObject.FindGameObjectWithTag("Player").transform.position;
        arenaEdgeGoalPosition = (-(playerPos - agentTransform.position).normalized * arenaController.arenaRadius) + arenaController.arenaCentre.position;
        agent.agent.SetDestination(arenaEdgeGoalPosition);
        agent.agent.speed = 100;
        
        
        agent.agent.acceleration = 150;
        agent.agent.angularSpeed = 100;
        
        agent.agent.isStopped = false;
        
    }

    public override void OnStateExit(NodeAI_Agent agent)
    {
        Debug.Log("OnStateExit");
    }

    public override void DrawStateGizmos(NodeAI_Agent agent)
    {
        Debug.Log("DrawStateGizmos");
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(agent.agent.destination, 1);
        Gizmos.DrawWireSphere(arenaController.arenaCentre.position, arenaController.arenaRadius);
    }

    private Vector3 ComputeB( Vector3 circleCenter, Vector3 circleNormal, float circleRadius, Vector3 point, Vector3 direction )
    {
        float a = Vector3.SignedAngle( (circleCenter - point).normalized * circleRadius, direction, circleNormal );
        float w = 0;
        if ( a >= 0 ) w = 180 - 2 * a; // because w + a + a = 180;
        else w = -( 180 + 2 * a );
        Vector3 BO = Quaternion.AngleAxis(w, -circleNormal) * ((point - circleCenter).normalized * circleRadius);
        return circleCenter + BO;
    }
}
#endif