using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;
using System;
using UnityEngine.Events;


/// <summary>
/// The health script for the player
/// @todo Make this script more generic and not specific to the player. (i.e. make it a generic health script, then inherit it for the player)
/// </summary>
public class PlayerHealth : MonoBehaviour
{
    [Header("Stats")]
    public float maxHealth = 100f; ///< The maximum health of the player.
    public float currentHealth = 100f; ///< The current health of the player.
    public bool isInvulnerable = false; ///< Whether or not the player is invulnerable.
    public bool isDead = false; ///< Whether or not the player is dead.
    public bool isStunned = false; ///< Whether or not the player is stunned.
    float stunTimer = 0f; ///< The timer for the stun.

    [Header("Sounds")]
    public AudioClip deathSound; ///< The sound to play when the player dies.
    public AudioClip hurtSound; ///< The sound to play when the player is hurt.
    private AudioSource _audioSource; ///< The audio source for the player.

    [Header("UI")]
    [HideInInspector] public UIScript uiScript; ///< The UI script for the player.

    [Header("Events")]
    public UnityEvent OnDeath; ///< The event to call when the player dies.
    public System.Action OnDamageTaken; ///< The event to call when the player takes damage.

    private Animator _animator; ///< The animator for the player.

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
        if (!isDead && currentHealth <= 0)
        {
            Die();
        }
    }

    public void UpdateUI()
    {
        if (uiScript == null) return;

        uiScript.healthBar.rectTransform.sizeDelta = new Vector2(uiScript.healthBarMaxWidth * (currentHealth / maxHealth), uiScript.healthBar.rectTransform.sizeDelta.y);
    }

    /// <summary>
    /// If the player is not invulnerable, play the hurt sound, trigger the injured animation, start the
    /// vignette, and subtract the damage from the current health. If the current health is less than or
    /// equal to zero, call the Die() function
    /// </summary>
    /// <param name="damage">The amount of damage to take.</param>
    /// <returns>
    /// The method is returning void, so nothing is being returned.
    /// </returns>
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

        OnDamageTaken?.Invoke();
    }

    /// <summary>
    /// "If the stunTimer is less than the duration, set the stunTimer to the duration and set isStunned
    /// to true."
    /// </summary>
    /// <param name="duration">The duration of the stun in seconds.</param>
    public void Stun(float duration)
    {
        stunTimer = Mathf.Max(stunTimer, duration);
        isStunned = true;
    }

    /// <summary>
    /// The player dies, the game ends, and the player's movement, shooting, and inventory are disabled
    /// </summary>
    /// <returns>
    /// The method is returning void, so nothing is being returned.
    /// </returns>
    public void Die()
    {
        if (isDead) return;

        isDead = true;
        currentHealth = 0;

        if (_audioSource && deathSound) _audioSource.PlayOneShot(deathSound);

        FadeScript fadeScript = FindObjectOfType<FadeScript>();
        if (fadeScript)
        {
            fadeScript.EndScreen(false);
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

        // stop all animations
        if (_animator) _animator.StopPlayback();

        OnDeath.Invoke();
    }

    /// <summary>
    /// This function takes in a float value and adds it to the current health. If the current health is
    /// greater than the max health, then the current health is set to the max health
    /// </summary>
    /// <param name="heal">The amount of health to heal the player by.</param>
    public void Heal(float heal)
    {
        currentHealth += heal;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
    }

    /// <summary>
    ///  Toggle the invulnerability of the player.
    /// </summary>
    public void ToggleInvulnerability()
    {
        isInvulnerable = !isInvulnerable;
    }
}
