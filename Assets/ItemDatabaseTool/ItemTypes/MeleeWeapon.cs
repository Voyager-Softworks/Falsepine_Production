using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
using System.Linq;

//[REQUIRED]
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// An example of a custom item type that can be used in the item database.
/// </summary>
//Make sure to set the [Serializable] attribute on this class, so that it can be saved!
[Serializable]
public class MeleeWeapon : Item
{
    public override string GetTypeDisplayName(){
        return "Melee";
    }

    // make sure to set variables as serialized fields in the inspector, so that they can be saved!
    [SerializeField] public float m_damage = 0;
    [SerializeField] public float m_radius = 0;
    [SerializeField] public float m_height = 0;
    [SerializeField] public float m_damageTime = 0;
    [SerializeField] public float m_damageTimer = 0;
    [SerializeField] public float m_swingCooldown = 0;
    [SerializeField] public float m_swingCooldownTimer = 0;
    [SerializeField] public float m_comboTime = 0;
    [SerializeField] public float m_comboTimer = 0;
    [SerializeField] public bool m_shouldDoComboSwing = false;

    // Sounds:
    [SerializeField] public GameObject m_swingSound = null;
    [SerializeField] public GameObject m_hitSolidSound = null;
    [SerializeField] public GameObject m_hitFleshSound = null;


    private List<Health_Base> m_hitObjects = new List<Health_Base>();

    private Transform m_damageTrans;

    /// <summary>
    /// [REQUIRED] Used to create a copy of the item. Make sure to set any unique values here!
    /// </summary>
    public override Item CreateInstance()
    {
        // [REQUIRED] Create base item
        MeleeWeapon newItem = (MeleeWeapon)base.CreateInstance();

        // Setting unique values here:
        newItem.m_damage = m_damage;
        newItem.m_radius = m_radius;
        newItem.m_height = m_height;
        newItem.m_damageTime = m_damageTime;
        newItem.m_damageTimer = m_damageTimer;
        newItem.m_swingCooldown = m_swingCooldown;
        newItem.m_swingCooldownTimer = m_swingCooldownTimer;
        newItem.m_comboTime = m_comboTime;
        newItem.m_comboTimer = m_comboTimer;
        newItem.m_shouldDoComboSwing = m_shouldDoComboSwing;
        
        // sounds:
        newItem.m_swingSound = m_swingSound;
        newItem.m_hitSolidSound = m_hitSolidSound;
        newItem.m_hitFleshSound = m_hitFleshSound;

        return newItem;
    }

    // update
    public override void ManualUpdate(GameObject _owner)
    {
        base.ManualUpdate(_owner);

        // update all timers and ensure they are never negative:
        m_damageTimer = Mathf.Max(0, m_damageTimer - Time.deltaTime);
        m_comboTimer = Mathf.Max(0, m_comboTimer - Time.deltaTime);
        m_swingCooldownTimer = Mathf.Max(0, m_swingCooldownTimer - Time.deltaTime);

        // if damage timer is active, do damage cast
        if (m_damageTimer > 0)
        {
            DoDamage(_owner);
            // enable trail under parent
            if (m_damageTrans != null && m_damageTrans.parent.GetComponentInChildren<TrailRenderer>() is TrailRenderer trail)
            {
                trail.enabled = true;
            }
        }
        else
        {
            // disable trail under parent
            if (m_damageTrans != null && m_damageTrans.parent.GetComponentInChildren<TrailRenderer>() is TrailRenderer trail)
            {
                trail.enabled = false;
            }
        }

        // if combo timer reaches 0, reset combo
        if (m_comboTimer <= 0)
        {
            m_shouldDoComboSwing = false;
        }
    }

