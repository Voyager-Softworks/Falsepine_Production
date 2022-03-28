using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeAI;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "New Custom State", menuName = "NodeAI/Custom State/BossAttackingState")]
public class BossAttackPlayer : NodeAI.CustomState
{

    bool attacking = false;
    public float attackDistance;
    public float agentSpeed;
    public float agentTurnSpeed;
    public float attackDuration;
    public int attackAmount;

    int currAttackAmount = 0;
    float attackTimer;
    Transform playerTransform;
    public override void OnStateEnter(NodeAI_Agent agent)
    {
        attacking = false;
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        agent.agent.speed = agentSpeed;
        currAttackAmount = 0;
        attackTimer = 0;
        agent.agent.isStopped = false;
        agent.agent.angularSpeed = agentTurnSpeed;
        agent.SetBool("AttackFinished", false);
    }

    public override void OnStateExit(NodeAI_Agent agent)
    {
        
    }

    public override void DoCustomState(NodeAI_Agent agent)
    {
        if(!attacking) 
        {
            
            agent.agent.SetDestination( playerTransform.position);
            if(Vector3.Distance(agent.transform.position, playerTransform.position) < attackDistance && currAttackAmount < attackAmount)
            {
                attacking = true;
                agent.agent.isStopped = true;
                agent.agent.velocity = Vector3.zero;
                
                attackTimer = attackDuration;
            }
            
        }
        else
        {
            attackTimer -= Time.deltaTime;
            if(attackTimer <= 0)
            {
                attacking = false;
                agent.agent.isStopped = false;
                currAttackAmount++;
                if(currAttackAmount >= attackAmount)
                {
                    agent.SetBool("AttackFinished", true);
                }
            }
        }
    }

    public override void DrawStateGizmos(NodeAI_Agent agent)
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(agent.transform.position, attackDistance);
        Gizmos.DrawLine(agent.transform.position, playerTransform.position);
    }
}
