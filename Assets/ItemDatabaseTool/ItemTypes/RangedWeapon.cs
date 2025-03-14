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
/// The main weapon type of the game.
/// </summary>
//Make sure to set the [Serializable] attribute on this class, so that it can be saved!
[Serializable]
public class RangedWeapon : Item
{
    // make sure to set variables as serialized fields in the inspector, so that they can be saved!

    public override string GetTypeDisplayName(){
        return "Ranged";
    }

    // Shooting Performance:
    [SerializeField] public float m_damage = 0;
    [SerializeField] public float m_range = 0;
    [SerializeField] public float m_shootTime = 0;
    [SerializeField] private float m_shootTimer = 0;
    [SerializeField] public int m_maxHitsPerShot = 0;
    [SerializeField] public bool m_isAutomnatic = false;

    // Aiming:
    [SerializeField] public float m_aimedInaccuracy = 0;
    public float calcedAimedInaccuracy {
        get {
            return StatsManager.CalculateInaccuracy(this, m_aimedInaccuracy);
        }
    }
    [SerializeField] public float m_unaimedInaccuracy = 0;
    public float calcedUnaimedInaccuracy {
        get {
            return StatsManager.CalculateInaccuracy(this, m_unaimedInaccuracy);
        }
    }
    [SerializeField] public float m_aimTime = 0;
    [SerializeField] private float m_aimTimer = 0;
    [SerializeField] public float m_horizFalloffMult = 0;
    public bool m_isAiming = false;

    // Ammunition:
    [SerializeField] public int m_clipAmmo = 0;
    [SerializeField] public int m_clipSize = 0;
    [SerializeField] public int m_spareAmmo = 0;
    [SerializeField] public int m_maxSpareAmmo = 0;
    public int CalcedMaxSpareAmmo{
        get{
            return StatsManager.CalculateMaxSpareAmmo(this, m_maxSpareAmmo);
        }
    }
    [SerializeField] public bool m_unlimitedAmmo = false;
    [SerializeField] public List<StatsManager.StatType> m_tempAmmoStats = new List<StatsManager.StatType>();
    
    // Reloading:
    [SerializeField] public bool m_splitReload = false;
    [SerializeField] public bool m_isReloading = false;
    [SerializeField] private int m_reloadAmount = 1;
    [SerializeField] public float m_reloadTime = 0;
    [SerializeField] private float m_reloadTimer = 0;

    // Other:
    [SerializeField] public float m_equipTime = 0;
    [SerializeField] private float m_equipTimer = 0;
    [SerializeField] public GameObject m_shootEffect = null;
    [SerializeField] public GameObject m_trailEffect = null;
    [SerializeField] public GameObject m_hitEffect = null;
    [SerializeField] public GameObject m_splashEffect = null;

    // Sounds:
    [SerializeField] public GameObject m_shootSound = null;
    [SerializeField] public GameObject m_emptySound = null;
    [SerializeField] public GameObject m_reloadSound = null;
    [SerializeField] public GameObject m_startReloadSound = null;
    [SerializeField] public GameObject m_singleReloadSound = null;
    [SerializeField] public GameObject m_endReloadSound = null;
    [SerializeField] public GameObject m_equipSound = null;
    [SerializeField] public GameObject m_startAimSound = null;
    [SerializeField] public GameObject m_stopAimSound = null;

    // save variables from last shoot attempt:
    public struct ShotInfo
    {
        public Vector3 originPoint;
        public Vector3 hitPoint;

        public Vector3 zoneIntersection;
        public Vector2 zoneUVPoint;

        public Health_Base healthScriptHit;

        public float damage;
    }
    public List<ShotInfo> m_allShots = new List<ShotInfo>();

    private float m_waitTimer = 0;

    public override List<StatsManager.StatType> GetStatTypes()
    {
        List<StatsManager.StatType> statTypes = base.GetStatTypes();

        // union of ammo stats and base stats
        statTypes = statTypes.Union(m_tempAmmoStats).ToList();

        return statTypes;
    }

