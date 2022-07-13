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
    public bool isStunned = false;
    float stunTimer = 0f;

    [Header("Sounds")]
    public AudioClip deathSound;
    public AudioClip hurtSound;
    private AudioSource _audioSource;

    [Header("UI")]
    [HideInInspector] public UIScript uiScript;

    [Header("Events")]
    public UnityEvent OnDeath;

    private Animator _animator;

    // Start is called before the first frame update
    void Start()
    {
        if (uiScript == null) uiScript = FindObjectOfType<UIScript>();
        if (_audioSource == null) _audioSource = GetComponent<AudioSource>();
        if (_animator == null) _animator = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateUI();
        if (isStunned)
        {
            stunTimer -= Time.deltaTime;
            if (stunTimer <= 0f)
            {
                isStunned = false;
                
            }
        }
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
        if (_animator) _animator.SetTrigger("Injured");
        _animator?.SetLayerWeight(2, 1);
        VignetteScript vs = FindObjectOfType<VignetteScript>();
        if (vs) vs.StartVignette();

        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Stun(float duration)
    {
        stunTimer = Mathf.Max(stunTimer, duration);
        isStunned = true;
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

        //disable player movement, shooting, inventory, etc.
        PlayerMovement playerMovement = FindObjectOfType<PlayerMovement>();
        if (playerMovement) playerMovement.enabled = false;
        old_InventoryManager inventoryScript = FindObjectOfType<old_InventoryManager>();
        if (inventoryScript) inventoryScript.enabled = false;

        //set die trigger
        if (_animator) _animator.SetTrigger("Die");
        //set aim move x and z to 0
        if (_animator) _animator.SetFloat("MoveX", 0);
        if (_animator) _animator.SetFloat("MoveZ", 0);
        //set walking to false
        if (_animator) _animator.SetBool("Walking", false);
        //set aiming to false
        if (_animator) _animator.SetBool("Aiming", false);

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
