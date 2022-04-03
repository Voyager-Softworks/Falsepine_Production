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

    public float chargeSpeed = 50f;
    public float chargeDamage = 10f;
    public GameObject debrisPrefab;

    bool hasDamagedPlayer = false;

    public AudioClip chargeSoundPhaseOne, chargeSoundPhaseTwo;
    public override void DoCustomState(NodeAI_Agent agent)
    {
        //Debug.Log("DoCustomState");
        
        if(agent.agent.remainingDistance < 0.1f && !agent.agent.isStopped)
        {
            agent.SetBool("Arrived", true);
            agent.agent.isStopped = true;
            agent.agent.velocity = Vector3.zero;
            agent.agent.SetDestination(GameObject.FindGameObjectWithTag("Player").transform.position);
            hasDamagedPlayer = false;
        }
        else
        {
            RaycastHit[] hits = Physics.SphereCastAll(agent.transform.position, 1.0f, agent.transform.forward, 1.0f);
            foreach(RaycastHit hit in hits)
            {
                if(hit.collider.gameObject.tag == "DestructibleProp")
                {
                    Destroy(Instantiate(debrisPrefab, hit.point, Quaternion.identity), 5.0f);
                    Destroy(hit.collider.gameObject);
                }
                else if(hit.collider.gameObject.tag == "Player" && !hasDamagedPlayer)
                {
                    hit.collider.gameObject.GetComponent<PlayerHealth>().TakeDamage(chargeDamage);
                    hasDamagedPlayer = true;
                }
            }
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
        
        if(agent.GetBool("SecondPhase")) {
            agent.GetComponent<AudioSource>().PlayOneShot(chargeSoundPhaseTwo);
            agent.agent.speed = 50;
        }
        else {
            agent.GetComponent<AudioSource>().PlayOneShot(chargeSoundPhaseOne);
            agent.agent.speed = 30;
        }

        agent.agent.velocity =  (arenaEdgeGoalPosition - agent.transform.position).normalized * agent.agent.speed;
        
        agent.agent.acceleration = 150;
        agent.agent.angularSpeed = 100000;
        
        agent.agent.isStopped = false;
        
    }

    public override void OnStateExit(NodeAI_Agent agent)
    {
        agent.GetComponent<RotateTowardsPlayer>().RotateToPlayer(1.5f, 4.0f, 0.2f);
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
