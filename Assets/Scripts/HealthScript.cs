using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HealthScript : MonoBehaviour
{
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


    // Start is called before the first frame update
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();

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

        _uiScript.bossHealthBar.rectTransform.sizeDelta = new Vector2(_uiScript.bossHealthBarMaxWidth * (currentHealth / maxHealth), _uiScript.bossHealthBar.rectTransform.sizeDelta.y);
    }

    public void TakeDamage(float damage)
    {
        if (isInvincible) return;
        if (isDead) return;

        currentHealth -= damage;
        GetComponent<NodeAI.NodeAI_Agent>().SetFloat("Health", currentHealth);
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
        GetComponent<NodeAI.NodeAI_Agent>().SetState("Dead");
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