    /// <summary>
    /// Checks capsulecast for damageable objects and deals damage to them.
    /// </summary>
    /// <param name="_owner"></param>
    private void DoDamage(GameObject _owner)
    {
        if (m_damageTrans == null || m_damageTrans.gameObject.activeSelf == false) return;

        // do damage:
        Collider[] hitColliders = Physics.OverlapCapsule(m_damageTrans.position + Vector3.up * m_height, m_damageTrans.position - Vector3.up * m_height , m_radius);
        foreach (Collider hitCollider in hitColliders)
        {
            if (hitCollider.gameObject != _owner)
            {
                // get health
                Health_Base health = hitCollider.GetComponent<Health_Base>();
                if (health != null)
                {
                    // if health is not already in list, add it and deal damage
                    if (m_hitObjects.Contains(health) == false)
                    {
                        m_hitObjects.Add(health);

                        //calc then deal damage
                        float calcdDamage = StatsManager.CalculateDamage(this, m_damage);
                        health.TakeDamage(new Health_Base.DamageStat(calcdDamage, _owner, _owner.transform.position, m_damageTrans.position, this));

                        // play respective hit sound
                        if (health as EnemyHealth != null)
                        {
                            if (m_hitFleshSound != null)
                            {
                                Instantiate(m_hitFleshSound, m_damageTrans.position, m_damageTrans.rotation);
                            }
                        }
                        else
                        {
                            if (m_hitSolidSound != null)
                            {
                                Instantiate(m_hitSolidSound, m_damageTrans.position, m_damageTrans.rotation);
                            }
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    ///  Checks if allowed to do melee, if so, starts the melee attack.
    /// </summary>
    /// <param name="_damageTrans"></param>
    /// <param name="_owner"></param>
    /// <returns></returns>
    public bool TryMelee(Transform _damageTrans, GameObject _owner){
        m_damageTrans = _damageTrans;
        if (m_swingCooldownTimer <= 0)
        {
            DoMelee(_damageTrans, _owner);

            // unlock achievement:
            Achievements.AchievementsManager.instance?.UnlockAchievement(this.m_unlockAchievement);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Starts the melee attack timers and sets the damage transform.
    /// </summary>
    /// <param name="_damageTrans"></param>
    /// <param name="_owner"></param>
    public void DoMelee(Transform _damageTrans, GameObject _owner){
        // reset timers:
        m_swingCooldownTimer = m_swingCooldown;
        m_damageTimer = m_damageTime;

        // reset hit objects:
        m_hitObjects.Clear();

        UpdateComboSwing();

        // play swing sound:
        if (m_swingSound != null)
        {
            GameObject swingSound = Instantiate(m_swingSound, _damageTrans.position, _damageTrans.rotation);
            swingSound.transform.parent = _damageTrans;
        }
    }

    /// <summary>
    /// Checks if the combo timer is up, if so, can combo.
    /// </summary>
    /// <returns></returns>
    public bool CanCombo()
    {
        return m_comboTimer > 0;
    }

    public void UpdateComboSwing()
    {
        if (CanCombo())
        {
            m_shouldDoComboSwing = !m_shouldDoComboSwing;
        }
        
        m_comboTimer = m_comboTime;
    }
    
    //Custom editor for this class
    #if UNITY_EDITOR
    [CustomEditor(typeof(MeleeWeapon))]
    public class MeleeWeaponEditor : ItemEditor
    {
        public override void OnInspectorGUI()
        {
            // [REQUIRED] draw base editor (Item in this case)
            base.OnInspectorGUI();
            // [REQUIRED] get the editor target
            MeleeWeapon item = (MeleeWeapon)target;

            // red box for weapon stats
            GUI.backgroundColor = Color.red;
            GUILayout.BeginVertical("box");
            GUI.backgroundColor = Color.white;
            // bold center text
            GUILayout.Label("MeleeWeapon Stats", CustomEditorStuff.center_bold_label);

            // Your custom values here
            item.m_damage = EditorGUILayout.FloatField(new GUIContent("Damage", "The damage of the weapon."), item.m_damage);
            item.m_radius = EditorGUILayout.FloatField(new GUIContent("Radius", "The damage radius of the tip of the weapon."), item.m_radius);
            item.m_height = EditorGUILayout.FloatField(new GUIContent("Height", "The vertical damage height of the tip of the weapon."), item.m_height);
            item.m_damageTime = EditorGUILayout.FloatField(new GUIContent("Damage Time", "The time in seconds that the weapon will deal damage after clicking swing.\nProbably make this less than the cooldown"), item.m_damageTime);
            item.m_swingCooldown = EditorGUILayout.FloatField(new GUIContent("Swing Cooldown", "The cooldown after clicking swing"), item.m_swingCooldown);
            item.m_comboTime = EditorGUILayout.FloatField(new GUIContent("Combo Time", "Swinging quicker than this again will combo.\nMake sure it is longer than the cooldown!"), item.m_comboTime);
            // ensure combo time is longer than cooldown
            if (item.m_comboTime < item.m_swingCooldown)
            {
                item.m_comboTime = item.m_swingCooldown;
            }

            // sounds:
            GUILayout.Label("Sounds", CustomEditorStuff.center_bold_label);
            // shoot sound
            item.m_swingSound = (GameObject)EditorGUILayout.ObjectField(new GUIContent("Swing Sound", "The sound to play when swinging the weapon."), item.m_swingSound, typeof(GameObject), false);
            // solid hit sound
            item.m_hitSolidSound = (GameObject)EditorGUILayout.ObjectField(new GUIContent("Hit Solid Sound", "The sound to play when hitting a solid object."), item.m_hitSolidSound, typeof(GameObject), false);
            // flesh hit sound
            item.m_hitFleshSound = (GameObject)EditorGUILayout.ObjectField(new GUIContent("Hit Flesh Sound", "The sound to play when hitting a flesh object."), item.m_hitFleshSound, typeof(GameObject), false);

            //end red box
            GUILayout.EndVertical();

            if (GUI.changed)
            {
                EditorUtility.SetDirty(item);
            }
        }
    }
    #endif
	
	
}
