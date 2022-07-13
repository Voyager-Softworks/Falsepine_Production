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
public class RangedWeapon : Item
{
    // make sure to set variables as serialized fields in the inspector, so that they can be saved!

    // Shooting Performance:
    [SerializeField] public float m_damage = 0;
    [SerializeField] public float m_range = 0;
    [SerializeField] public float m_shootTime = 0;
    [SerializeField] private float m_shootTimer = 0;
    [SerializeField] public int m_projectileCount = 0;
    [SerializeField] public bool m_isAutomnatic = false;

    // Aiming:
    [SerializeField] public float m_aimedInaccuracy = 0;
    [SerializeField] public float m_unaimedInaccuracy = 0;
    [SerializeField] public float m_aimTime = 0;
    [SerializeField] private float m_aimTimer = 0;
    public bool m_isAiming = false;

    // Ammunition:
    [SerializeField] public int m_clipAmmo = 0;
    [SerializeField] public int m_clipSize = 0;
    
    // Reloading:
    [SerializeField] public float m_reloadTime = 0;
    [SerializeField] public float m_reloadTimer = 0;
    [SerializeField] private float m_reloadAmount = 0;

    // Other:
    [SerializeField] public float m_equipTime = 0;
    [SerializeField] private float m_equipTimer = 0;
    [SerializeField] public GameObject m_shootEffect = null;
    [SerializeField] public GameObject m_trailEffect = null;
    [SerializeField] public GameObject m_hitEffect = null;

    // Sounds:
    [SerializeField] public GameObject m_shootSound = null;
    [SerializeField] public GameObject m_emptySound = null;
    [SerializeField] public GameObject m_reloadSound = null;
    [SerializeField] public GameObject m_equipSound = null;
    [SerializeField] public GameObject m_startAimSound = null;
    [SerializeField] public GameObject m_stopAimSound = null;


    private float m_waitTimer = 0;

    /// <summary>
    /// [REQUIRED] Used to create a copy of the item. Make sure to set any unique values here!
    /// </summary>
    public override Item CreateInstance()
    {
        // [REQUIRED] Create base item
        RangedWeapon newItem = (RangedWeapon)base.CreateInstance();

        // Setting unique values here:
        newItem.m_damage = m_damage;
        newItem.m_range = m_range;
        newItem.m_shootTime = m_shootTime;
        newItem.m_projectileCount = m_projectileCount;
        newItem.m_isAutomnatic = m_isAutomnatic;
        newItem.m_aimedInaccuracy = m_aimedInaccuracy;
        newItem.m_unaimedInaccuracy = m_unaimedInaccuracy;
        newItem.m_aimTime = m_aimTime;
        newItem.m_clipAmmo = m_clipAmmo;
        newItem.m_clipSize = m_clipSize;
        newItem.m_reloadTime = m_reloadTime;
        newItem.m_equipTime = m_equipTime;
        newItem.m_shootEffect = m_shootEffect;
        newItem.m_hitEffect = m_hitEffect;
        newItem.m_trailEffect = m_trailEffect;
        newItem.m_shootSound = m_shootSound;
        newItem.m_emptySound = m_emptySound;
        newItem.m_reloadSound = m_reloadSound;
        newItem.m_equipSound = m_equipSound;
        newItem.m_startAimSound = m_startAimSound;
        newItem.m_stopAimSound = m_stopAimSound;


        return newItem;
    }

    public override void ManualUpdate(GameObject _owner){
        base.ManualUpdate(_owner);
        
        if (!_owner) return;

        // update all timers and ensure they are never negative:
        m_shootTimer = Mathf.Max(0, m_shootTimer - Time.deltaTime);
        if (!m_isAiming) m_aimTimer = Mathf.Max(0, m_aimTimer - Time.deltaTime);
        else m_aimTimer = Mathf.Min(m_aimTime, m_aimTimer + Time.deltaTime);
        m_reloadTimer = Mathf.Max(0, m_reloadTimer - Time.deltaTime);
        m_equipTimer = Mathf.Max(0, m_equipTimer - Time.deltaTime);

        m_waitTimer = Mathf.Max(0, m_waitTimer - Time.deltaTime);
    }

