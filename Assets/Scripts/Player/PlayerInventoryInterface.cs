using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEngine.InputSystem;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Interacts with the player's inventory, item database, and functionality of items.
/// @todo Make sure that ammo is replenished when the player returns to the town.
/// </summary>
[Serializable]
public class PlayerInventoryInterface : MonoBehaviour
{
    [Header("Input")]
    // Input actions:
    // swap weapon
    public InputAction swapWeaponAction;
    // reload
    public InputAction reloadAction;
    // fire weapon
    public InputAction fireWeaponAction;
    // aim weapon
    public InputAction aimWeaponAction;
    public InputAction useEquipmentAction;
    // melee attack
    public InputAction meleeAttackAction;

    [Header("Inventory")]
    public string playerInventoryName = "player";
    public Inventory playerInventory;

    public enum SelectedWeaponType{ Primary, Secondary, None }
    [SerializeField] public SelectedWeaponType selectedWeaponType = SelectedWeaponType.None;
    public Item selectedWeapon;
    public Item selectedEquipment;

    [Header("Melee")]
    public float meleeAttackDamage = 20.0f;
    public float meleeAttackRange = 2.0f;
    public float meleeAttackSize = 2.0f;
    public float meleeAttackCooldown = 0.5f;
    private float meleeAttackTimer = 0.0f;

    [Serializable]
    public class WeaponModelLink
    {
        public Item weapon;
        public GameObject model;
        public Transform weaponFirepoint;
        public string animatorBoolName = "";
    }


    [Header("References")]  
    public List<WeaponModelLink> weaponModelLinks = new List<WeaponModelLink>();

    public AimZone m_aimZone;

    public Animator playerAnimator;