    /// <summary>
    /// [REQUIRED] Used to create a copy of the item. Make sure to set any unique values here!
    /// </summary>
    public override Item CreateInstance()
    {
        // [REQUIRED] Create base item
        RangedWeapon newItem = (RangedWeapon)base.CreateInstance();

        // Setting unique values here:
        // Shooting Performance:
        newItem.m_damage = m_damage;
        newItem.m_range = m_range;
        newItem.m_shootTime = m_shootTime;
        newItem.m_maxHitsPerShot = m_maxHitsPerShot;
        newItem.m_isAutomnatic = m_isAutomnatic;

        // Aiming:
        newItem.m_aimedInaccuracy = m_aimedInaccuracy;
        newItem.m_unaimedInaccuracy = m_unaimedInaccuracy;
        newItem.m_aimTime = m_aimTime;
        newItem.m_horizFalloffMult = m_horizFalloffMult;

        // Ammunition:
        newItem.m_clipAmmo = m_clipAmmo;
        newItem.m_clipSize = m_clipSize;
        newItem.m_spareAmmo = m_spareAmmo;
        newItem.m_maxSpareAmmo = m_maxSpareAmmo;
        newItem.m_unlimitedAmmo = m_unlimitedAmmo;
        newItem.m_tempAmmoStats = m_tempAmmoStats;

        // Reloading:
        newItem.m_splitReload = m_splitReload;
        newItem.m_reloadAmount = m_reloadAmount;
        newItem.m_reloadTime = m_reloadTime;

        // Other:
        newItem.m_equipTime = m_equipTime;
        newItem.m_shootEffect = m_shootEffect;
        newItem.m_hitEffect = m_hitEffect;
        newItem.m_trailEffect = m_trailEffect;
        newItem.m_splashEffect = m_splashEffect;

        // Sounds:
        newItem.m_shootSound = m_shootSound;
        newItem.m_emptySound = m_emptySound;
        newItem.m_reloadSound = m_reloadSound;
        newItem.m_startReloadSound = m_startReloadSound;
        newItem.m_singleReloadSound = m_singleReloadSound;
        newItem.m_endReloadSound = m_endReloadSound;
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
        float calcedAimTime = StatsManager.CalculateRangedAimTime(this, m_aimTime);
        if (m_isAiming) m_aimTimer = Mathf.Min(calcedAimTime, m_aimTimer + Time.deltaTime);
        else m_aimTimer = 0;
        m_equipTimer = Mathf.Max(0, m_equipTimer - Time.deltaTime);

        m_waitTimer = Mathf.Max(0, m_waitTimer - Time.deltaTime);

        m_reloadTimer = Mathf.Max(0, m_reloadTimer - Time.deltaTime);
        // if current state is Reload_Start, Reload_Middle, or Reload_End, then keep resetting timer
        if (m_isReloading && m_splitReload){
            Animator anim = _owner.GetComponentInChildren<Animator>();
            if (anim){
                if (anim.GetCurrentAnimatorStateInfo(2).IsName("Reload_Start") || anim.GetCurrentAnimatorStateInfo(2).IsName("Reload_Middle") || anim.GetCurrentAnimatorStateInfo(2).IsName("Reload_End")){
                    m_reloadTimer = m_reloadTime;
                }
            }
        }
        // if timer is up but is still reloading, then stop reloading
        if (m_isReloading && m_reloadTimer <= 0){
            TryEndReload(_owner);
        }
    }

    /// <summary>
    /// Tries to shoot the weapon.
    /// </summary>
    public bool TryShoot(Transform _originTransform, Vector3 _direction, GameObject _owner, AimZone _aimZone)
    {
        if (!m_isReloading && m_isAiming && m_shootTimer <= 0 && m_waitTimer <= 0)
        {
            // get origin point:
            Vector3 originPoint = _originTransform.position;

            if (m_clipAmmo > 0)
            {
                //RaycastShoot(_origin, _direction, _owner);
                AimZoneShoot(_originTransform, _direction, _owner, _aimZone);

                // unlock achievement:
                Achievements.AchievementsManager.instance?.UnlockAchievement(this.m_unlockAchievement);
                return true;
            }
            else {
                m_shootTimer = m_shootTime;
                UpdateWaitTimer(m_shootTime);

                // play empty sound:
                if (m_emptySound != null)
                {
                    GameObject sound = Instantiate(m_emptySound, originPoint, Quaternion.identity);
                }
            }
        }
        return false;
    }