    /// <summary>
    /// Tries to shoot the weapon.
    /// </summary>
    public bool TryShoot(Vector3 _origin, Vector3 _direction, GameObject _owner)
    {
        if (m_isAiming && m_shootTimer <= 0 && m_waitTimer <= 0)
        {
            if (m_clipAmmo > 0)
            {
                Shoot(_origin, _direction, _owner);
                return true;
            }
            else {
                // play empty sound:
                if (m_emptySound != null)
                {
                    GameObject sound = Instantiate(m_emptySound, _origin, Quaternion.identity);
                }
            }
        }
        return false;
    }

    /// <summary>
    /// Shoots the weapon using raycasts. Uses wait timer.
    /// </summary>
    private void Shoot(Vector3 _origin, Vector3 _direction, GameObject _owner)
    {
        m_shootTimer = m_shootTime;
        UpdateWaitTimer(m_shootTime);

        // shoot raycasts
        for (int i = 0; i < m_projectileCount; i++)
        {
            float currentAimInaccuracy = CalcCurrentAimAngle();
            float aimAngle = UnityEngine.Random.Range(-currentAimInaccuracy, currentAimInaccuracy);
            Vector3 currentDirection = Quaternion.AngleAxis(aimAngle, Vector3.up) * _direction;

            // create raycast
            RaycastHit hit;
            Ray ray = new Ray(_origin, currentDirection);

            // shoot raycast
            if (Physics.Raycast(ray, out hit, m_range))
            {
                // get hit object
                GameObject hitObject = hit.collider.gameObject;

                //if hit something, apply damage
                HealthScript healthScript = hit.collider.GetComponentInChildren<HealthScript>();
                if (!healthScript) healthScript = hit.collider.GetComponentInParent<HealthScript>();
                if (healthScript != null)
                {
                    healthScript.TakeDamage(m_damage, _owner);
                    if (m_hitEffect) Destroy(Instantiate(m_hitEffect, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal)), 2.0f);
                }
                hit.collider.GetComponentInChildren<NodeAI.NodeAI_Senses>()?.RegisterSensoryEvent(
                                                                                                _owner,
                                                                                                hit.collider.gameObject,
                                                                                                20.0f,
                                                                                                NodeAI.SensoryEvent.SenseType.SOMATIC
                                                                                                );
            }

            // create trail effect
            if (m_trailEffect != null)
            {
                GameObject trail = Instantiate(m_trailEffect, _origin, Quaternion.identity);
                // LineRenderer lineRenderer = trail.GetComponent<LineRenderer>();
                // if (lineRenderer != null)
                // {
                //     lineRenderer.SetPosition(0, _origin);
                //     lineRenderer.SetPosition(1, (hit.distance > 0 ? hit.point : _origin + currentDirection * m_range));
                // }
                BulletTrail bulletTrail = trail.GetComponent<BulletTrail>();
                if (bulletTrail != null)
                {
                    bulletTrail.SetPositions(
                        _origin,
                        (hit.distance > 0 ? hit.point : _origin + currentDirection * m_range)
                    );
                }
            }
        }

        // play shoot sound:
        if (m_shootSound != null)
        {
            GameObject sound = Instantiate(m_shootSound, _origin, Quaternion.identity);
        }

        //Trigger auditory event on all sensors in range:
        foreach (NodeAI.NodeAI_Senses sensor in FindObjectsOfType<NodeAI.NodeAI_Senses>())
        {
            if (Vector3.Distance(sensor.gameObject.transform.position, _origin) < sensor.maxHearingRange)
            {
                sensor.RegisterSensoryEvent(_owner, sensor.gameObject, 10.0f, NodeAI.SensoryEvent.SenseType.AURAL);
            }
        }

        // create shoot effect:
        if (m_shootEffect != null)
        {
            GameObject effect = Instantiate(m_shootEffect, _origin, Quaternion.identity);
            effect.transform.LookAt(_origin + _direction);
        }

