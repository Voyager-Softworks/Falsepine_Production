using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEngine.InputSystem;
using System.IO;
using UnityEngine.Serialization;

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
    [FormerlySerializedAs("useEquipmentAction")]
    public InputAction useEquipmentAction_1; ///< The action to use an equipment.
    public InputAction useEquipmentAction_2; ///< The action to use an equipment.
    // melee attack
    public InputAction meleeAttackAction; ///< The action to melee attack.
    [ReadOnly] public ItemThrow m_currentlyThrowingItem; ///< The item that is currently being thrown.

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
    public Item selectedMeleeWeapon; ///< The currently selected melee weapon.

    [Serializable]
    /// <summary>
    ///  Wrapper class grouping abstract weapon data with the weapons model.
    /// </summary>
    public class WeaponModelLink
    {
        public List<Item> weapons;
        public GameObject model; ///< The model of the weapon.
        public Transform weaponFirepoint; ///< The weapon's firepoint: the point where the weapon is fired from.
        public string animatorBoolName = ""; ///< The name of the animator bool to set when the weapon is fired.

        public AnimationClip startReloadClip; ///< The animation clip to play when the weapon is reloaded.
        public AnimationClip singleReloadClip; ///< The animation clip to play when the weapon is reloaded.
        public AnimationClip endReloadClip; ///< The animation clip to play when the weapon is reloaded.
    }


    [Header("References")]
    public List<WeaponModelLink> weaponModelLinks = new List<WeaponModelLink>(); ///< The list of weapon model links.

    public AimZone m_aimZone; ///< The aim zone.

    public Animator playerAnimator; ///< The player's animator.

    [Header("Events")]
    public System.Action OnPrimaryUsed; ///< The event to call when the primary weapon is used.
    public System.Action OnSecondaryUsed; ///< The event to call when the secondary weapon is used.
    public System.Action OnEquipmentUsed; ///< The event to call when the equipment is used.
    public System.Action OnMeleeUsed; ///< The event to call when the melee weapon is used.
    public System.Action OnReload; ///< The event to call when the player reloads.

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
        MeleeWeapon meleeWeapon = selectedMeleeWeapon as MeleeWeapon;
        if (meleeWeapon)
        {
            Gizmos.color = Color.red;

            Transform meleeWeaponTip = GetWeaponFirepoint(selectedMeleeWeapon);
            Vector3 meleeWeaponTipPosition = meleeWeaponTip.position;
            float rad = meleeWeapon.m_radius;
            float height = meleeWeapon.m_height;

            // draw spheres along the height at each radius
            for (float i = -height/2.0f; i < height/2.0f; i+=rad)
            {
                Gizmos.DrawWireSphere(meleeWeaponTipPosition + new Vector3(0, i, 0), rad);
            }
        }
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
        SelectEquipment(1);

        // select melee weapon
        SelectMeleeWeapon();
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

        if (selectedWeapon == null){
            TrySelectValidWeapon(false);
        }

        if (selectedWeapon)
        {
            // if the currwen weapon is not in the inventory, select a valid weapon
            if (playerInventory.GetItemIndex(selectedWeapon) == -1)
            {
                SwapWeapon();
            }

            selectedWeapon?.ManualUpdate(gameObject);

            Transform weaponFirepoint = GetWeaponFirepoint(selectedWeapon);
            Vector3 weaponFirepointPosition = weaponFirepoint.position;
            Vector3 fireDirection = GetComponent<PlayerMovement>().GetMouseAimPlanePoint() - weaponFirepointPosition;

            RangedWeapon rangedWeapon = selectedWeapon as RangedWeapon;
            if (rangedWeapon)
            {
                // if fire weapon action is pressed and is not auto and is not shooting, shoot weapon || if fire weapon action is current down and is auto, shoot weapon
                if ((fireWeaponAction.triggered && !rangedWeapon.m_isAutomnatic) || (fireWeaponAction.ReadValue<float>() > 0 && rangedWeapon.m_isAutomnatic))
                {
                    // DoReload false
                    playerAnimator.SetBool("DoReload", false);

                    if (rangedWeapon.TryShoot(weaponFirepoint, fireDirection, gameObject, m_aimZone))
                    {
                        // play shoot animation
                        playerAnimator.SetTrigger("Shoot");
                        // DoReload false
                        playerAnimator.SetBool("DoReload", false);

                        playerAnimator.SetLayerWeight(2, 1);

                        // event (primary or secondary)
                        if (selectedWeaponType == SelectedWeaponType.Primary)
                        {
                            OnPrimaryUsed?.Invoke();
                        }
                        else if (selectedWeaponType == SelectedWeaponType.Secondary)
                        {
                            OnSecondaryUsed?.Invoke();
                        }
                    }
                }

                // if reload action is pressed, reload weapon
                if (reloadAction.triggered)
                {
                    if (rangedWeapon.TryStartReload(gameObject))
                    {
                        // Reload anim
                        playerAnimator.SetTrigger("Reload");
                        playerAnimator.SetBool("DoReload", true);

                        playerAnimator.SetLayerWeight(2, 1);

                        // event
                        OnReload?.Invoke();
                    }
                }

                // if weapon is still reloading, play reload animation
                if (rangedWeapon.m_clipAmmo >= rangedWeapon.m_clipSize || rangedWeapon.m_spareAmmo <= 0)
                {
                    playerAnimator.SetBool("DoReload", false);
                }

                // if (rangedWeapon.m_isReloading && !playerAnimator.GetBool("DoReload"))
                // {
                //     rangedWeapon.TryEndReload(gameObject);
                // }

                // if aim weapon action is down, aim weapon
                if (aimWeaponAction.ReadValue<float>() > 0)
                {
                    rangedWeapon.TrySetAim(true, gameObject);

                    playerAnimator.SetLayerWeight(2, 1);
                }
                else
                {
                    rangedWeapon.TrySetAim(false, gameObject);
                }

                // stop aiming
                if (swapWeaponAction.triggered)
                {
                    rangedWeapon.TrySetAim(false, gameObject);

                    playerAnimator.SetLayerWeight(2, 1);
                }
            }

            UpdateAimZone();
        }

        // use equipment_1
        if (useEquipmentAction_1.triggered)
        {
            SelectEquipment(1);
        }
        // use equipment_2
        if (useEquipmentAction_2.triggered)
        {
            SelectEquipment(2);
        }
        if (selectedEquipment)
        {
            selectedEquipment.ManualUpdate(gameObject);

            if ((useEquipmentAction_1.triggered || useEquipmentAction_2.triggered))
            {
                Equipment equipment = selectedEquipment as Equipment;
                if (equipment && equipment.currentStackSize > 0 && equipment.m_useDelayTimer <= 0)
                {
                    Vector3 spawnDirection = (transform.forward).normalized;

                    Transform throwPoint = GetWeaponFirepoint(selectedEquipment);
                    if (throwPoint == null)
                    {
                        throwPoint = transform;
                    }

                    if (equipment.UseEquipment(throwPoint, spawnDirection, gameObject)){
                        string animatorName = GetWeaponAnimatorBoolName(selectedEquipment);
                        if (animatorName != "") playerAnimator.SetTrigger(animatorName);

                        // event
                        OnEquipmentUsed?.Invoke();
                    }
                }
            }
        }

        // melee
        if (meleeAttackAction.triggered)
        {
            SelectMeleeWeapon();

            if (selectedMeleeWeapon)
            {
                selectedMeleeWeapon.ManualUpdate(gameObject);

                Transform meleeWeaponTip = GetWeaponFirepoint(selectedMeleeWeapon);
                Vector3 meleeWeaponTipPosition = meleeWeaponTip.position;

                MeleeWeapon meleeWeapon = selectedMeleeWeapon as MeleeWeapon;
                if (meleeWeapon.TryMelee(meleeWeaponTip, gameObject))
                {
                    // enable model
                    DisableAllWeaponModels();
                    GameObject weaponModel = GetWeaponModel(selectedMeleeWeapon);
                    if (weaponModel)
                    {
                        weaponModel.SetActive(true);
                    }

                    // set animator
                    string animatorBoolName = GetWeaponAnimatorBoolName(selectedMeleeWeapon);
                    if (meleeWeapon.m_shouldDoComboSwing) {
                        animatorBoolName += "_Combo";
                    }
                    if (playerAnimator && animatorBoolName != "")
                    {
                        //DisableAllAnimatorWeapons();
                        playerAnimator.SetBool(animatorBoolName, true);
                        playerAnimator.SetLayerWeight(2, 1);
                    }

                    // event
                    OnMeleeUsed?.Invoke();
                }
            }
        }

        if (selectedMeleeWeapon)
        {
            MeleeWeapon meleeWeapon = selectedMeleeWeapon as MeleeWeapon;
            if (meleeWeapon != null)
            {
                selectedMeleeWeapon.ManualUpdate(gameObject);
            }

            // after melee attack, select weapon (only once)
            if (meleeWeapon.m_comboTimer <= (0 + meleeWeapon.m_comboTime/10.0f) && GetWeaponModel(selectedMeleeWeapon).activeSelf)
            {
                SelectWeapon(selectedWeaponType);
            }
            else if (meleeWeapon.m_comboTimer > 0){
                playerAnimator.SetLayerWeight(2, 1);
            }
        }

        if (m_currentlyThrowingItem != null)
        {
            // make player look at cursor
            PlayerMovement playerMovement = GetComponent<PlayerMovement>();
            playerMovement.SetLookDirection(playerMovement.GetMouseAimPlanePoint() - transform.position);

            //@todo make blending work with throwing
            // playerAnimator.SetLayerWeight(1, 0);
            // playerAnimator.SetLayerWeight(2, 0);
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

            if (!rangedWeapon.m_isAiming || rangedWeapon.m_isReloading || playerMovement.isRolling)
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

        corners.backLeft = m_aimZone.transform.position + m_aimZone.transform.right * -0.0f + m_aimZone.transform.forward * 0.1f;
        corners.backRight = m_aimZone.transform.position + m_aimZone.transform.right * 0.0f + m_aimZone.transform.forward * 0.1f;

        float distFromBackLeftToAimPoint = Vector3.Distance(corners.backLeft, new Vector3(aimPoint.x, corners.backLeft.y, aimPoint.z));
        float distFromBackRightToAimPoint = Vector3.Distance(corners.backRight, new Vector3(aimPoint.x, corners.backRight.y, aimPoint.z));

        float calcedRange = StatsManager.CalculateRange(rangedWeapon, rangedWeapon.m_range);

        corners.frontLeft = Quaternion.Euler(0, -currentAimAngle, 0) * (corners.backLeft + (m_aimZone.transform.forward * calcedRange) - corners.backLeft) + corners.backLeft;
        corners.frontRight = Quaternion.Euler(0, currentAimAngle, 0) * (corners.backRight + (m_aimZone.transform.forward * calcedRange) - corners.backRight) + corners.backRight;
        corners.frontLeft.y = m_aimZone.transform.position.y;
        corners.frontRight.y = m_aimZone.transform.position.y;

        return corners;
    }

    /// <summary>
    /// Gets the weapon model link of the given item
    /// </summary>
    /// <param name="weapon"></param>
    /// <returns></returns>
    public WeaponModelLink GetLink(Item weapon)
    {
        if (!weapon) return null;
        // check display name
        foreach (WeaponModelLink link in weaponModelLinks)
        {
            if (link.weapons.Any(w => w.m_displayName == weapon.m_displayName))
            {
                return link;
            }
        }
        // check ID if nothing
        foreach (WeaponModelLink link in weaponModelLinks)
        {
            if (link.weapons.Any(w => w.id == weapon.id))
            {
                return link;
            }
        }
        return null;
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
        // check display name
        foreach (WeaponModelLink link in weaponModelLinks)
        {
            if (link.weapons.Any(w => w.m_displayName == weapon.m_displayName))
            {
                return link.model;
            }
        }
        // check ID if nothing
        foreach (WeaponModelLink link in weaponModelLinks)
        {
            if (link.weapons.Any(w => w.id == weapon.id))
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
        // check display name
        foreach (WeaponModelLink link in weaponModelLinks)
        {
            if (link.weapons.Any(w => w.m_displayName == weapon.m_displayName))
            {
                return link.weaponFirepoint;
            }
        }
        // check ID if nothing
        foreach (WeaponModelLink link in weaponModelLinks)
        {
            if (link.weapons.Any(w => w.id == weapon.id))
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
        // check display name
        foreach (WeaponModelLink link in weaponModelLinks)
        {
            if (link.weapons.Any(w => w.m_displayName == weapon.m_displayName))
            {
                return link.animatorBoolName;
            }
        }
        // check ID if nothing
        foreach (WeaponModelLink link in weaponModelLinks)
        {
            if (link.weapons.Any(w => w.id == weapon.id))
            {
                return link.animatorBoolName;
            }
        }
        return "";
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
    private void TrySelectValidWeapon(bool _allowNone = true)
    {
        if (selectedWeapon == null)
        {
            SelectWeapon(SelectedWeaponType.Primary);
            if (selectedWeapon == null)
            {
                SelectWeapon(SelectedWeaponType.Secondary);
                if (selectedWeapon == null && _allowNone)
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

        int weaponIndex = -1;
        switch (selectedWeaponType)
        {
            case SelectedWeaponType.Primary:
                weaponIndex = 0;
                break;
            case SelectedWeaponType.Secondary:
                weaponIndex = 1;
                break;
            case SelectedWeaponType.None:
                weaponIndex = -1;
                break;
        }

        // get selected weapon
        selectedWeapon = playerInventory.slots.ElementAtOrDefault(weaponIndex)?.item;

        // if selected weapon is null and none, return
        if (selectedWeapon == null) {
            if (weaponIndex == -1){
                DisableAllWeaponModels();
            }
            return;
        }

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

            // play equip sound
            RangedWeapon rangedWeapon = selectedWeapon as RangedWeapon;
            if (rangedWeapon)
            {
                Instantiate(rangedWeapon.m_equipSound, weaponModel.transform.position, Quaternion.identity, null);
            }
        }
    }


    /// <summary>
    /// If the player's inventory is not null, and the third slot in the inventory is not null, then set
    /// the selected equipment to the item in the third slot
    /// </summary>
    /// <returns>
    /// The selected equipment is being returned.
    /// </returns>
    public void SelectEquipment(int _num)
    {
        // if inventory is null, return
        if (playerInventory == null) return;

        // get selected equipment
        selectedEquipment = playerInventory.slots.ElementAtOrDefault((_num == 1 ? 2 : 3))?.item;
        if (selectedEquipment && selectedEquipment.currentStackSize <= 0)
        {
            selectedEquipment = null;
        }

        // if selected equipment is null, return
        if (selectedEquipment == null) return;
    }

    public void SelectMeleeWeapon()
    {
        // if inventory is null, return
        if (playerInventory == null) return;

        // get selected equipment
        selectedMeleeWeapon = playerInventory.slots.ElementAtOrDefault(4)?.item;
        if (selectedMeleeWeapon && selectedMeleeWeapon.currentStackSize <= 0)
        {
            selectedMeleeWeapon = null;
        }

        // if selected equipment is null, return
        if (selectedMeleeWeapon == null) return;
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
        useEquipmentAction_1.Enable();
        useEquipmentAction_2.Enable();
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
        useEquipmentAction_1.Disable();
        useEquipmentAction_2.Disable();
        meleeAttackAction.Disable();
    }

    // custom editor
#if UNITY_EDITOR
    [CustomEditor(typeof(PlayerInventoryInterface))]
    public class PlayerWeaponControllerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            PlayerInventoryInterface myScript = (PlayerInventoryInterface)target;

            // if (GUILayout.Button("Move Weapons"))
            // {
            //     // move all link.weapon into link.weapons
            //     foreach (WeaponModelLink link in myScript.weaponModelLinks)
            //     {
            //         if (link.weapon != null)
            //         {
            //             // clear
            //             link.weapons.Clear();
            //             if (link.weapons.Contains(link.weapon) == false)
            //             {
            //                 link.weapons.Add(link.weapon);

            //                 // set dirty
            //                 EditorUtility.SetDirty(myScript);
            //             }
            //         }
            //     }
            // }
        }
    }
#endif
}
