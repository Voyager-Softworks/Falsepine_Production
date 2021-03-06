using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

public class UIScript : MonoBehaviour  /// @todo Comment
{
    public CursorScript cursorScript;

    [Header("Game UI")]
    public TextMeshProUGUI ammoText;
    public Image healthBG;
    public Image healthBar;

    public GameObject primaryWeaponIcon;
    public GameObject secondaryWeaponIcon;
    public GameObject equipmentIcon;

    [HideInInspector] public float healthBarMaxWidth;

    [Header("Boss UI")]
    public GameObject bossUI;
    public Image bossHealthBG;
    public Image bossHealthBar;
    [HideInInspector] public float bossHealthBarMaxWidth;
    public TextMeshProUGUI bossNameText;

    [Header("Pause UI")]
    public GameObject pauseUI;


    public UnityEvent OnStart;

    private void Start() {
        if (cursorScript == null) cursorScript = GetComponent<CursorScript>();
        if (cursorScript == null) cursorScript = FindObjectOfType<CursorScript>();

        if (healthBar != null) healthBarMaxWidth = healthBar.rectTransform.sizeDelta.x;
        if (bossHealthBar != null) bossHealthBarMaxWidth = bossHealthBar.rectTransform.sizeDelta.x;

        OnStart.Invoke();

        // close pause
        ClosePauseMenu();
    }

    private void Update() {
        // if escape is pressed, toggle pause
        if (Keyboard.current.escapeKey.wasPressedThisFrame) {
            TogglePauseMenu();
        }

        if (InventoryManager.instance){
            PlayerInventoryInterface pii = FindObjectOfType<PlayerInventoryInterface>();
            Item currentWeapon = pii.selectedWeapon;

            // update ammo text
            if (currentWeapon != null && (RangedWeapon)currentWeapon && ammoText != null) {
                string spareAmmoText = "";
                if (((RangedWeapon)currentWeapon).m_unlimitedAmmo) spareAmmoText = "∞";
                else spareAmmoText = ((RangedWeapon)currentWeapon).m_spareAmmo.ToString();
                ammoText.text = ((RangedWeapon)currentWeapon).m_clipAmmo + "/" + spareAmmoText;
            }

            // get player inventory
            Inventory inventory = InventoryManager.instance.GetInventory("player");
            if (inventory != null) {
                //set icon
                RangedWeapon primaryWeapon = inventory.slots[0].item as RangedWeapon;
                if (primaryWeapon != null) {
                    primaryWeaponIcon.SetActive(true);
                    primaryWeaponIcon.GetComponent<Image>().sprite = primaryWeapon.m_icon;

                    // set col of icon
                    if (currentWeapon && primaryWeapon.id == currentWeapon.id){
                        primaryWeaponIcon.GetComponent<Image>().color = Color.white;
                    } else {
                        primaryWeaponIcon.GetComponent<Image>().color = Color.gray;
                    }

                } else {
                    primaryWeaponIcon.SetActive(false);
                }

                //set icon
                RangedWeapon secondaryWeapon = inventory.slots[1].item as RangedWeapon;
                if (secondaryWeapon != null) {
                    secondaryWeaponIcon.SetActive(true);
                    secondaryWeaponIcon.GetComponent<Image>().sprite = secondaryWeapon.m_icon;

                    //set col of icon
                    if (currentWeapon && secondaryWeapon.id == currentWeapon.id){
                        secondaryWeaponIcon.GetComponent<Image>().color = Color.white;
                    } else {
                        secondaryWeaponIcon.GetComponent<Image>().color = Color.gray;
                    }

                } else {
                    secondaryWeaponIcon.SetActive(false);
                }

                

                Equipment equipment = inventory.slots[2].item as Equipment;
                if (equipment != null) {
                    equipmentIcon.SetActive(true);
                    equipmentIcon.GetComponent<Image>().sprite = equipment.m_icon;

                    //if equipment count > 0 make white
                    if (equipment.currentStackSize > 0 && pii.selectedEquipment == equipment){
                        equipmentIcon.GetComponent<Image>().color = Color.white;
                    } else {
                        equipmentIcon.GetComponent<Image>().color = Color.gray;
                    }
                } else {
                    equipmentIcon.SetActive(false);
                }


            }
        }
    }

    private void TogglePauseMenu()
    {
        if (pauseUI == null) return;

        if (pauseUI.activeSelf)
        {
            ClosePauseMenu();
        }
        else
        {
            OpenPauseMenu();
        }
    }

    private void OpenPauseMenu(){
        if (pauseUI == null) return;

        pauseUI.SetActive(true);
    }

    private void ClosePauseMenu(){
        if (pauseUI == null) return;

        pauseUI.SetActive(false);
    }


}
