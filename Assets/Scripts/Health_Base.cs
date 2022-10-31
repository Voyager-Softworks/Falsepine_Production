using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using UnityEngine.Serialization;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// A base health class for all objects that can take damage.
/// </summary>
[RequireComponent(typeof(AudioController))]
public class Health_Base : MonoBehaviour, StatsManager.UsesStats /// @todo Impliment this into players, enemies, destructable objects.
{

    public static List<Health_Base> allHealths = new List<Health_Base>();

    private void OnDrawGizmos()
    {
        // draw the bounding box
        Collider collider = GetComponent<Collider>();
        if (collider != null)
        {
            Gizmos.color = new Color(1, 0, 0, 0.3f);
            Gizmos.DrawWireCube(collider.bounds.center, collider.bounds.size);
        }

        // draw the last 10 damage numbers
        #if UNITY_EDITOR
        if (m_damageHistory.Count > 0){
            int count = 0;
            for (int i = m_damageHistory.Count - 1; i >= 0; i--){
                float val = m_damageHistory[i].m_damage;
                Vector3 pos = m_damageHistory[i].m_hitPoint;
                pos.y += 3;
                Handles.color = Color.red;
                // make new style
                GUIStyle style = new GUIStyle();
                style.normal.textColor = Color.red;
                float timeSince = (Time.time - m_damageHistory[i].m_time);
                // size between 20-0, based on time since (after 3 seconds, it will be 0)
                style.fontSize = (int)Mathf.Lerp(20, 0, timeSince / 3.0f);
                style.fontStyle = FontStyle.Bold;
                Handles.Label(pos, val.ToString(), style);

                count += 1;
            }
        }
        #endif
    }

    /// <summary>
    ///  A data object containing info about a damage dealt.
    /// </summary>
    [Serializable]
    public class DamageStat
    {
        public float m_damage; ///< The amount of damage dealt.
        public GameObject m_sourceObject; ///< The object that dealt the damage.
        public Vector3 m_originPoint; ///< The point of origin of the damage.
        public Vector3 m_hitPoint; ///< The point of impact of the damage.
        public Vector3 direction { get { return m_hitPoint - m_originPoint; } } ///< The direction the attack was moving in.
        public float m_time; ///< The time the damage was dealt.

        public List<StatsManager.StatType> m_sourceStatsTypes; ///< The stats of the object that dealt the damage. (used for kill tracking and whatnot)
        public string m_itemID; ///< The ID of the item that dealt the damage. (used for kill tracking and whatnot)
        public string m_itemDisplayName; ///< The display name of the item that dealt the damage. (used for kill tracking and whatnot)


        public DamageStat(float damage, GameObject sourceObject, Vector3 origin, Vector3 hitPoint, StatsManager.UsesStats sourceStats)
        {
            m_damage = damage;
            m_sourceObject = sourceObject;
            m_originPoint = origin;
            m_hitPoint = hitPoint;
            m_time = Time.time;
            m_sourceStatsTypes = new List<StatsManager.StatType>(sourceStats.GetStatTypes());
            if (sourceStats is Item item)
            {
                m_itemID = item.id;
                m_itemDisplayName = item.m_displayName;
            }
        }

        public DamageStat(float damage, GameObject sourceObject, Vector3 origin, Vector3 hitPoint, Item item)
        {
            m_damage = damage;
            m_sourceObject = sourceObject;
            m_originPoint = origin;
            m_hitPoint = hitPoint;
            m_time = Time.time;
            m_sourceStatsTypes = new List<StatsManager.StatType>(item.GetStatTypes());
            m_itemID = item.id;
            m_itemDisplayName = item.m_displayName;
        }

        public DamageStat(float damage, GameObject sourceObject, Vector3 origin, Vector3 hitPoint, StatsManager.UsesStats sourceStats, Item item)
        {
            m_damage = damage;
            m_sourceObject = sourceObject;
            m_originPoint = origin;
            m_hitPoint = hitPoint;
            m_time = Time.time;
            m_sourceStatsTypes = new List<StatsManager.StatType>(sourceStats.GetStatTypes());
            m_itemID = item.id;
            m_itemDisplayName = item.m_displayName;
        }
    }

    /// <summary>
    ///  Context object containing relavent information about the death of the object.
    /// </summary>
    public struct DeathContext
    {
        public GameObject m_sourceObject; ///< The object that dealt the killing blow.
        public Vector3 m_originPoint;  ///< The point of origin of the damage.
        public Vector3 m_hitPoint; ///< The point of impact of the damage.
        public Vector3 Direction { get { return m_hitPoint - m_originPoint; } } ///< The direction the attack was moving in.
        public float m_time; ///< The time the damage was dealt.
    }

    public System.Action<DeathContext> Death; ///< Delegate for when the object dies.
    public System.Action<DamageStat> Damage; ///< Delegate for when the object takes damage.

    [Header("Stats")]
    public float m_currentHealth = 100f; ///< The current health of the object.
    [SerializeField] private float m_maxHealth = 100f; ///< The maximum health of the object.
    public float calcedMaxHealth {
        get {
            return StatsManager.CalculateMaxHealth(this, m_maxHealth);
        }
        set {
            m_maxHealth = value;
        }
    }

