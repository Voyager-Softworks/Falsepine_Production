using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// A base health class for all objects that can take damage.
/// @todo Impliment this into players, enemies, destructable objects.
/// </summary>
public class Health_Base : MonoBehaviour
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

    public struct DeathContext
    {
        public GameObject m_sourceObject;
        public Vector3 m_originPoint;
        public Vector3 m_hitPoint;
        public Vector3 Direction { get { return m_hitPoint - m_originPoint; } }
        public float m_time;
    }

    public System.Action<DeathContext> Death;
    public System.Action<DamageStat> Damage;

    [Header("Stats")]
    public float m_currentHealth = 100f;
    public float m_maxHealth = 100f;
    protected bool m_isInvulnerable = false;
    public bool isInvulnerable { get { return m_isInvulnerable; } set { m_isInvulnerable = value; } }
    protected bool m_hasDied = false;
    public bool hasDied { get { UpdateDeath(); return m_hasDied; } }

    public bool m_disablePlayerCollision = true;

    public List<DamageStat> m_damageHistory = new List<DamageStat>();

    [Header("Sounds")]
    public GameObject m_deathSound = null;
    public GameObject m_hurtSound = null;

    public virtual void Start(){
        CheckMaxHealth();
        UpdateDeath();
    }

    public virtual void Update() {
        //UpdateDeath();
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

        // add to damage history
        m_damageHistory.Add(_damageStat);

        // deal dmg
        m_currentHealth -= _damageStat.m_damage;

        Damage?.Invoke(_damageStat);

        UpdateDeath();

        PlayHurtSound();
    }

    /// <summary>
    /// Kills this object.
    /// </summary>
    public virtual void Die() {
        if (m_hasDied) return;

        m_hasDied = true;

        PlayDeathSound();

        Death?.Invoke(new DeathContext() {
            m_sourceObject = m_damageHistory[m_damageHistory.Count-1].m_sourceObject,
            m_originPoint = m_damageHistory[m_damageHistory.Count-1].m_originPoint,
            m_hitPoint = m_damageHistory[m_damageHistory.Count-1].m_hitPoint,
            m_time = m_damageHistory[m_damageHistory.Count-1].m_time
        });

        if (m_disablePlayerCollision)
        {
            Collider[] colliders = GetComponentsInChildren<Collider>();
            foreach (Collider collider in colliders)
            {
                collider.gameObject.layer = LayerMask.NameToLayer("NOPlayerCollide");
            }
        }
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
        if (m_hurtSound == null) return;

        GameObject sound = Instantiate(m_hurtSound, transform.position, Quaternion.identity, transform);
    }

    /// <summary>
    /// Plays a random death sound.
    /// </summary>
    public void PlayDeathSound() {
        if (m_deathSound == null) return;

        GameObject sound = Instantiate(m_deathSound, transform.position, Quaternion.identity);
    }

}