    /// <summary>
    /// Uses the aim zone to detect what to shoot and how much damage to deal<br/>
    /// </summary>
    /// <param name="_origin"></param>
    /// <param name="_direction"></param>
    /// <param name="_owner"></param>
    /// <param name="_aimZone"></param>
    public void AimZoneShoot(Transform _originTransform, Vector3 _direction, GameObject _owner, AimZone _aimZone){

        // get the origin point:
        Vector3 originPoint = _originTransform.position;

        m_shootTimer = m_shootTime;
        UpdateWaitTimer(m_shootTime);

        // get all objects with healthScript in the scene
        List<Health_Base> healthScripts = new List<Health_Base>(Health_Base.allHealths);
        // sort by distance from the aimQuad
        Vector3 aimQuadPos = _aimZone.transform.position;
        healthScripts.Sort((x, y) => Vector3.Distance(x.transform.position, aimQuadPos).CompareTo(Vector3.Distance(y.transform.position, aimQuadPos)));

        // store hitlist
        List<ShotInfo> hitList = new List<ShotInfo>();

        //clear allShots
        m_allShots.Clear();

        //List<HealthScript> healthScriptsInAimZone = new List<HealthScript>();
        foreach (Health_Base healthScript in healthScripts)
        {
            if (healthScript.hasDied) continue;
            // get bounds of healthScript object
            Collider col = healthScript.GetComponent<Collider>();
            // if no collider, skip
            if (col == null) continue;
            // make collider not trigger
            bool isTrigger = col.isTrigger;
            col.isTrigger = false;
            Bounds? tempBounds = col.bounds;
            if (tempBounds == null) continue;
            Bounds bounds = (Bounds)tempBounds;

            ShotInfo shotInfo = new ShotInfo();

            List<Vector3> intersections1 = new List<Vector3>();
            List<Vector3> intersections2 = new List<Vector3>();

            bool inTri1 = BoundsIntersectTriangle(bounds, _aimZone.bl, _aimZone.fl, _aimZone.fr, out intersections1);
            bool inTry2 = BoundsIntersectTriangle(bounds, _aimZone.bl, _aimZone.fr, _aimZone.br, out intersections2);

            // combine the two lists of intersections
            List<Vector3> intersections = new List<Vector3>();
            intersections.AddRange(intersections1);
            intersections.AddRange(intersections2);

            // set shotInfo.damage to the highest damage value
            shotInfo.damage = 0;
            foreach (Vector3 intersection in intersections)
            {
                //update damage
                float calcdDamage = StatsManager.CalculateDamage(this, m_damage);
                calcdDamage *= _aimZone.CalcDmgMult_float(intersection, m_horizFalloffMult);

                if (calcdDamage > shotInfo.damage)
                {
                    shotInfo.damage = calcdDamage;
                    shotInfo.zoneIntersection = intersection;
                }
            }

            // split aimzone into two triangles (bl, fl, fr) and (bl, fr, br)
            if (inTri1 || inTry2)
            {
                // raycast all 8 corners of the bounds, and the center of the bounds
                List<Vector3> corners = new List<Vector3>();
                corners.Add(bounds.center);
                corners.AddRange(GetCorners(bounds));
                foreach (Vector3 corner in corners)
                {
                    RaycastHit hitInfo;
                    // layer mask for all layers
                    int layerMask = -1;
                    // to hit position
                    Vector3 toPosition = Vector3.Lerp(corner, bounds.center, 0.5f);
                    float calcedRange = StatsManager.CalculateRange(this, m_range);
                    if (Physics.Raycast(originPoint, toPosition - originPoint, out hitInfo, calcedRange, layerMask, QueryTriggerInteraction.Ignore))
                    {
                        
                        if (hitInfo.collider.GetComponentInParent<Health_Base>() == healthScript)
                        {
                            // update shotInfo
                            shotInfo.originPoint = originPoint;
                            shotInfo.hitPoint = hitInfo.point;
                            shotInfo.healthScriptHit = healthScript;

                            hitList.Add(shotInfo);
                            break;
                        }
                    }
                }
            }

            // reset collider
            col.isTrigger = isTrigger;
        }

        for (int i = 0; i < hitList.Count; i++)
        {
            //get shotInfo from list
            ShotInfo shotInfo = hitList[i];

            //update UV
            shotInfo.zoneUVPoint = _aimZone.CalculateUVFromWorld(shotInfo.zoneIntersection);

            //update list
            hitList[i] = shotInfo;
        }

        //sort hitList by damage (most first) (if damage is within 0.1f of eachother, sort by distance)
        hitList.Sort((x, y) =>
        {
            if (Mathf.Abs(x.damage - y.damage) < 0.1f)
            {
                return Vector3.Distance(x.hitPoint, originPoint).CompareTo(Vector3.Distance(y.hitPoint, originPoint));
            }
            return y.damage.CompareTo(x.damage);
        });

        // split hitList by EnemyHealth and PropHealth
        List<ShotInfo> priorityList = new List<ShotInfo>();
        List<ShotInfo> propHitList = new List<ShotInfo>();
        List<ShotInfo> remainingHitList = new List<ShotInfo>();
        foreach (ShotInfo shotInfo in hitList)
        {
            if (shotInfo.healthScriptHit.GetComponent<EnemyHealth>() != null)
            {
                priorityList.Add(shotInfo);
            }
            else if (shotInfo.healthScriptHit.GetComponent<PropHealth>() != null)
            {
                if (shotInfo.healthScriptHit.GetComponent<ExplodeOnDeath>() != null)
                {
                    priorityList.Add(shotInfo);
                }
                else
                {
                    propHitList.Add(shotInfo);
                }
            }
            else
            {
                remainingHitList.Add(shotInfo);
            }
        }

        // try shoot enemies, then props, then anything else
        hitList.Clear();
        hitList.AddRange(priorityList);
        hitList.AddRange(propHitList);
        hitList.AddRange(remainingHitList);

        // deal damage to healthScripts in aimZone (using m_maxHitsPerShot)
        for (int i = 0; i < hitList.Count && i < m_maxHitsPerShot; i++)
        {
            //get shotInfo
            ShotInfo shotInfo = hitList[i];

            //add to allShots
            m_allShots.Add(shotInfo);

            // check if player has Splash Damage
            if (StatsManager.GetAllStatMods().Any(x => x.statType == StatsManager.StatType.SplashDamage)){

                // calculate radius
                float minRadius = 3.5f;
                float dmgRadStart = 9.0f;
                float maxRadius = 8.0f;
                float dmgRadEnd = 40.0f;
                float radius = Mathf.Lerp(minRadius, maxRadius, Mathf.InverseLerp(dmgRadStart, dmgRadEnd, shotInfo.damage));

                // calculate damage
                float splashDamage = StatsManager.CalculateSplashDamage(this, m_damage);

                // splash effect
                if (m_splashEffect != null)
                {
                    GameObject splashEffect = Instantiate(
                        m_splashEffect, 
                        shotInfo.hitPoint,
                        Quaternion.FromToRotation(Vector3.up, shotInfo.hitPoint - shotInfo.originPoint)
                    );
                    // multiply current local scale by radius
                    splashEffect.transform.localScale *= radius;
                    Destroy(splashEffect, 2.0f);
                }

                // do damage check
                Collider[] colliders = Physics.OverlapSphere(shotInfo.hitPoint, radius);
                List<Health_Base> hitObjects = new List<Health_Base>();
                List<PlayerHealth> hitPlayers = new List<PlayerHealth>();
                foreach (Collider collider in colliders)
                {
                    List<StatsManager.StatType> statsList = new List<StatsManager.StatType>(this.GetStatTypes());

                    // get health from parent and children
                    Health_Base health = collider.transform.GetComponentInParent<Health_Base>();
                    if (health == null) health = collider.transform.GetComponentInChildren<Health_Base>();
                    if (health != null && !hitObjects.Contains(health))
                    {
                        hitObjects.Add(health);
                        health.TakeDamage(new Health_Base.DamageStat(shotInfo.damage, _owner, originPoint, health.transform.position, this));

                        // hit effect
                        if (m_hitEffect != null)
                        {
                            Destroy(Instantiate(
                            m_hitEffect, health.transform.position,
                            Quaternion.FromToRotation(Vector3.up, health.transform.position - shotInfo.originPoint)),
                            2.0f);
                        }
                    }

                    // get player health from parent and children
                    PlayerHealth playerHealth = collider.transform.GetComponentInParent<PlayerHealth>();
                    if (playerHealth == null) playerHealth = collider.transform.GetComponentInChildren<PlayerHealth>();
                    if (playerHealth != null && !hitPlayers.Contains(playerHealth))
                    {
                        hitPlayers.Add(playerHealth);
                        playerHealth.TakeDamage(shotInfo.damage);
                    }
                }
            }
            else{
                shotInfo.healthScriptHit.TakeDamage(new Health_Base.DamageStat(shotInfo.damage, _owner, originPoint, shotInfo.hitPoint, this));

                // hit effect
                if (m_hitEffect != null)
                {
                    Destroy(Instantiate(
                    m_hitEffect, shotInfo.hitPoint,
                    Quaternion.FromToRotation(Vector3.up, shotInfo.hitPoint - shotInfo.originPoint)),
                    2.0f);
                }
            }

            //create trail effect
            if (m_trailEffect != null)
            {
                GameObject trail = Instantiate(m_trailEffect, originPoint, Quaternion.identity);
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
                        shotInfo.originPoint,
                        shotInfo.hitPoint
                    );
                }
            }
        }

        // play shoot sound:
        if (m_shootSound != null)
        {
            GameObject sound = Instantiate(m_shootSound, originPoint, Quaternion.identity);
            Destroy(sound, 2.0f);

            // if ammo is last round, set the pitch of the sound to 1.5x (and not single shot)
            if (m_clipAmmo <= 1 && m_clipSize > 2)
            {
                AutoSound auto = sound.GetComponent<AutoSound>();
                if (auto != null)
                {
                    auto.minPitch *= 1.3f;
                    auto.maxPitch *= 1.3f;
                }

                // play empty sound
                if (m_emptySound != null)
                {
                    GameObject emptySound = Instantiate(m_emptySound, originPoint, Quaternion.identity);
                    Destroy(emptySound, 2.0f);
                }
            }
        }

        //Trigger auditory event on all sensors in range:
        foreach (NodeAI.NodeAI_Senses sensor in FindObjectsOfType<NodeAI.NodeAI_Senses>())
        {
            if (Vector3.Distance(sensor.gameObject.transform.position, originPoint) < sensor.maxHearingRange)
            {
                sensor.RegisterSensoryEvent(_owner, sensor.gameObject, 10.0f, NodeAI.SensoryEvent.SenseType.AURAL);
            }
        }

        // create shoot effect:
        if (m_shootEffect != null)
        {
            GameObject effect = Instantiate(m_shootEffect, originPoint, Quaternion.identity);
            // look same direction as originTransform
            effect.transform.rotation = _originTransform.rotation;
            Destroy(effect, 2.0f);
        }

        m_clipAmmo--;
    }

    /// <summary>
    /// Checks if a point is within a triangle
    /// </summary>
    /// <param name="_point"></param>
    /// <param name="_triangleA"></param>
    /// <param name="_triangleB"></param>
    /// <param name="_triangleC"></param>
    /// <param name="intersection"></param>
    /// <returns></returns>
    public static bool PointInTriangle(Vector3 _point, Vector3 _triangleA, Vector3 _triangleB, Vector3 _triangleC, out Vector3 intersection)
    {
        // set y to 0
        _point.y = 0;
        _triangleA.y = 0;
        _triangleB.y = 0;
        _triangleC.y = 0;

        // get vectors
        Vector3 v0 = _triangleC - _triangleA;
        Vector3 v1 = _triangleB - _triangleA;
        Vector3 v2 = _point - _triangleA;

        // get dot products
        float dot00 = Vector3.Dot(v0, v0);
        float dot01 = Vector3.Dot(v0, v1);
        float dot02 = Vector3.Dot(v0, v2);
        float dot11 = Vector3.Dot(v1, v1);
        float dot12 = Vector3.Dot(v1, v2);

        // get barycentric coordinates
        float b = (dot00 * dot11 - dot01 * dot01);
        float inv = 1 / b;
        float u = (dot11 * dot02 - dot01 * dot12) * inv;
        float v = (dot00 * dot12 - dot01 * dot02) * inv;

        if (u >= 0 && v >= 0 && u + v < 1)
        {
            intersection = _triangleA + u * v0 + v * v1;
            return true;
        }
        else
        {
            intersection = Vector3.zero;
            return false;
        }

        // check if point is in triangle
        return (u >= 0) && (v >= 0) && (u + v < 1);
    }

    /// <summary>
    /// Checks if two lines intersect, and where
    /// </summary>
    /// <param name="a1"></param>
    /// <param name="a2"></param>
    /// <param name="b1"></param>
    /// <param name="b2"></param>
    /// <param name="intersection"></param>
    /// <returns></returns>
    public static bool LinesIntersect(Vector3 a1, Vector3 a2, Vector3 b1, Vector3 b2, out Vector3 intersection) {
        // set y to 0
        a1.y = 0;
        a2.y = 0;
        b1.y = 0;
        b2.y = 0;

        intersection = Vector3.zero;

        float denominator = (b2.z - b1.z) * (a2.x - a1.x) - (b2.x - b1.x) * (a2.z - a1.z);
        if (denominator == 0) {
            return false;
        }
        float ua = ((b2.x - b1.x) * (a1.z - b1.z) - (b2.z - b1.z) * (a1.x - b1.x)) / denominator;
        float ub = ((a2.x - a1.x) * (a1.z - b1.z) - (a2.z - a1.z) * (a1.x - b1.x)) / denominator;

        if (ua >= 0 && ua <= 1 && ub >= 0 && ub <= 1) {
            intersection = new Vector3(a1.x + ua * (a2.x - a1.x), 0, a1.z + ua * (a2.z - a1.z));
            return true;
        }
        return false;
    }

    /// <summary>
    /// Checks if bounds are intersecting the given triangle.
    /// </summary>
    /// <param name="_bounds"></param>
    /// <param name="_triangleA"></param>
    /// <param name="_triangleB"></param>
    /// <param name="_triangleC"></param>
    /// <returns></returns>
    public static bool BoundsIntersectTriangle(Bounds _bounds, Vector3 _triangleA, Vector3 _triangleB, Vector3 _triangleC, out List<Vector3> intersections)
    {
        Vector3 b1 = _bounds.center + new Vector3(_bounds.extents.x, 0, _bounds.extents.z);
        Vector3 b2 = _bounds.center + new Vector3(_bounds.extents.x, 0, -_bounds.extents.z);
        Vector3 b3 = _bounds.center + new Vector3(-_bounds.extents.x, 0, _bounds.extents.z);
        Vector3 b4 = _bounds.center + new Vector3(-_bounds.extents.x, 0, -_bounds.extents.z);

        // list of all intersection points
        intersections = new List<Vector3>();

        // temp vec to use
        Vector3 intersection = Vector3.zero;

        //check center of bounds
        if (PointInTriangle(_bounds.center, _triangleA, _triangleB, _triangleC, out intersection)) { intersections.Add(intersection);}

        //check corners of bounds
        if (PointInTriangle(b1, _triangleA, _triangleB, _triangleC, out intersection)) { intersections.Add(intersection); }
        if (PointInTriangle(b2, _triangleA, _triangleB, _triangleC, out intersection)) { intersections.Add(intersection); }
        if (PointInTriangle(b3, _triangleA, _triangleB, _triangleC, out intersection)) { intersections.Add(intersection); }
        if (PointInTriangle(b4, _triangleA, _triangleB, _triangleC, out intersection)) { intersections.Add(intersection); }

        //check edges of bounds
        if (LinesIntersect(b1, b2, _triangleA, _triangleB, out intersection)) { intersections.Add(intersection); }
        if (LinesIntersect(b2, b3, _triangleA, _triangleB, out intersection)) { intersections.Add(intersection); }
        if (LinesIntersect(b3, b4, _triangleA, _triangleB, out intersection)) { intersections.Add(intersection); }
        if (LinesIntersect(b4, b1, _triangleA, _triangleB, out intersection)) { intersections.Add(intersection); }

        if (LinesIntersect(b1, b2, _triangleA, _triangleC, out intersection)) { intersections.Add(intersection); }
        if (LinesIntersect(b2, b3, _triangleA, _triangleC, out intersection)) { intersections.Add(intersection); }
        if (LinesIntersect(b3, b4, _triangleA, _triangleC, out intersection)) { intersections.Add(intersection); }
        if (LinesIntersect(b4, b1, _triangleA, _triangleC, out intersection)) { intersections.Add(intersection); }

        if (LinesIntersect(b1, b2, _triangleB, _triangleC, out intersection)) { intersections.Add(intersection); }
        if (LinesIntersect(b2, b3, _triangleB, _triangleC, out intersection)) { intersections.Add(intersection); }
        if (LinesIntersect(b3, b4, _triangleB, _triangleC, out intersection)) { intersections.Add(intersection); }
        if (LinesIntersect(b4, b1, _triangleB, _triangleC, out intersection)) { intersections.Add(intersection); }

        if (intersections.Count > 0) {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Gets a list of corners of the given bounds.
    /// </summary>
    /// <param name="_bounds"></param>
    /// <returns></returns>
    public List<Vector3> GetCorners(Bounds _bounds){
        List<Vector3> corners = new List<Vector3>();
        corners.Add(_bounds.center + new Vector3(_bounds.extents.x, -_bounds.extents.y, _bounds.extents.z));
        corners.Add(_bounds.center + new Vector3(_bounds.extents.x, -_bounds.extents.y, -_bounds.extents.z));
        corners.Add(_bounds.center + new Vector3(-_bounds.extents.x, -_bounds.extents.y, _bounds.extents.z));
        corners.Add(_bounds.center + new Vector3(-_bounds.extents.x, -_bounds.extents.y, -_bounds.extents.z));
        corners.Add(_bounds.center + new Vector3(_bounds.extents.x, _bounds.extents.y, _bounds.extents.z));
        corners.Add(_bounds.center + new Vector3(_bounds.extents.x, _bounds.extents.y, -_bounds.extents.z));
        corners.Add(_bounds.center + new Vector3(-_bounds.extents.x, _bounds.extents.y, _bounds.extents.z));
        corners.Add(_bounds.center + new Vector3(-_bounds.extents.x, _bounds.extents.y, -_bounds.extents.z));
        return corners;
    }

    /// <summary>
    /// Tries to reload the weapon
    /// </summary>
    public bool TryStartReload(GameObject _owner)
    {
        if (!m_isReloading)
        {
            if (m_clipAmmo < m_clipSize && (m_spareAmmo > 0 || m_unlimitedAmmo))
            {
                m_isReloading = true;
                m_reloadTimer = m_reloadTime;

                if (m_splitReload){
                    // play reload sound:
                    if (m_startReloadSound != null)
                    {
                        GameObject sound = Instantiate(m_startReloadSound, _owner.transform.position, Quaternion.identity, _owner.transform);
                    }
                }
                else{
                    // play reload sound:
                    if (m_reloadSound != null)
                    {
                        GameObject sound = Instantiate(m_reloadSound, _owner.transform.position, Quaternion.identity, _owner.transform);
                    }
                    TryReload(_owner);
                }

                //Reload(_owner);
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Reloads the weapon. Uses wait timer
    /// </summary>
    public void TryReload(GameObject _owner)
    {
        m_isReloading = true;
        m_reloadTimer = m_reloadTime;

        // play reload sound:
        if (m_singleReloadSound != null)
        {
            GameObject sound = Instantiate(m_singleReloadSound, _owner.transform.position, Quaternion.identity, _owner.transform);
        }

        if (m_unlimitedAmmo){
            m_spareAmmo = int.MaxValue;
        }

        // reload clip:
        int ammoToReload = Mathf.Min(new int[] { m_clipSize - m_clipAmmo, m_spareAmmo, m_reloadAmount});
        m_spareAmmo = Mathf.Max(0, m_spareAmmo - ammoToReload);
        m_clipAmmo += ammoToReload;

        // clear temp ammo:
        m_tempAmmoStats.Clear();
    }

    public void TryEndReload(GameObject _owner)
    {
        if (!m_isReloading) return;
        
        m_isReloading = false;

        // play reload sound:
        if (m_endReloadSound != null)
        {
            GameObject sound = Instantiate(m_endReloadSound, _owner.transform.position, Quaternion.identity, _owner.transform);
        }
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
        float calcedAimTime = StatsManager.CalculateRangedAimTime(this, m_aimTime);
        float val = Mathf.Lerp(calcedUnaimedInaccuracy, calcedAimedInaccuracy, m_aimTimer / calcedAimTime);
        // if NaN or infinite, set 0
        if (float.IsNaN(val) || float.IsInfinity(val))
        {
            val = 0;
        }
        // ensure value is between m_unaimedInaccuracy and m_aimedInaccuracy
        val = Mathf.Clamp(val, Mathf.Min(calcedUnaimedInaccuracy, calcedAimedInaccuracy), Mathf.Max(calcedUnaimedInaccuracy, calcedAimedInaccuracy));
        return val;
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
            rangedWeapon.m_maxHitsPerShot = EditorGUILayout.IntField(new GUIContent("Max Hits", "Max amount of targets that can be hit per shot"), rangedWeapon.m_maxHitsPerShot);
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
            // horizontal falloff multiplier
            rangedWeapon.m_horizFalloffMult = EditorGUILayout.FloatField(new GUIContent("Horizontal Falloff Multiplier", "1 means 0 damage at side, 0 means no falloff"), rangedWeapon.m_horizFalloffMult);
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
            // spare ammo
            rangedWeapon.m_spareAmmo = EditorGUILayout.IntField(new GUIContent("Spare Ammo", "The amount of ammo currently in reserve"), rangedWeapon.m_spareAmmo);
            // max spare ammo
            rangedWeapon.m_maxSpareAmmo = EditorGUILayout.IntField(new GUIContent("Max Spare Ammo", "The max amount of ammo in reserve the player can carry"), rangedWeapon.m_maxSpareAmmo);
            // unlimited ammo
            rangedWeapon.m_unlimitedAmmo = EditorGUILayout.Toggle(new GUIContent("Unlimited Ammo", "If this weapon has unlimited ammo"), rangedWeapon.m_unlimitedAmmo);
            // // m_tempAmmoStats (list)
            // SerializedProperty tempAmmoStats = serializedObject.FindProperty("m_tempAmmoStats");
            // EditorGUILayout.PropertyField(tempAmmoStats, true);
            // serializedObject.ApplyModifiedProperties();

            // reloading:
            GUILayout.Label("Reloading", CustomEditorStuff.center_bold_label);
            // split Reload
            rangedWeapon.m_splitReload = EditorGUILayout.Toggle(new GUIContent("Split Reload", "If the reload is split into multiple parts"), rangedWeapon.m_splitReload);
            // reload amount
            rangedWeapon.m_reloadAmount = EditorGUILayout.IntField(new GUIContent("Reload Amount", "The amount of ammo to reload"), rangedWeapon.m_reloadAmount);
            //EditorGUI.BeginDisabledGroup(rangedWeapon.m_splitReload);
            // reload time
            rangedWeapon.m_reloadTime = EditorGUILayout.FloatField(new GUIContent("Reload Time", "The time it takes to reload"), rangedWeapon.m_reloadTime);
            //EditorGUI.EndDisabledGroup();

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
            // splash effect
            rangedWeapon.m_splashEffect = (GameObject)EditorGUILayout.ObjectField(new GUIContent("Splash Effect", "The effect to play when splash damage is dealt"), rangedWeapon.m_splashEffect, typeof(GameObject), false);

            // sounds:
            GUILayout.Label("Sounds", CustomEditorStuff.center_bold_label);
            // shoot sound
            rangedWeapon.m_shootSound = (GameObject)EditorGUILayout.ObjectField(new GUIContent("Shoot Sound", "The sound to play when shooting"), rangedWeapon.m_shootSound, typeof(GameObject), false);
            // empty sound
            rangedWeapon.m_emptySound = (GameObject)EditorGUILayout.ObjectField(new GUIContent("Empty Sound", "The sound to play when out of ammo"), rangedWeapon.m_emptySound, typeof(GameObject), false);
            //disabled group
            EditorGUI.BeginDisabledGroup(rangedWeapon.m_splitReload);
            // reload sound
            rangedWeapon.m_reloadSound = (GameObject)EditorGUILayout.ObjectField(new GUIContent("Reload Sound", "The sound to play when reloading"), rangedWeapon.m_reloadSound, typeof(GameObject), false);
            EditorGUI.EndDisabledGroup();
            //disabled group
            EditorGUI.BeginDisabledGroup(!rangedWeapon.m_splitReload);
            // start reload sound
            rangedWeapon.m_startReloadSound = (GameObject)EditorGUILayout.ObjectField(new GUIContent("Start Reload Sound", "The sound to play when starting to reload"), rangedWeapon.m_startReloadSound, typeof(GameObject), false);
            // single reload sound
            rangedWeapon.m_singleReloadSound = (GameObject)EditorGUILayout.ObjectField(new GUIContent("Single Reload Sound", "The sound to play when reloading"), rangedWeapon.m_singleReloadSound, typeof(GameObject), false);
            // end reload sound
            rangedWeapon.m_endReloadSound = (GameObject)EditorGUILayout.ObjectField(new GUIContent("End Reload Sound", "The sound to play when ending reload"), rangedWeapon.m_endReloadSound, typeof(GameObject), false);
            EditorGUI.EndDisabledGroup();
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