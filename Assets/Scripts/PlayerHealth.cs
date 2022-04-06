using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;
using System;
using UnityEngine.Events;

//NOTES:
// Player has health from 0-100, Starting at 100.
// Player must be able to become invulnerable (toggle).
// Player must be able to be healed.
// Player must be able to take damage from enemies.
// Player must be able to die.

public class PlayerHealth : MonoBehaviour
{
    [Header("Stats")]
    public float maxHealth = 100f;
    public float currentHealth = 100f;
    public bool isInvulnerable = false;
    public bool isDead = false;

    [Header("Sounds")]
    public AudioClip deathSound;
    public AudioClip hurtSound;
    private AudioSource _audioSource;

    [Header("UI")]
    [HideInInspector] public UIScript uiScript;

    [Header("Events")]
    public UnityEvent OnDeath;

    // Start is called before the first frame update
    void Start()
    {
        if (uiScript == null) uiScript = FindObjectOfType<UIScript>();
        if (_audioSource == null) _audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateUI();

        //check for health and dead
        if (!isDead && currentHealth <= 0){
            Die();
        }
    }

    public void UpdateUI()
    {
        if (uiScript == null) return;

        uiScript.healthBar.rectTransform.sizeDelta = new Vector2(uiScript.healthBarMaxWidth * (currentHealth / maxHealth), uiScript.healthBar.rectTransform.sizeDelta.y);
    }

    public void TakeDamage(float damage)
    {
        if (isInvulnerable || isDead) return;

        if (_audioSource && hurtSound) _audioSource.PlayOneShot(hurtSound);

        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Die(){
        if (isDead) return;

        isDead = true;
        currentHealth = 0;

        if (_audioSource && deathSound) _audioSource.PlayOneShot(deathSound);

        FadeScript fadeScript = FindObjectOfType<FadeScript>();
        if (fadeScript){
            fadeScript.EndScreen();
        }

        OnDeath.Invoke();
    }

    public void Heal(float heal)
    {
        currentHealth += heal;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
    }

    public void ToggleInvulnerability()
    {
        isInvulnerable = !isInvulnerable;
    }
}
