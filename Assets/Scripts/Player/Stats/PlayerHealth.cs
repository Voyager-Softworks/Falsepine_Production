using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;
using System;
using UnityEngine.Events;
using System.IO;
using Achievements;
using UnityEngine.SceneManagement;


/// <summary>
/// The health script for the player
/// @todo Make this script more generic and not specific to the player. (i.e. make it a generic health script, then inherit it for the player)
/// </summary>
public class PlayerHealth : MonoBehaviour, StatsManager.UsesStats
{
    [Header("Stats")]
    [ReadOnly] public string note = "Stats are store in stats manager";
    public float currentHealth
    {
        get
        {
            if (StatsManager.instance != null)
            {
                return StatsManager.instance.m_playerCurrentHealth;
            }
            else
            {
                return -1.0f;
            }
        }
        set
        {
            if (StatsManager.instance != null)
            {
                StatsManager.instance.m_playerCurrentHealth = value;
            }
        }
    }
    public float calcedMaxHealth
    {
        get
        {
            if (StatsManager.instance != null)
            {
                return StatsManager.instance.m_calcedPlayerMaxHealth;
            }
            else
            {
                return -1.0f;
            }
        }
        set
        {
            if (StatsManager.instance != null)
            {
                StatsManager.instance.m_calcedPlayerMaxHealth = value;
            }
        }
    }
    public bool isInvulnerable = false; ///< Whether or not the player is invulnerable.
    public bool godMode = false; ///< Whether or not the player is in god mode (permanent invulnerability).
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

    private InfoBox infoBox;
    
    public Sprite healthIcon;

    // StatsManager.UsesStats interface implementation
    public List<StatsManager.StatType> m_usedStatTypes = new List<StatsManager.StatType>() { };
    public List<StatsManager.StatType> GetStatTypes()
    {
        return m_usedStatTypes;
    }
    public void AddStatType(StatsManager.StatType type)
    {
        if (type == null) return;

        if (!m_usedStatTypes.Contains(type))
        {
            m_usedStatTypes.Add(type);
        }
    }
    public void RemoveStatType(StatsManager.StatType type)
    {
        if (m_usedStatTypes.Contains(type))
        {
            m_usedStatTypes.Remove(type);
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        if (uiScript == null) uiScript = FindObjectOfType<UIScript>();
        if (_audioSource == null) _audioSource = GetComponent<AudioSource>();
        if (_animator == null) _animator = GetComponentInChildren<Animator>();

        // heal 20% of max health
        Heal(calcedMaxHealth * 0.2f);
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

        uiScript.healthBar.rectTransform.sizeDelta = new Vector2(uiScript.healthBarMaxWidth * (currentHealth / calcedMaxHealth), uiScript.healthBar.rectTransform.sizeDelta.y);
        uiScript.healthBarDark.rectTransform.sizeDelta = Vector2.Lerp(uiScript.healthBarDark.rectTransform.sizeDelta, uiScript.healthBar.rectTransform.sizeDelta, Time.deltaTime * 2.0f);

        uiScript.healthText.text = currentHealth.ToString("0") + "/" + calcedMaxHealth.ToString("0");

        // health bar infobox
        // get mouse position
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        // convert mouse pos to local pos
        Vector2 localPos = uiScript.healthBG.rectTransform.InverseTransformPoint(mouseScreenPos);
        // show info box if mouse is over icon
        if (uiScript.healthBG.rectTransform.rect.Contains(localPos))
        {
            if (infoBox == null) infoBox = FindObjectOfType<InfoBox>();
            infoBox.DisplayMain("Player Health", healthIcon, "Health: " + currentHealth.ToString("0") + "/" + calcedMaxHealth + "\nYou heal 20% of your max health when you move to the next room.", "", null);
        }
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
        if (godMode || isInvulnerable || isDead) return;

        float calcedDamage = StatsManager.CalculateDamageTaken(this, damage);

        if (_audioSource && hurtSound) _audioSource.PlayOneShot(hurtSound);
        if (_animator) _animator.SetTrigger("Injured");
        _animator?.SetLayerWeight(2, 1);
        VignetteScript vs = FindObjectOfType<VignetteScript>();
        if (vs) vs.StartVignette();

        // stop reloading
        if (FindObjectOfType<PlayerInventoryInterface>() is PlayerInventoryInterface pii)
        {
            pii.TryEndReload();
        }

        currentHealth -= calcedDamage;
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
        if (isDead || godMode) return;

        isDead = true;
        currentHealth = 0;
        if (FindObjectOfType<AchievementsManager>() is AchievementsManager am)
        {
            am.UnlockAchievement(AchievementsManager.Achievement.Die);
            int deathCount = PlayerPrefs.GetInt("DeathCount", 0);
            deathCount++;
            PlayerPrefs.SetInt("DeathCount", deathCount);
            if (deathCount >= 50) am.UnlockAchievement(AchievementsManager.Achievement.DieXFifty);
            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "TutorialScene") am.UnlockAchievement(AchievementsManager.Achievement.DieInTutorial);
        }
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
    /// <param name="_amount">The amount of health to heal the player by.</param>
    public void Heal(float _amount)
    {
        float calcedHealAmount = StatsManager.CalculateHealAmount(this, _amount);

        currentHealth += calcedHealAmount;
        if (currentHealth > calcedMaxHealth)
        {
            currentHealth = calcedMaxHealth;
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
