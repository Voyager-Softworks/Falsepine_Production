using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEngine.InputSystem;

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

    [Header("Inventory")]
    public string playerInventoryName = "player";
    public Inventory playerInventory;

    public enum SelectedWeaponType{ Primary, Secondary, None }
    [SerializeField] public SelectedWeaponType selectedWeaponType = SelectedWeaponType.None;
    public Item selectedWeapon;

    [Serializable]
    public class WeaponModelLink
    {
        public Item weapon;
        public GameObject weaponFirepoint;
        public string animatorBoolName = "";
    }


    [Header("References")]  
    public List<WeaponModelLink> weaponFirepointLinks = new List<WeaponModelLink>();

    public LineRenderer aimLines;

    public Animator playerAnimator;


    // Start is called before the first frame update
    void Start()
    {
        playerInventory = InventoryManager.instance?.GetInventory(playerInventoryName);

        SwapWeapon();
        // select weapon
        SelectWeapon(selectedWeaponType);
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
            selectedWeapon.Update(gameObject);

            GameObject weaponFirepoint = GetWeaponFirepoint(selectedWeapon);
            Vector3 weaponFirepointPosition = weaponFirepoint.transform.position;
            Vector3 fireDirection = GetComponent<PlayerMovement>().GetMouseAimPoint() - weaponFirepointPosition;

            RangedWeapon rangedWeapon = selectedWeapon as RangedWeapon;
            if (rangedWeapon)
            {
                // if fire weapon action is pressed and is not auto and is not shooting, shoot weapon || if fire weapon action is current down and is auto, shoot weapon
                if ((fireWeaponAction.triggered && !rangedWeapon.m_isAutomnatic) || (fireWeaponAction.ReadValue<float>() > 0 && rangedWeapon.m_isAutomnatic))
                {
                    rangedWeapon.TryShoot(weaponFirepointPosition, fireDirection, gameObject);
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

            UpdateAimLines();
        }
    }

    private void UpdateAimLines()
    {
        if (aimLines)
        {
            if (!selectedWeapon) return;

            RangedWeapon rangedWeapon = selectedWeapon as RangedWeapon;
            if (!rangedWeapon) return;

            PlayerMovement playerMovement = GetComponent<PlayerMovement>();
            if (!playerMovement) return;

            if (!rangedWeapon.m_isAiming || rangedWeapon.m_reloadTimer > 0 || playerMovement.isRolling){
                // fade to transparent over time
                Color currentColor = aimLines.startColor;
                // subtract alpha from current color using time
                currentColor.a -= Time.deltaTime * 10.0f;
                // clamp aplpha between 0 and 1
                currentColor.a = Mathf.Clamp01(currentColor.a);
                // set color
                aimLines.startColor = currentColor;
                aimLines.endColor = currentColor;
            }
            else{
                // fade to opaque over time
                Color currentColor = aimLines.startColor;
                // add alpha to current color using time
                currentColor.a += Time.deltaTime * 2.0f;
                // clamp aplpha between 0 and 1
                currentColor.a = Mathf.Clamp01(currentColor.a);
                // set color
                aimLines.startColor = currentColor;
                aimLines.endColor = currentColor;
            }

            Vector3 aimPoint = playerMovement.GetMouseAimPoint();

            GameObject firepointObj = GetWeaponFirepoint(selectedWeapon);
            if (!firepointObj) return;

            float currentAimAngle = rangedWeapon.CalcCurrentAimAngle();
            // if nan or inf, set to 0
            if (float.IsNaN(currentAimAngle) || float.IsInfinity(currentAimAngle)) currentAimAngle = 0; ;

            Vector3 aimpointL = Quaternion.Euler(0, -currentAimAngle, 0) * (aimPoint - firepointObj.transform.position) + firepointObj.transform.position;
            Vector3 aimpointR = Quaternion.Euler(0, currentAimAngle, 0) * (aimPoint - firepointObj.transform.position) + firepointObj.transform.position;

            aimLines.SetPosition(0, aimpointL);
            aimLines.SetPosition(1, firepointObj.transform.position);
            aimLines.SetPosition(2, aimpointR);
        }
    }

    public GameObject GetWeaponFirepoint(Item weapon)
    {
        if (!weapon) return null;
        foreach (WeaponModelLink link in weaponFirepointLinks)
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
        foreach (WeaponModelLink link in weaponFirepointLinks)
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
    }

    public void DisableAllAnimatorWeapons(){
        if (playerAnimator == null) return;

        foreach (WeaponModelLink link in weaponFirepointLinks)
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
    }

    public void DisableInput()
    {
        swapWeaponAction.Disable();
        reloadAction.Disable();
        fireWeaponAction.Disable();
        aimWeaponAction.Disable();
    }
}
