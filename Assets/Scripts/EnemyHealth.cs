using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;

/// <summary>
/// Manages the health of enemies.
/// </summary>
public class EnemyHealth : Health_Base
{
    protected NodeAI.NodeAI_Senses m_senses;

    private SkinnedMeshRenderer m_renderer;
    private List<Material> m_materials = new List<Material>();

    public GameObject m_bloodEffect;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        m_senses = GetComponent<NodeAI.NodeAI_Senses>();

        m_renderer = GetComponentInChildren<SkinnedMeshRenderer>();
        m_materials.AddRange(m_renderer.materials);
    }

    IEnumerator DamageFlashCoroutine(float duration)
    {
        Vector3 startScale = transform.localScale;
        Vector3 endScale = startScale * 1.03f;

        Color flashCol = new Color(1f, 0.6f, 0.6f, 1f);
        // Make the material flash red
        for (int i = 0; i < m_materials.Count; i++)
        {
            m_materials[i].SetColor("_BaseColor", flashCol);
        }

        // fade the material back to normal over the duration
        float elapsedTime = 0;
        while (elapsedTime < duration)
        {
            for (int i = 0; i < m_materials.Count; i++)
            {
                m_materials[i].SetColor("_BaseColor", Color.Lerp(flashCol, Color.white, elapsedTime / duration));
            }
            transform.localScale = Vector3.Lerp(endScale, startScale, elapsedTime / duration);  
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
    }

    public override void TakeDamage(Health_Base.DamageStat _damage)
    {
        base.TakeDamage(_damage);
        GetComponent<NodeAI.NodeAI_Agent>().SetParameter("Health", m_currentHealth);
        GetComponentInChildren<Animator>()?.SetTrigger("Hit");
        GetComponentInChildren<Animator>()?.SetFloat("PainNum", UnityEngine.Random.value);
        StopCoroutine("DamageFlashCoroutine");
        StartCoroutine(DamageFlashCoroutine(0.25f));

        m_senses?.RegisterSensoryEvent(_damage.m_sourceObject, this.gameObject, 20.0f, NodeAI.SensoryEvent.SenseType.SOMATIC);
        Instantiate(m_bloodEffect, _damage.m_hitPoint + (_damage.direction.normalized * 0.5f) , Quaternion.LookRotation(_damage.direction));
    }

    public override void Die(){
        base.Die();
        
        //GetComponent<NodeAI.NodeAI_Agent>().SetState("Dead"); Legacy code

        GetComponent<DamageDealer>()?.CancelAttack();

        GetComponentInChildren<Animator>().SetBool("Dead", true);

        // Disable agent
        NodeAI.NodeAI_Agent agent = GetComponent<NodeAI.NodeAI_Agent>();
        if (agent != null) agent.enabled = false;;

        if (m_senses != null) m_senses.enabled = false;

        // disable nav mesh agent
        NavMeshAgent navMeshAgent = GetComponent<NavMeshAgent>();
        if (navMeshAgent != null) navMeshAgent.enabled = false;

        // do ragdoll
        Ragdoll ragdoll = GetComponent<Ragdoll>();
        if (ragdoll != null) 
        {
            ragdoll.EnableRagdoll();
        }

        //stop audiosource
        AudioSource audioSource = GetComponent<AudioSource>();
        if (audioSource != null) audioSource.Stop();
    }
}
