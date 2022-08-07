using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;

/// <summary>
/// Manages the health of boss enemies.
/// </summary>
public class BossEnemyHealth : EnemyHealth
{
    public bool m_endScreenOnDeath = true;
    public GameObject m_indicator = null;
    public Mission m_linkedMission = null;

    private UIScript m_uiScript;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        m_senses = GetComponent<NodeAI.NodeAI_Senses>();
        if (m_uiScript == null) m_uiScript = FindObjectOfType<UIScript>();
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();

        UpdateUI();
    }

    private void UpdateUI()
    {
        //boss health bar
        if (m_uiScript == null) return;

        m_uiScript.bossHealthBar.rectTransform.sizeDelta = new Vector2(m_uiScript.bossHealthBarMaxWidth * (m_currentHealth / m_maxHealth), m_uiScript.bossHealthBar.rectTransform.sizeDelta.y);
    }

    public override void TakeDamage(Health_Base.DamageStat _damage)
    {
        base.TakeDamage(_damage);
    }

    public override void Die(){
        base.Die();

        m_indicator.SetActive(false);

        MissionManager mm = MissionManager.instance;
        if (mm != null && m_linkedMission != null && mm.GetCurrentMission() == m_linkedMission)
        {
            mm.GetCurrentMission().SetCompleted(true);
        }

        if (m_endScreenOnDeath)
        {
            FadeScript fadeScript = FindObjectOfType<FadeScript>();
            if (fadeScript)
            {
                fadeScript.EndScreen(true);
            }
        }
    }
}
