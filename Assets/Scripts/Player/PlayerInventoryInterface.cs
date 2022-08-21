using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEngine.InputSystem;
using System.IO;

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
    public InputAction swapWeaponAction; ///< The action to swap weapons.
    // reload
    public InputAction reloadAction; ///< The action to reload.
    // fire weapon
    public InputAction fireWeaponAction; ///< The action to fire the weapon.
    // aim weapon
    public InputAction aimWeaponAction; ///< The action to aim the weapon.
    public InputAction useEquipmentAction; ///< The action to use an equipment.
    // melee attack
    public InputAction meleeAttackAction; ///< The action to melee attack.

    [Header("Inventory")]
    public string playerInventoryName = "player"; ///< The name of the player's inventory.
    public Inventory playerInventory; ///< The player's inventory.

    /// <summary>
    ///  Enum describing different weapon types in respect to what inventory slot they may be in.
    /// </summary>
    public enum SelectedWeaponType
    {
        Primary, ///< Primary weapons which may be equipped in the primary slot.
        Secondary, ///< Secondary weapons which may be equipped in the secondary slot.
        None ///< No weapon is selected.
    }
    [SerializeField] public SelectedWeaponType selectedWeaponType = SelectedWeaponType.None; ///< The currently selected weapon type.
    public Item selectedWeapon; ///< The currently selected weapon.
    public Item selectedEquipment; ///< The currently selected equipment.

    public float m_equipmentDelay = 0.75f; ///< The delay between using an equipment and the next use.
    private float m_equipmentDelayTimer = 0.0f; ///< The timer for the delay between using an equipment and the next use.

    [Header("Melee")]
    public float meleeAttackDamage = 20.0f; ///< The damage of the melee attack.
    public float meleeAttackRange = 2.0f; ///< The range of the melee attack.
    public float meleeAttackSize = 2.0f; ///< The size of the melee attack.
    public float meleeAttackCooldown = 0.5f; ///< The cooldown of the melee attack.
    private float meleeAttackTimer = 0.0f; ///< The timer of the melee attack.

    [Serializable]
    /// <summary>
    ///  Wrapper class grouping abstract weapon data with the weapons model.
    /// </summary>
    public class WeaponModelLink
    {
        public Item weapon; ///< The weapon.
        public GameObject model; ///< The model of the weapon.
        public Transform weaponFirepoint; ///< The weapon's firepoint: the point where the weapon is fired from.
        public string animatorBoolName = ""; ///< The name of the animator bool to set when the weapon is fired.
    }


    [Header("References")]
    public List<WeaponModelLink> weaponModelLinks = new List<WeaponModelLink>(); ///< The list of weapon model links.

    public AimZone m_aimZone; ///< The aim zone.

    public Animator playerAnimator; ///< The player's animator.

    /// <summary>
    ///  Draw Gizmos.
    /// </summary>
    private void OnDrawGizmos()
    {
        // get equipped weapon
        RangedWeapon rangedWeapon = selectedWeapon as RangedWeapon;
        if (rangedWeapon)
        {
            //draw m_rHits
            Gizmos.color = Color.red;
            foreach (RangedWeapon.ShotInfo hit in rangedWeapon.m_allShots)
            {
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

    public static string GetSaveFolderPath(int _saveSlot)
    {
        return SaveManager.GetSaveFolderPath(_saveSlot) + "/inventories/";
    }

    public static string GetSaveFilePath(int saveSlot)
    {
        return GetSaveFolderPath(saveSlot) + "interface.json";
    }

    /// <summary>
    /// Saves the interface to a file.
    /// </summary>
    /// <param name="_saveSlot"></param>
    public void SaveInterface(int _saveSlot){
        // if the save folder doesn't exist, create it
        if (!Directory.Exists(GetSaveFolderPath(_saveSlot)))
        {
            Directory.CreateDirectory(GetSaveFolderPath(_saveSlot));
        }

        FileStream file = File.Create(GetSaveFilePath(_saveSlot));

        // save the selected weapon type
        int selectedWeaponTypeInt = (int)selectedWeaponType;

        StreamWriter writer = new StreamWriter(file);

        writer.WriteLine(selectedWeaponTypeInt);

        writer.Close();

        file.Close();
    }

    /// <summary>
    /// Tries to laod the interface from the save file.
    /// </summary>
    /// <param name="_saveSlot"></param>
    public void LoadInterface(int _saveSlot){
        // if save path doesn't exist, create it
        if (!Directory.Exists(GetSaveFolderPath(_saveSlot)))
        {
            Directory.CreateDirectory(GetSaveFolderPath(_saveSlot));
        }
        // if save file doesn't exist, return
        if (!File.Exists(GetSaveFilePath(_saveSlot)))
        {
            Debug.Log("Save file does not exist.");
            return;
        }

        FileStream file = File.Open(GetSaveFilePath(_saveSlot), FileMode.Open);

        // read the selected weapon type
        StreamReader reader = new StreamReader(file);
        string line = reader.ReadLine();
        int selectedWeaponTypeInt = int.Parse(line);
        reader.Close();
        file.Close();

        // set the selected weapon type
        selectedWeaponType = (SelectedWeaponType)selectedWeaponTypeInt;
    }

    // Start is called before the first frame update
    void Start()
    {
        // get player inventory
        playerInventory = InventoryManager.instance?.GetInventory(playerInventoryName);
        if (playerInventory == null)
        {
            Debug.LogError("Player inventory not found!");
        }

        // LevelController.GamePaused += () => {
        //     DisableInput();
        // };
        // LevelController.GameUnpaused += () => {
        //     EnableInput();
        // };

        LoadInterface(SaveManager.currentSaveSlot);

        // select weapon
        SelectWeapon(selectedWeaponType);

        // ensure player has valid weapon selected
        TrySelectValidWeapon();

        // select equipment
        SelectEquipment();
    }


    // Update is called once per frame
    void Update()
    {
        if (LevelController.IsPaused || ToggleableTownWindow.AnyWindowOpen()) {
            DisableInput();
        }
        else {
            EnableInput();
        }

        // if swap weapon action is pressed, swap weapon
        if (swapWeaponAction.triggered)
        {
            SwapWeapon();
        }

        if (selectedWeapon)
        {
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
                    if (rangedWeapon.TryShoot(weaponFirepointPosition, fireDirection, gameObject, m_aimZone))
                    {
                        // play shoot animation
                        playerAnimator.SetTrigger("Shoot");
                    }
                }

                // if reload action is pressed, reload weapon
                if (reloadAction.triggered)
                {
                    if (rangedWeapon.TryReload(gameObject))
                    {
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

        if (useEquipmentAction.triggered)
        {
            SelectEquipment();
        }
        if (selectedEquipment)
        {
            selectedEquipment.ManualUpdate(gameObject);

            if (useEquipmentAction.triggered && m_equipmentDelayTimer <= 0.0f)
            {
                Equipment equipment = selectedEquipment as Equipment;
                if (equipment && equipment.currentStackSize > 0)
                {
                    m_equipmentDelayTimer = m_equipmentDelay;

                    Vector3 spawnDirection = (transform.forward).normalized;

                    Transform throwPoint = GetWeaponFirepoint(selectedEquipment);

                    equipment.TossPrefab(throwPoint, spawnDirection, gameObject);

                    string animatorName = GetWeaponAnimatorBoolName(selectedEquipment);
                    playerAnimator.SetTrigger(animatorName);

                    // make player look at cursor
                    PlayerMovement playerMovement = GetComponent<PlayerMovement>();
                    playerMovement.SetLookDirection(playerMovement.GetMouseAimPlanePoint() - transform.position);

                    equipment.currentStackSize -= 1;
                }
            }
        }

        // update equipment delay timer
        if (m_equipmentDelayTimer > 0.0f)
        {
            m_equipmentDelayTimer -= Time.deltaTime;
        }

        //update melee attack timer
        if (meleeAttackTimer > 0.0f)
        {
            meleeAttackTimer -= Time.deltaTime;
        }
        
        if (meleeAttackTimer < 0.5f && GetMeleeLink().model.activeSelf)
        {
            // disable melee model
            GetMeleeLink().model.SetActive(false);
            // enable equiped wepaon model
            SelectWeapon(selectedWeaponType);
        }
    }

    /// <summary>
    ///  Attempts to melee attack.
    ///  @todo move this to a separate script on the melee
    /// </summary>
    private void TryMeleeAttack()
    {
        if (meleeAttackTimer > 0.0f)
        {
            return;
        }
        meleeAttackTimer = meleeAttackCooldown;

        // play melee attack clip
        playerAnimator.SetTrigger(GetMeleeLink().animatorBoolName);

        // enable model
        DisableAllWeaponModels();
        GetMeleeLink().model.SetActive(true);

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

    /// <summary>
    ///  Updates the position and shape of the Aim Zone.
    /// </summary>
    /// @todo Fix interaction with snow material.
    private void UpdateAimZone()
    {
        if (m_aimZone)
        {
            if (!selectedWeapon) return;

            RangedWeapon rangedWeapon = selectedWeapon as RangedWeapon;
            if (!rangedWeapon) return;

            PlayerMovement playerMovement = GetComponent<PlayerMovement>();
            if (!playerMovement) return;

            if (!rangedWeapon.m_isAiming || rangedWeapon.m_reloadTimer > 0 || playerMovement.isRolling)
            {
                m_aimZone.Hide();
            }
            else
            {
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


    /// <summary>
    /// If the weapon exists, return the weapon model.
    /// </summary>
    /// <param name="Item">The weapon item that you want to get the model for.</param>
    /// <returns>
    /// The weapon model.
    /// </returns>
    public GameObject GetWeaponModel(Item weapon)
    {
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

    /// <summary>
    /// If the weapon exists, return the weapon's firepoint.
    /// </summary>
    /// <param name="Item">The weapon item that you want to get the firepoint for.</param>
    /// <returns>
    /// The weapon firepoint of the weapon that is being passed in.
    /// </returns>
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

    /// <summary>
    /// It returns the animator bool name of the weapon model link that has the same id as the weapon
    /// passed in
    /// </summary>
    /// <param name="Item">The weapon item that you want to get the animator bool name for.</param>
    /// <returns>
    /// The animator bool name of the weapon.
    /// </returns>
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

    public WeaponModelLink GetMeleeLink(){
        // find the link with "Melee" as the animator bool name
        foreach (WeaponModelLink link in weaponModelLinks)
        {
            if (link.animatorBoolName == "Melee")
            {
                return link;
            }
        }
        return null;
    }

    /// <summary>
    /// If the player has a primary weapon, select the secondary weapon. If the player has a secondary
    /// weapon, select the primary weapon. If the player has no weapons, select no weapon
    /// </summary>
    /// <returns>
    /// The method is returning the selected weapon type.
    /// </returns>
    public void SwapWeapon()
    {
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

        TrySelectValidWeapon();
    }

    /// <summary>
    /// If selected weapon is null, try to select primary, secondary, then none
    /// </summary>
    private void TrySelectValidWeapon()
    {
        if (selectedWeapon == null)
        {
            SelectWeapon(SelectedWeaponType.Primary);
            if (selectedWeapon == null)
            {
                SelectWeapon(SelectedWeaponType.Secondary);
                if (selectedWeapon == null)
                {
                    SelectWeapon(SelectedWeaponType.None);
                }
            }
        }
    }

    /// <summary>
    /// It selects a weapon from the inventory and enables the weapon model
    /// </summary>
    /// <param name="_type">The weapon type that you want to select.</param>
    /// <returns>
    /// The weapon that is selected.
    /// </returns>
    public void SelectWeapon(SelectedWeaponType _type)
    {
        selectedWeaponType = _type;

        SaveInterface(SaveManager.currentSaveSlot);

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


    /// <summary>
    /// If the player's inventory is not null, and the third slot in the inventory is not null, then set
    /// the selected equipment to the item in the third slot
    /// </summary>
    /// <returns>
    /// The selected equipment is being returned.
    /// </returns>
    public void SelectEquipment()
    {
        // if inventory is null, return
        if (playerInventory == null) return;

        // get selected equipment
        selectedEquipment = playerInventory.slots.ElementAtOrDefault(2)?.item;
        if (selectedEquipment && selectedEquipment.currentStackSize <= 0)
        {
            selectedEquipment = null;
        }

        // if selected equipment is null, return
        if (selectedEquipment == null) return;
    }


    /// <summary>
    /// Disable all weapon models
    /// </summary>
    public void DisableAllWeaponModels()
    {
        foreach (WeaponModelLink link in weaponModelLinks)
        {
            if (link.model)
            {
                link.model.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Disable all the weapons in the animator
    /// </summary>
    public void DisableAllAnimatorWeapons()
    {
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
    /// <summary>
    ///  Enables input when this object is enabled
    /// </summary>
    private void OnEnable()
    {
        EnableInput();
    }
    /// <summary>
    /// Disables input when this object is disabled.
    /// </summary>
    private void OnDisable()
    {
        DisableInput();
    }

    /// <summary>
    ///  Enables input actions.
    /// </summary>
    public void EnableInput()
    {
        swapWeaponAction.Enable();
        reloadAction.Enable();
        fireWeaponAction.Enable();
        aimWeaponAction.Enable();
        useEquipmentAction.Enable();
        meleeAttackAction.Enable();
    }

    /// <summary>
    /// Disables input actions.
    /// </summary>
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
