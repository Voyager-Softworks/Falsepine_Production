using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeAI;

[CreateAssetMenu(fileName = "New Custom State", menuName = "NodeAI/Custom State/BossChargingState")]
public class BossChargingState : CustomState
{
    Transform agentTransform;
    GameObject agentGameObject;
    BossArenaController arenaController;

    Vector3 arenaEdgeGoalPosition;
    public override void DoCustomState(NodeAI_Agent agent)
    {
        //Debug.Log("DoCustomState");
        
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
        //Debug.Log("OnStateEnter");
        agent.SetBool("Arrived", false);
        agentTransform = agent.transform;
        agentGameObject = agent.gameObject;
        
        if(arenaController == null)
            arenaController = FindObjectOfType<BossArenaController>();
        Vector3 playerPos = GameObject.FindGameObjectWithTag("Player").transform.position;
        arenaEdgeGoalPosition = ComputeB(arenaController.arenaCentre.position, Vector3.up, arenaController.arenaRadius, agent.transform.position, (playerPos - agentTransform.position).normalized);
        agent.agent.SetDestination(arenaEdgeGoalPosition);
        agent.agent.speed = 100;
        agent.agent.velocity =  (arenaEdgeGoalPosition - agent.transform.position).normalized * agent.agent.speed;
        
        agent.agent.acceleration = 150;
        agent.agent.angularSpeed = 100000;
        
        agent.agent.isStopped = false;
        
    }

    public override void OnStateExit(NodeAI_Agent agent)
    {
        //Debug.Log("OnStateExit");
    }

    public override void DrawStateGizmos(NodeAI_Agent agent)
    {
        //Debug.Log("DrawStateGizmos");
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
