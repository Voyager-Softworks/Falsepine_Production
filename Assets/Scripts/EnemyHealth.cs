using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;

/// <summary>
/// Manages the health of enemies.
/// @todo seperate the give money command to new script
/// </summary>
public class EnemyHealth : Health_Base
{
    public MonsterInfo m_monsterType;

    protected NodeAI.NodeAI_Senses m_senses; ///< The senses of the enemy.

    private SkinnedMeshRenderer m_renderer; ///< The renderer of the enemy.
    private List<Material> m_materials = new List<Material>(); ///< The materials of the enemy.

    public GameObject m_bloodEffect; ///< The blood effect to show when the enemy is damaged.

    public int m_silverReward = 3;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        m_senses = GetComponent<NodeAI.NodeAI_Senses>();

        m_renderer = GetComponentInChildren<SkinnedMeshRenderer>();
        m_materials.AddRange(m_renderer.materials);
    }


    /// <summary>
    /// It makes the object flash red, then fade back to white over the duration of the function
    /// </summary>
    /// <param name="duration">The amount of time the flash should last for.</param>
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

    /// <summary>
    /// > This function takes damage, and then tells the AI to update its health parameter, and then
    /// tells the animator to play a hit animation, and then tells the animator to play a random pain
    /// animation, and then stops the damage flash coroutine and starts it again, and then tells the
    /// senses to register a sensory event, and then instantiates a blood effect
    /// </summary>
    /// <param name="_damage">The damage object that contains the damage information.</param>
    /// <returns>
    /// The return type is void, so nothing is being returned.
    /// </returns>
    public override void TakeDamage(Health_Base.DamageStat _damage)
    {
        if (m_hasDied) return;

        base.TakeDamage(_damage);
        GetComponent<NodeAI.NodeAI_Agent>().SetParameter("Health", m_currentHealth);
        GetComponentInChildren<Animator>()?.SetTrigger("Hit");
        GetComponentInChildren<Animator>()?.SetFloat("PainNum", UnityEngine.Random.value);
        StopCoroutine("DamageFlashCoroutine");
        StartCoroutine(DamageFlashCoroutine(0.25f));

        m_senses?.RegisterSensoryEvent(_damage.m_sourceObject, this.gameObject, 20.0f, NodeAI.SensoryEvent.SenseType.SOMATIC);
        Instantiate(m_bloodEffect, _damage.m_hitPoint + (_damage.direction.normalized * 0.5f), Quaternion.LookRotation(_damage.direction));
    }

    /// <summary>
    ///  This function is called when the enemy dies. It tells the animator to play a death animation, and disables AI and colliders.
    ///  If the enemy has a ragdoll, it activates it.
    /// </summary>
    public override void Die()
    {
        base.Die();

        //GetComponent<NodeAI.NodeAI_Agent>().SetState("Dead"); Legacy code

        GetComponent<DamageDealer>()?.CancelAttack();

        GetComponentInChildren<Animator>().SetBool("Dead", true);

        // add kill
        StatsManager.instance.AddKill(m_monsterType);

        // Disable agent
        NodeAI.NodeAI_Agent agent = GetComponent<NodeAI.NodeAI_Agent>();
        if (agent != null) agent.enabled = false; ;

        if (m_senses != null) m_senses.enabled = false;

        // disable nav mesh agent
        NavMeshAgent navMeshAgent = GetComponent<NavMeshAgent>();
        if (navMeshAgent != null) navMeshAgent.enabled = false;


        Ragdoll ragdoll = GetComponent<Ragdoll>();
        if (ragdoll != null)
        {
            GetComponentInChildren<Animator>().enabled = false;
        }

        //stop audiosource
        AudioSource audioSource = GetComponent<AudioSource>();
        if (audioSource != null) audioSource.Stop();

        // get econ manager, and give money
        EconomyManager.instance.m_playerSilver += m_silverReward;
    }
}
