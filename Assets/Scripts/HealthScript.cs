using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HealthScript : MonoBehaviour
{
    public bool isBoss = false;
    public float currentHealth = 100f;
    public float maxHealth = 100f;
    public bool isDead = false;
    public bool isInvincible = false;

    public bool endScreenOnDeath = true;

    public AudioClip deathSound;
    public AudioClip hurtSound;

    public UnityEvent OnDeath;

    private AudioSource _audioSource;

    private UIScript _uiScript;
    private NodeAI.NodeAI_Senses _senses;

    // Start is called before the first frame update
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _senses = GetComponent<NodeAI.NodeAI_Senses>();
        if (_uiScript == null) _uiScript = FindObjectOfType<UIScript>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateUI();

        //check for health and dead
        if (!isDead && currentHealth <= 0)
        {
            Die();
        }
    }

    private void UpdateUI()
    {
        //boss health bar
        if (_uiScript == null) return;

        if (isBoss) _uiScript.bossHealthBar.rectTransform.sizeDelta = new Vector2(_uiScript.bossHealthBarMaxWidth * (currentHealth / maxHealth), _uiScript.bossHealthBar.rectTransform.sizeDelta.y);
    }

    public void TakeDamage(float damage, GameObject source)
    {
        if (isInvincible) return;
        if (isDead) return;

        currentHealth -= damage;
        _senses?.RegisterSensoryEvent(source, this.gameObject, damage, NodeAI.SensoryEvent.SenseType.SOMATIC);
        if (_audioSource && hurtSound) _audioSource.PlayOneShot(hurtSound);
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Die(){
        isDead = true;
        currentHealth = 0;

        OnDeath.Invoke();
        //GetComponent<NodeAI.NodeAI_Agent>().SetState("Dead"); Legacy code

        GetComponentInChildren<Animator>().SetBool("Dead", true);

        if (_audioSource && deathSound) _audioSource.PlayOneShot(deathSound);

        if (endScreenOnDeath)
        {
            FadeScript fadeScript = FindObjectOfType<FadeScript>();
            if (fadeScript)
            {
                fadeScript.EndScreen();
            }
        }

    }

    public void Heal(float heal)
    {
        if (isDead) return;

        currentHealth += heal;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
    }

    public void SetInvincible(bool _invincible)
    {
        isInvincible = _invincible;
    }

    // public void SetRegenerating(bool _regenerating)
    // {
    //     isRegenerating = _regenerating;
    // }


}