    private void OnDrawGizmos() {
        // get equipped weapon
        RangedWeapon rangedWeapon = selectedWeapon as RangedWeapon;
        if (rangedWeapon) {
            //draw m_rHits
            Gizmos.color = Color.red;
            foreach (RangedWeapon.ShotInfo hit in rangedWeapon.m_allShots) {
                Gizmos.DrawLine(hit.originPoint, hit.hitPoint);

                //draw damage dealt in text
                #if UNITY_EDITOR
                Handles.Label(hit.hitPoint, hit.damage.ToString());
                #endif
            }
        }

        // draw melee attack zone
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position + (transform.forward * meleeAttackRange), meleeAttackSize);
    }


    // Start is called before the first frame update
    void Start()
    {
        playerInventory = InventoryManager.instance?.GetInventory(playerInventoryName);

        SwapWeapon();
        // select weapon
        SelectWeapon(selectedWeaponType);

        // select equipment
        SelectEquipment();

        // aimLines.startColor = aimLineStartColor;
        // aimLines.endColor = aimLineEndColor;
    }
    

    // Update is called once per frame
    void Update()
    {
        // if swap weapon action is pressed, swap weapon
        if (swapWeaponAction.triggered)
        {
            SwapWeapon();
        }

        if (selectedWeapon){
            selectedWeapon.ManualUpdate(gameObject);

            Transform weaponFirepoint = GetWeaponFirepoint(selectedWeapon);
            Vector3 weaponFirepointPosition = weaponFirepoint.position;
            Vector3 fireDirection = GetComponent<PlayerMovement>().GetMouseAimPlanePoint() - weaponFirepointPosition;

            RangedWeapon rangedWeapon = selectedWeapon as RangedWeapon;
            if (rangedWeapon)
            {
                // if fire weapon action is pressed and is not auto and is not shooting, shoot weapon || if fire weapon action is current down and is auto, shoot weapon
                if ((fireWeaponAction.triggered && !rangedWeapon.m_isAutomnatic) || (fireWeaponAction.ReadValue<float>() > 0 && rangedWeapon.m_isAutomnatic))
                {
                    if (rangedWeapon.TryShoot(weaponFirepointPosition, fireDirection, gameObject, m_aimZone)){
                        // play shoot animation
                        playerAnimator.SetTrigger("Shoot");
                    }
                }

                // if reload action is pressed, reload weapon
                if (reloadAction.triggered)
                {
                    if (rangedWeapon.TryReload(gameObject)){
                        playerAnimator.SetTrigger("Reload");
                    }
                }

                // if aim weapon action is down, aim weapon
                if (aimWeaponAction.ReadValue<float>() > 0)
                {
                    rangedWeapon.TrySetAim(true, gameObject);

                    if (meleeAttackAction.triggered)
                    {
                        TryMeleeAttack();
                    }
                }
                else
                {
                    rangedWeapon.TrySetAim(false, gameObject);
                }

                // stop aiming
                if (swapWeaponAction.triggered)
                {
                    rangedWeapon.TrySetAim(false, gameObject);
                }
            }

            UpdateAimZone();
        }

        if (useEquipmentAction.triggered && selectedEquipment == null)
        {
            SelectEquipment();
        }
        if (selectedEquipment){
            selectedEquipment.ManualUpdate(gameObject);

            if (useEquipmentAction.triggered)
            {
                Equipment equipment = selectedEquipment as Equipment;
                if (equipment)
                {
                    Vector3 spawnDirection = (transform.forward).normalized;

                    Transform throwPoint = GetWeaponFirepoint(selectedEquipment);

                    equipment.TossPrefab(throwPoint, spawnDirection * 2.5f, gameObject);

                    string animatorName = GetWeaponAnimatorBoolName(selectedEquipment);
                    playerAnimator.SetTrigger(animatorName);
                }
            }
        }

        //update melee attack timer
        if (meleeAttackTimer > 0.0f)
        {
            meleeAttackTimer -= Time.deltaTime;
        }
    }

    private void TryMeleeAttack()
    {
        if (meleeAttackTimer > 0.0f)
        {
            return;
        }
        meleeAttackTimer = meleeAttackCooldown;

        // play melee attack clip
        playerAnimator.SetTrigger("Melee");

        // do sphere cast to find enemies in range
        Collider[] hitColliders = Physics.OverlapSphere(transform.position + transform.forward * meleeAttackRange, meleeAttackSize);
        List<Health_Base> hitEnemies = new List<Health_Base>();
        foreach (Collider hitCollider in hitColliders)
        {
            // if hit collider is an enemy, deal damage
            Health_Base enemy = hitCollider.GetComponentInParent<Health_Base>();
            if (enemy)
            {
                // check if already hit this enemy
                if (hitEnemies.Contains(enemy))
                {
                    continue;
                }
                hitEnemies.Add(enemy);

                Health_Base.DamageStat ds = new Health_Base.DamageStat(meleeAttackDamage, gameObject, transform.position, transform.position + transform.forward * meleeAttackRange);
                enemy.TakeDamage(ds);

                RangedWeapon rangedWeapon = selectedWeapon as RangedWeapon;
                if (rangedWeapon == null) continue;

                // hit effect
                if (rangedWeapon.m_hitEffect != null)
                {
                    Destroy(Instantiate(
                    rangedWeapon.m_hitEffect, ds.m_hitPoint,
                    Quaternion.FromToRotation(Vector3.up, ds.m_hitPoint - ds.m_originPoint)),
                    2.0f);
                }
            }
        }
    }

    private void UpdateAimZone()
    {
        if (m_aimZone)
        {
            if (!selectedWeapon) return;

            RangedWeapon rangedWeapon = selectedWeapon as RangedWeapon;
            if (!rangedWeapon) return;

            PlayerMovement playerMovement = GetComponent<PlayerMovement>();
            if (!playerMovement) return;

            if (!rangedWeapon.m_isAiming || rangedWeapon.m_reloadTimer > 0 || playerMovement.isRolling){
                m_aimZone.Hide();
            }
            else{
                m_aimZone.Show();

                m_aimZone.SetCorners(CalculateAimZoneCorners());

                m_aimZone.UpdateVisuals(rangedWeapon.m_horizFalloffMult);
            }
        }
    }

    /// <summary>
    /// Calculates the corners of the aim zone using the weapon's range and accuracy.
    /// </summary>
    /// <returns></returns>
    public AimZone.Corners CalculateAimZoneCorners()
    {
        AimZone.Corners corners = AimZone.Corners.Zero;

        if (m_aimZone == null) return corners;

        RangedWeapon rangedWeapon = selectedWeapon as RangedWeapon;
        if (!rangedWeapon) return corners;

        PlayerMovement playerMovement = GetComponent<PlayerMovement>();
        if (!playerMovement) return corners;
        
        Vector3 aimPoint = playerMovement.GetMouseAimPlanePoint();

        float currentAimAngle = rangedWeapon.CalcCurrentAimAngle();
        // if nan or inf, set to 0
        if (float.IsNaN(currentAimAngle) || float.IsInfinity(currentAimAngle)) currentAimAngle = 0;

        corners.backLeft = m_aimZone.transform.position + m_aimZone.transform.right * -0.0f + m_aimZone.transform.forward * 0.1f;
        corners.backRight = m_aimZone.transform.position + m_aimZone.transform.right * 0.0f + m_aimZone.transform.forward * 0.1f;

        float distFromBackLeftToAimPoint = Vector3.Distance(corners.backLeft, new Vector3(aimPoint.x, corners.backLeft.y, aimPoint.z));
        float distFromBackRightToAimPoint = Vector3.Distance(corners.backRight, new Vector3(aimPoint.x, corners.backRight.y, aimPoint.z));

        corners.frontLeft = Quaternion.Euler(0, -currentAimAngle, 0) * (corners.backLeft + (m_aimZone.transform.forward * rangedWeapon.m_range) - corners.backLeft) + corners.backLeft;
        corners.frontRight = Quaternion.Euler(0, currentAimAngle, 0) * (corners.backRight + (m_aimZone.transform.forward * rangedWeapon.m_range) - corners.backRight) + corners.backRight;
        corners.frontLeft.y = m_aimZone.transform.position.y;
        corners.frontRight.y = m_aimZone.transform.position.y;

        return corners;
    }

    public GameObject GetWeaponModel(Item weapon){
        if (!weapon) return null;
        foreach (WeaponModelLink link in weaponModelLinks)
        {
            if (link.weapon.id == weapon.id)
            {
                return link.model;
            }
        }
        return null;
    }

    public Transform GetWeaponFirepoint(Item weapon)
    {
        if (!weapon) return null;
        foreach (WeaponModelLink link in weaponModelLinks)
        {
            if (link.weapon.id == weapon.id)
            {
                return link.weaponFirepoint;
            }
        }
        return null;
    }

    public string GetWeaponAnimatorBoolName(Item weapon)
    {
        foreach (WeaponModelLink link in weaponModelLinks)
        {
            if (link.weapon.id == weapon.id)
            {
                return link.animatorBoolName;
            }
        }
        return "";
    }

    public void SwapWeapon(){
        // if inventory is null, return
        if (playerInventory == null) return;

        // toggle between primary and secondary weapon
        if (selectedWeaponType == SelectedWeaponType.Primary)
        {
            SelectWeapon(SelectedWeaponType.Secondary);
        } 
        else if (selectedWeaponType == SelectedWeaponType.Secondary)
        {
            SelectWeapon(SelectedWeaponType.Primary);
        }

        // if selected weapon is null, try to select primary, secondary, then none
        if (selectedWeapon == null){
            SelectWeapon(SelectedWeaponType.Primary);
            if (selectedWeapon == null){
                SelectWeapon(SelectedWeaponType.Secondary);
                if (selectedWeapon == null){
                    SelectWeapon(SelectedWeaponType.None);
                }
            }
        }
    }

    public void SelectWeapon(SelectedWeaponType _type){
        selectedWeaponType = _type;

        // if inventory is null, return
        if (playerInventory == null) return;

        // get selected weapon
        selectedWeapon = playerInventory.slots.ElementAtOrDefault((int)selectedWeaponType)?.item;

        // if selected weapon is null, return
        if (selectedWeapon == null) return;

        // set animator
        string animatorBoolName = GetWeaponAnimatorBoolName(selectedWeapon);
        if (playerAnimator && animatorBoolName != "")
        {
            DisableAllAnimatorWeapons();
            playerAnimator.SetBool(animatorBoolName, true);
        }

        // enable weapon model
        DisableAllWeaponModels();
        GameObject weaponModel = GetWeaponModel(selectedWeapon);
        if (weaponModel)
        {
            weaponModel.SetActive(true);
        }
    }

    public void SelectEquipment(){
        // if inventory is null, return
        if (playerInventory == null) return;

        // get selected equipment
        selectedEquipment = playerInventory.slots.ElementAtOrDefault(2)?.item;

        // if selected equipment is null, return
        if (selectedEquipment == null) return;
    }

    public void DisableAllWeaponModels(){
        foreach (WeaponModelLink link in weaponModelLinks)
        {
            if (link.model)
            {
                link.model.SetActive(false);
            }
        }
    }

    public void DisableAllAnimatorWeapons(){
        if (playerAnimator == null) return;

        foreach (WeaponModelLink link in weaponModelLinks)
        {
            string animatorBoolName = link.animatorBoolName;
            if (animatorBoolName != "")
            {
                playerAnimator.SetBool(animatorBoolName, false);
            }
        }
    }

    private void OnEnable() {
        EnableInput();
    }

    private void OnDisable() {
        DisableInput();
    }

    public void EnableInput()
    {
        swapWeaponAction.Enable();
        reloadAction.Enable();
        fireWeaponAction.Enable();
        aimWeaponAction.Enable();
        useEquipmentAction.Enable();
        meleeAttackAction.Enable();
    }

    public void DisableInput()
    {
        swapWeaponAction.Disable();
        reloadAction.Disable();
        fireWeaponAction.Disable();
        aimWeaponAction.Disable();
        useEquipmentAction.Disable();
        meleeAttackAction.Disable();
    }
}