        m_clipAmmo--;
    }

    /// <summary>
    /// Tries to reload the weapon
    /// </summary>
    public bool TryReload(GameObject _owner)
    {
        if (m_reloadTimer <= 0 && m_waitTimer <= 0)
        {
            if (m_clipAmmo < m_clipSize)
            {
                Reload(_owner);
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Reloads the weapon. Uses wait timer
    /// </summary>
    private void Reload(GameObject _owner)
    {
        m_reloadTimer = m_reloadTime;
        UpdateWaitTimer(m_reloadTime);

        // play reload sound:
        if (m_reloadSound != null)
        {
            GameObject sound = Instantiate(m_reloadSound, _owner.transform.position, Quaternion.identity, _owner.transform);
        }

        m_clipAmmo = m_clipSize;
    }

    /// <summary>
    /// Tries to set aim bool.
    /// </summary>
    public bool TrySetAim(bool _aim, GameObject _owner)
    {
        // TODO: check if we can aim, or if other things are blocking it
        if (_aim && !m_isAiming)
        {
            m_isAiming = true;
            // play start aim sound:
            if (m_startAimSound != null)
            {
                GameObject sound = Instantiate(m_startAimSound, _owner.transform.position, Quaternion.identity);
            }
            return true;
        }
        else if (!_aim && m_isAiming)
        {
            m_isAiming = false;
            // play stop aim sound:
            if (m_stopAimSound != null)
            {
                GameObject sound = Instantiate(m_stopAimSound, _owner.transform.position, Quaternion.identity);
            }
            return true;
        }
        return false;
    }

    public float UpdateWaitTimer(float _time){
        m_waitTimer = Mathf.Max(m_waitTimer, _time);
        return m_waitTimer;
    }

    public float CalcCurrentAimAngle()
    {
        // calculate new direction using inaccuracy
        return Mathf.Lerp(m_unaimedInaccuracy, m_aimedInaccuracy, m_aimTimer / m_aimTime);
    }

    
    //Custom editor for this class
    #if UNITY_EDITOR
    [CustomEditor(typeof(RangedWeapon))]
    public class RangedWeaponEditor : ItemEditor
    {
        public override void OnInspectorGUI()
        {
            // [REQUIRED] draw base editor (Item in this case)
            base.OnInspectorGUI();
            // [REQUIRED] get the editor target
            RangedWeapon rangedWeapon = (RangedWeapon)target;

            // red box for weapon stats
            GUI.backgroundColor = Color.red;
            GUILayout.BeginVertical("box");
            GUI.backgroundColor = Color.white;
            // bold center text
            GUILayout.Label("Weapon Item Stats", CustomEditorStuff.center_bold_label);

            // horiz
            GUILayout.BeginHorizontal();

            // vertical
            GUILayout.BeginVertical();
            // performance:
            GUILayout.Label("Shooting Performance", CustomEditorStuff.center_bold_label);
            // damage
            rangedWeapon.m_damage = EditorGUILayout.FloatField(new GUIContent("Damage", "The amount of damage this weapon does"), rangedWeapon.m_damage);
            // range
            rangedWeapon.m_range = EditorGUILayout.FloatField(new GUIContent("Range", "The range of this weapon"), rangedWeapon.m_range);
            // shoot time
            rangedWeapon.m_shootTime = EditorGUILayout.FloatField(new GUIContent("Shoot Time", "The time it takes to shoot\nThe delay after one shot"), rangedWeapon.m_shootTime);
            // projectile count
            rangedWeapon.m_projectileCount = EditorGUILayout.IntField(new GUIContent("Projectile Count", "The amount of projectiles this weapon shoots"), rangedWeapon.m_projectileCount);
            // is automatic
            rangedWeapon.m_isAutomnatic = EditorGUILayout.Toggle(new GUIContent("Is Automatic", "If this weapon is automatic"), rangedWeapon.m_isAutomnatic);
            // end vertical
            GUILayout.EndVertical();

            //space
            GUILayout.Space(20);

            // vertical
            GUILayout.BeginVertical();
            // aiming:
            GUILayout.Label("Aiming", CustomEditorStuff.center_bold_label);
            // aimed accuracy
            rangedWeapon.m_aimedInaccuracy = EditorGUILayout.FloatField(new GUIContent("Aimed Inaccuracy", "The amount of degrees +/- of inaccuracy when AIMED"), rangedWeapon.m_aimedInaccuracy);
            // unaimed accuracy
            rangedWeapon.m_unaimedInaccuracy = EditorGUILayout.FloatField(new GUIContent("Unaimed Inaccuracy", "The amount of degrees +/- of inaccuracy when UNAIMED"), rangedWeapon.m_unaimedInaccuracy);
            // aim time
            rangedWeapon.m_aimTime = EditorGUILayout.FloatField(new GUIContent("Aim Time", "The time it takes to aim"), rangedWeapon.m_aimTime);
            // end vertical
            GUILayout.EndVertical();

            // end horiz
            GUILayout.EndHorizontal();

            // ammunition:
            GUILayout.Label("Ammunition", CustomEditorStuff.center_bold_label);
            // clip ammo
            rangedWeapon.m_clipAmmo = EditorGUILayout.IntField(new GUIContent("Clip Ammo", "The amount of ammo currently in the clip"), rangedWeapon.m_clipAmmo);
            // reserve ammo
            rangedWeapon.m_clipSize = EditorGUILayout.IntField(new GUIContent("Clip Size", "The amount of ammo the clip can carry"), rangedWeapon.m_clipSize);

            // reloading:
            GUILayout.Label("Reloading", CustomEditorStuff.center_bold_label);
            // reload time
            rangedWeapon.m_reloadTime = EditorGUILayout.FloatField(new GUIContent("Reload Time", "The time it takes to reload"), rangedWeapon.m_reloadTime);
            // reload amount
            rangedWeapon.m_reloadAmount = EditorGUILayout.FloatField(new GUIContent("Reload Amount", "The amount of ammo to reload"), rangedWeapon.m_reloadAmount);

            // other:
            GUILayout.Label("Other", CustomEditorStuff.center_bold_label);
            // equip time
            rangedWeapon.m_equipTime = EditorGUILayout.FloatField(new GUIContent("Equip Time", "The time it takes to equip"), rangedWeapon.m_equipTime);
            // shoot effect
            rangedWeapon.m_shootEffect = (GameObject)EditorGUILayout.ObjectField(new GUIContent("Shoot Effect", "The effect to play when shooting"), rangedWeapon.m_shootEffect, typeof(GameObject), false);
            // trail effect
            rangedWeapon.m_trailEffect = (GameObject)EditorGUILayout.ObjectField(new GUIContent("Trail Effect", "The effect to play when shooting"), rangedWeapon.m_trailEffect, typeof(GameObject), false);
            // hit effect
            rangedWeapon.m_hitEffect = (GameObject)EditorGUILayout.ObjectField(new GUIContent("Hit Effect", "The effect to play when hitting"), rangedWeapon.m_hitEffect, typeof(GameObject), false);

            // sounds:
            GUILayout.Label("Sounds", CustomEditorStuff.center_bold_label);
            // shoot sound
            rangedWeapon.m_shootSound = (GameObject)EditorGUILayout.ObjectField(new GUIContent("Shoot Sound", "The sound to play when shooting"), rangedWeapon.m_shootSound, typeof(GameObject), false);
            // empty sound
            rangedWeapon.m_emptySound = (GameObject)EditorGUILayout.ObjectField(new GUIContent("Empty Sound", "The sound to play when out of ammo"), rangedWeapon.m_emptySound, typeof(GameObject), false);
            // reload sound
            rangedWeapon.m_reloadSound = (GameObject)EditorGUILayout.ObjectField(new GUIContent("Reload Sound", "The sound to play when reloading"), rangedWeapon.m_reloadSound, typeof(GameObject), false);
            // equip sound
            rangedWeapon.m_equipSound = (GameObject)EditorGUILayout.ObjectField(new GUIContent("Equip Sound", "The sound to play when equipping"), rangedWeapon.m_equipSound, typeof(GameObject), false);
            // start aim sound
            rangedWeapon.m_startAimSound = (GameObject)EditorGUILayout.ObjectField(new GUIContent("Start Aim Sound", "The sound to play when starting to aim"), rangedWeapon.m_startAimSound, typeof(GameObject), false);
            // stop aim sound
            rangedWeapon.m_stopAimSound = (GameObject)EditorGUILayout.ObjectField(new GUIContent("Stop Aim Sound", "The sound to play when stopping to aim"), rangedWeapon.m_stopAimSound, typeof(GameObject), false);
            // end vertical






            //end red box
            GUILayout.EndVertical();

            // on change, save the changes
            if (GUI.changed)
            {
                EditorUtility.SetDirty(rangedWeapon);
            }
        }
    }
    #endif


}