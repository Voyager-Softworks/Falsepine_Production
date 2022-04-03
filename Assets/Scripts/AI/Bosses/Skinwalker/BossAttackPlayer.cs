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

    public AudioClip AttkC, AttkC1, AttkC2, AttkS, AttkS1, AttkS2;

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
            if(Vector3.Distance(agent.transform.position, playerTransform.position) < attackDistance && currAttackAmount < attackAmount && Vector3.Dot(agent.transform.forward, playerTransform.position - agent.transform.position) > 0)
            {
                int rand = Random.Range(0, 3);
                switch(rand)
                {
                    case 0:
                        if(agent.GetBool("SecondPhase"))
                        {
                            agent.animator.SetTrigger("Attack");
                            agent.gameObject.GetComponent<DamageDealer>().EnableHurtBox("AttkS");
                            agent.gameObject.GetComponent<RotateTowardsPlayer>().RotateToPlayer(0.2f, 15.0f, 1.1f);
                            agent.gameObject.GetComponent<AudioSource>().PlayOneShot(AttkS);
                            attackTimer = agent.gameObject.GetComponent<DamageDealer>().GetAttackDuration("AttkS");
                        }
                        else
                        {
                            agent.animator.SetTrigger("Attack");
                            agent.gameObject.GetComponent<DamageDealer>().EnableHurtBox("AttkC");
                            agent.gameObject.GetComponent<RotateTowardsPlayer>().RotateToPlayer(0.2f, 15.0f, 1.1f);
                            agent.gameObject.GetComponent<AudioSource>().PlayOneShot(AttkC);
                            attackTimer = agent.gameObject.GetComponent<DamageDealer>().GetAttackDuration("AttkC");
                        }
                        break;
                    case 1:
                        if(agent.GetBool("SecondPhase"))
                        {
                            agent.animator.SetTrigger("Combo1");
                            agent.gameObject.GetComponent<DamageDealer>().EnableHurtBox("AttkS1");
                            agent.gameObject.GetComponent<RotateTowardsPlayer>().RotateToPlayer(0.2f, 15.0f, 1.1f);
                            agent.gameObject.GetComponent<AudioSource>().PlayOneShot(AttkS1);
                            attackTimer = agent.gameObject.GetComponent<DamageDealer>().GetAttackDuration("AttkS1");
                        }
                        else
                        {
                            agent.animator.SetTrigger("Combo1");
                            agent.gameObject.GetComponent<DamageDealer>().EnableHurtBox("AttkC1");
                            agent.gameObject.GetComponent<DamageDealer>().EnableHurtBox("AttkC12");
                            agent.gameObject.GetComponent<RotateTowardsPlayer>().RotateToPlayer(0.2f, 15.0f, 1.1f);
                            agent.gameObject.GetComponent<AudioSource>().PlayOneShot(AttkC1);
                            attackTimer = agent.gameObject.GetComponent<DamageDealer>().GetAttackDuration("AttkC1");
                        }
                        break;
                    case 2:
                        if(agent.GetBool("SecondPhase"))
                        {
                            agent.animator.SetTrigger("Combo2");
                            agent.gameObject.GetComponent<DamageDealer>().EnableHurtBox("AttkS2");
                            agent.gameObject.GetComponent<RotateTowardsPlayer>().RotateToPlayer(2.0f, 2.0f, 0.5f);
                            agent.gameObject.GetComponent<RotateTowardsPlayer>().MoveToPlayer(1.5f, 1.5f, 0.5f);
                            agent.gameObject.GetComponent<AudioSource>().PlayOneShot(AttkS2);
                            attackTimer = agent.gameObject.GetComponent<DamageDealer>().GetAttackDuration("AttkS2");
                        }
                        else
                        {
                            agent.animator.SetTrigger("Combo2");
                            agent.gameObject.GetComponent<DamageDealer>().EnableHurtBox("AttkC2");
                            agent.gameObject.GetComponent<RotateTowardsPlayer>().RotateToPlayer(2.0f, 2.0f, 0.5f);
                            agent.gameObject.GetComponent<RotateTowardsPlayer>().MoveToPlayer(1.5f, 1.5f, 0.5f);
                            agent.gameObject.GetComponent<AudioSource>().PlayOneShot(AttkC2);
                            attackTimer = agent.gameObject.GetComponent<DamageDealer>().GetAttackDuration("AttkC2");
                            
                        }
                        break;
                }
                
                attacking = true;
                agent.agent.isStopped = true;
                

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
