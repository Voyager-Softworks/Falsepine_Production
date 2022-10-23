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

    public List<Artifact> m_artifacts = new List<Artifact>(); ///< The artifacts to drop when the boss dies.

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

        UpdateUI();
    }

    private void UpdateUI()
    {
        //boss health bar
        if (m_uiScript == null) return;

        // sets the boss health bar size to represent the boss's health
        m_uiScript.bossHealthBar.rectTransform.sizeDelta = new Vector2(m_uiScript.bossHealthBarMaxWidth * (m_currentHealth / calcedMaxHealth), m_uiScript.bossHealthBar.rectTransform.sizeDelta.y);
        m_uiScript.bossHealthBarDark.rectTransform.sizeDelta = Vector2.Lerp(m_uiScript.bossHealthBarDark.rectTransform.sizeDelta, m_uiScript.bossHealthBar.rectTransform.sizeDelta, Time.deltaTime * 2f);
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

        if (m_indicator != null) m_indicator.SetActive(false);

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
                fadeScript.EndScreen(true, 5f);
            }
        }

        // add artifact to home inventory
        if (m_artifacts.Count > 0)
        {
            Artifact artifact = m_artifacts[UnityEngine.Random.Range(0, m_artifacts.Count)];
            if (artifact != null)
            {
                Inventory home = InventoryManager.instance.GetInventory("home");
                if (home != null)
                {
                    if (home.TryAddItemToInventory(artifact.CreateInstance()) == null)
                    {
                        // message
                        if (MessageManager.instance)
                        {
                            MessageManager.instance.AddMessage("You found a " + artifact.m_displayName + "!", "talisman");
                        }
                        // notify
                        NotificationManager.instance?.AddIcon("talisman", transform.position + Vector3.up * 2f);
                    }
                }
            }
        }

        // message
        if (MessageManager.instance)
        {
            MessageManager.instance.AddMessage("You defeated " + m_monsterType.m_name + "!", "journal", true);
            // notify
            NotificationManager.instance?.AddIcon("ammo", transform.position + Vector3.up * 2f);
        }
    }
}
