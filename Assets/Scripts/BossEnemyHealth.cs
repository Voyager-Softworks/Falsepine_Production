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
    public bool m_endScreenOnDeath = true; ///< Whether or not to end the game when the boss dies.
    public GameObject m_indicator = null; ///< The indicator to show when the boss is alive.
    public Mission m_linkedMission = null; ///< The mission to complete when the boss is alive.

    private UIScript m_uiScript; ///< The UI script to use.

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        m_senses = GetComponent<NodeAI.NodeAI_Senses>();
        if (m_uiScript == null) m_uiScript = FindObjectOfType<UIScript>();
        m_uiScript.bossNameText.text = gameObject.name;
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

        // sets the boss health bar size to represent the boss's health
        m_uiScript.bossHealthBar.rectTransform.sizeDelta = new Vector2(m_uiScript.bossHealthBarMaxWidth * (m_currentHealth / m_maxHealth), m_uiScript.bossHealthBar.rectTransform.sizeDelta.y);
    }

    public override void TakeDamage(Health_Base.DamageStat _damage)
    {
        base.TakeDamage(_damage);
    }

    /// <summary>
    /// When the boss is killed, end the game.
    /// </summary>
    public override void Die()
    {
        base.Die();

        m_indicator.SetActive(false);

        // try complete the linked mission, if it is the current mission
        MissionManager mm = MissionManager.instance;
        if (mm != null && m_linkedMission != null && mm.GetCurrentMission() == m_linkedMission)
        {
            mm.GetCurrentMission().SetState(MissionCondition.ConditionState.COMPLETE);
        }

        // fade to black and show complete scene
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
