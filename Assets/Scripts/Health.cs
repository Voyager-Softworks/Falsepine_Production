using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A base health class for all objects that can take damage.
/// @Todo: Impliment this into players, enemies, destructable objects.
/// </summary>
public class Health : MonoBehaviour
{
    public class DamageStat
    {
        public float m_damage;
        public GameObject m_sourceObject;
        public Vector3 m_originPoint;
        public Vector3 m_hitPoint;
        public Vector3 direction { get { return m_hitPoint - m_originPoint; } }
        public float m_time;

        public DamageStat(float damage, GameObject sourceObject, Vector3 origin, Vector3 hitPoint)
        {
            m_damage = damage;
            m_sourceObject = sourceObject;
            m_originPoint = origin;
            m_hitPoint = hitPoint;
            m_time = Time.time;
        }
    }

    [Header("Stats")]
    public float m_currentHealth = 100f;
    public float m_maxHealth = 100f;
    protected bool m_isInvulnerable = false;
    public bool isInvulnerable { get { return m_isInvulnerable; } set { m_isInvulnerable = value; } }
    protected bool m_hasDied = false;
    public bool hasDied { get { UpdateDeath(); return m_hasDied; } }

    public List<DamageStat> m_damageHistory = new List<DamageStat>();

    [Header("Sounds")]
    public List<GameObject> m_deathSounds = new List<GameObject>();
    public List<GameObject> m_hurtSounds = new List<GameObject>();

    public virtual void Start(){
        CheckMaxHealth();
        UpdateDeath();
    }

    public virtual void Update() {
        UpdateDeath();
    }

    protected void UpdateDeath() {
        if (!m_hasDied && m_currentHealth <= 0) {
            Die();
        }
    }

    /// <summary>
    /// Deals damage to this object.
    /// </summary>
    /// <param name="damage"></param>
    /// <param name="source"></param>
    public virtual void TakeDamage(DamageStat _damageStat) {
        // if dead or invulnerable, do nothing
        if (m_isInvulnerable || m_hasDied) return;

        if (_damageStat == null) return;

        // deal dmg
        m_currentHealth -= _damageStat.m_damage;

        UpdateDeath();

        PlayHurtSound();

        // add to damage history
        m_damageHistory.Add(_damageStat);
    }

    /// <summary>
    /// Kills this object.
    /// </summary>
    public virtual void Die() {
        if (m_hasDied) return;

        m_hasDied = true;

        PlayDeathSound();
    }

    /// <summary>
    /// Heals this object.
    /// </summary>
    /// <param name="_amount"></param>
    public virtual void Heal(float _amount)
    {
        m_currentHealth += _amount;
        CheckMaxHealth();
    }

    /// <summary>
    /// Caps the current health at the max health.
    /// </summary>
    protected void CheckMaxHealth()
    {
        if (m_currentHealth > m_maxHealth)
        {
            m_currentHealth = m_maxHealth;
        }
    }

    /// <summary>
    /// Plays a random hurt sound.
    /// </summary>
    public void PlayHurtSound() {
        if (m_hurtSounds.Count > 0) {
            GameObject sound = m_hurtSounds[Random.Range(0, m_hurtSounds.Count)];
            if (sound != null) {
                Instantiate(sound, transform.position, Quaternion.identity, transform);
            }
        }
    }

    /// <summary>
    /// Plays a random death sound.
    /// </summary>
    public void PlayDeathSound() {
        if (m_deathSounds.Count > 0) {
            GameObject sound = m_deathSounds[Random.Range(0, m_deathSounds.Count)];
            if (sound != null) {
                Instantiate(sound, transform.position, Quaternion.identity);
            }
        }
    }

}
