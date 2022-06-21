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

        if (aimLines){
            PlayerMovement playerMovement = GetComponent<PlayerMovement>();

            Vector3 aimPoint = playerMovement.GetMouseAimPoint();

            GameObject firepointObj = GetWeaponFirepoint(selectedWeapon);
            if (!firepointObj) return;

            Vector3 aimpointL = Quaternion.Euler(0, -15, 0) * (aimPoint - firepointObj.transform.position) + firepointObj.transform.position;
            Vector3 aimpointR = Quaternion.Euler(0, 15, 0) * (aimPoint - firepointObj.transform.position) + firepointObj.transform.position;
            
            aimLines.SetPosition(0, aimpointL);
            aimLines.SetPosition(1, firepointObj.transform.position);
            aimLines.SetPosition(2, aimpointR);
        }
    }

    public GameObject GetWeaponFirepoint(Item weapon)
    {
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