    protected bool m_isInvulnerable = false; ///< Whether the object is invulnerable.
    public bool isInvulnerable { get { return m_isInvulnerable; } set { m_isInvulnerable = value; } } ///< Whether the object is invulnerable.
    protected bool m_hasDied = false; ///< Whether the object has died.
    public bool hasDied { get { UpdateDeath(); return m_hasDied; } } ///< Whether the object has died.

    public bool m_disablePlayerCollision = true; ///< Whether or not the player should be able to collide with the object.

    public List<StatsManager.StatType> m_weaknesses = new List<StatsManager.StatType>(); ///< The weaknesses of the object.

    public List<DamageStat> m_damageHistory = new List<DamageStat>(); ///< The damage history of the object.

    // StatsManager.UsesStats interface implementation
    public List<StatsManager.StatType> m_usedStatTypes = new List<StatsManager.StatType>();
    public virtual List<StatsManager.StatType> GetStatTypes(){
        return m_usedStatTypes;
    }
    public void AddStatType(StatsManager.StatType type){
        if (type == null) return;

        if (!m_usedStatTypes.Contains(type))
        {
            m_usedStatTypes.Add(type);
        }
    }
    public void RemoveStatType(StatsManager.StatType type){
        if (m_usedStatTypes.Contains(type))
        {
            m_usedStatTypes.Remove(type);
        }
    }

    [Header("Sounds")]
    public string m_deathSoundName = "DeathSound"; ///< The sound to play when the object dies. Uses the audio controller component.
    public string m_damageSoundname = "DamageSound"; ///< The sound to play when the object takes damage. Uses the audio controller component.
    private AudioController m_audioController;

    [Header("UI")]
    public bool m_useDamagePopup = true; ///< Whether or not to use damage popups.

    public virtual void Start()
    {
        CheckMaxHealth();
        UpdateDeath();

        m_audioController = GetComponent<AudioController>();
    }

    protected virtual void OnEnable() {
        AddHealthScript();
    }

    protected virtual void OnDisable() {
        RemoveHealthScript();
    }

    private void AddHealthScript() {
        if (!allHealths.Contains(this)) {
            allHealths.Add(this);
        }
    }

    private void RemoveHealthScript() {
        if (allHealths.Contains(this)) {
            allHealths.Remove(this);
        }
    }

    public virtual void Update()
    {
        //UpdateDeath();
    }

    /// <summary>
    ///  Calls the Die method if the object is not already dead and has a health value of 0 or less.
    /// </summary>
    protected void UpdateDeath()
    {
        if (!m_hasDied && m_currentHealth <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// Deals damage to this object.
    /// </summary>
    /// <param name="_damageStat">Data object containing info about the damage to be dealt.</param>
    public virtual void TakeDamage(DamageStat _damageStat)
    {
        // if dead or invulnerable, do nothing
        if (m_isInvulnerable || m_hasDied) return;

        if (_damageStat == null) return;

        float calcedDamage = StatsManager.CalculateDamageTaken(this, _damageStat.m_damage);
        _damageStat.m_damage = calcedDamage;

        // if weakness, 1.5x damage
        bool weak = false;
        foreach (StatsManager.StatType weakness in m_weaknesses)
        {
            if (_damageStat.m_sourceStatsTypes.Contains(weakness))
            {
                weak = true;
                break;
            }
        }

        if (weak)
        {
            _damageStat.m_damage *= 1.5f;
        }

        // add to damage history
        m_damageHistory.Add(_damageStat);

        // deal dmg
        m_currentHealth -= _damageStat.m_damage;

        Damage?.Invoke(_damageStat);

        UpdateDeath();

        PlayHurtSound();

        // show damage popup
        if (m_useDamagePopup && NotificationManager.instance && NotificationManager.instance.m_damagePopupPrefab)
        {
            GameObject damagePopup = Instantiate(NotificationManager.instance.m_damagePopupPrefab, _damageStat.m_hitPoint, Quaternion.identity);
            damagePopup.GetComponent<DamagePopup>().ShowPopup(_damageStat);
        }
    }

    /// <summary>
    /// Kills this object.
    /// </summary>
    public virtual void Die()
    {
        if (m_hasDied) return;

        m_hasDied = true;

        PlayDeathSound();

        Death?.Invoke(new DeathContext()
        {
            m_sourceObject = m_damageHistory[m_damageHistory.Count - 1].m_sourceObject,
            m_originPoint = m_damageHistory[m_damageHistory.Count - 1].m_originPoint,
            m_hitPoint = m_damageHistory[m_damageHistory.Count - 1].m_hitPoint,
            m_time = m_damageHistory[m_damageHistory.Count - 1].m_time
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
    /// <param name="_amount">The amount of health to heal.</param>
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
        if (m_currentHealth > calcedMaxHealth)
        {
            m_currentHealth = calcedMaxHealth;
        }
    }

    /// <summary>
    /// Plays a random hurt sound.
    /// </summary>
    public void PlayHurtSound()
    {
        if (m_audioController == null) return;

        m_audioController.PlayOnce(m_damageSoundname);
    }

    /// <summary>
    /// Plays a random death sound.
    /// </summary>
    public void PlayDeathSound()
    {
        if (m_audioController == null) return;

        m_audioController.Play(m_deathSoundName);
    }

}
