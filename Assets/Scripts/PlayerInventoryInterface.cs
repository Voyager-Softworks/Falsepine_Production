using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.InputSystem;

[Serializable]
public class PlayerInventoryInterface : MonoBehaviour
{
    // Input actions:
    // swap weapon
    public InputAction swapWeaponAction;
    // reload
    public InputAction reloadAction;
    // fire weapon
    public InputAction fireWeaponAction;
    // aim weapon
    public InputAction aimWeaponAction;


    public string playerInventoryName = "player";
    public Inventory playerInventory;

    public enum SelectedWeaponType{ None, Primary, Secondary }
    [SerializeField] public SelectedWeaponType selectedWeaponType = SelectedWeaponType.None;


    // Start is called before the first frame update
    void Start()
    {
        playerInventory = InventoryManager.instance?.GetInventory(playerInventoryName);
    }

    // Update is called once per frame
    void Update()
    {
        // if swap weapon action is pressed, swap weapon
        if (swapWeaponAction.triggered)
        {
            SwapWeapon();
        }
    }

    public void SwapWeapon(){
        // toggle between primary and secondary weapon
        if (selectedWeaponType == SelectedWeaponType.Primary){
            selectedWeaponType = SelectedWeaponType.Secondary;
        } else if (selectedWeaponType == SelectedWeaponType.Secondary){
            selectedWeaponType = SelectedWeaponType.Primary;
        }
        else {
            selectedWeaponType = SelectedWeaponType.Primary;
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
