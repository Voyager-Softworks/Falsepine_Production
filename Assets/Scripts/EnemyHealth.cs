using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;

/// <summary>
/// Manages the health of enemies.
/// </summary>
public class EnemyHealth : Health
{
    protected NodeAI.NodeAI_Senses m_senses;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        m_senses = GetComponent<NodeAI.NodeAI_Senses>();
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
    }

    public override void TakeDamage(Health.DamageStat _damage)
    {
        base.TakeDamage(_damage);
        GetComponent<NodeAI.NodeAI_Agent>().SetParameter("Health", m_currentHealth);

        m_senses?.RegisterSensoryEvent(_damage.m_sourceObject, this.gameObject, _damage.m_damage, NodeAI.SensoryEvent.SenseType.SOMATIC);
    }

    public override void Die(){
        base.Die();
        
        //GetComponent<NodeAI.NodeAI_Agent>().SetState("Dead"); Legacy code

        GetComponent<DamageDealer>()?.CancelAttack();

        GetComponentInChildren<Animator>().SetBool("Dead", true);

        NodeAI.NodeAI_Agent agent = GetComponent<NodeAI.NodeAI_Agent>();
        if (agent != null) agent.enabled = false;;

        if (m_senses != null) m_senses.enabled = false;

        NavMeshAgent navMeshAgent = GetComponent<NavMeshAgent>();
        if (navMeshAgent != null) navMeshAgent.enabled = false;

        Ragdoll ragdoll = GetComponent<Ragdoll>();
        if (ragdoll != null) ragdoll.EnableRagdoll();
    }
}
