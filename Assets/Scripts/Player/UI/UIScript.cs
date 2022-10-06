using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;
using System;

/// <summary>
/// Manages the player UI.
/// </summary>
public class UIScript : MonoBehaviour
{
    public CursorScript cursorScript;

    [Header("Game UI")]
    public Image healthBG;
    public Image healthBar, healthBarDark;

    public GameObject primaryWeaponIcon;
    public GameObject secondaryWeaponIcon;
    
    public TextMeshProUGUI ammoText_1;
    public TextMeshProUGUI ammoText_2;

    public GameObject equipmentIcon_1;
    public GameObject equipmentIcon_2;
    public TextMeshProUGUI equipmentAmmoText_1;
    public TextMeshProUGUI equipmentAmmoText_2;

    public TextMeshProUGUI m_conditionText;

    [HideInInspector] public float healthBarMaxWidth;

    [Header("Interact Text")]
    public TextMeshProUGUI interactText;


    [Header("Boss UI")]
    public GameObject bossUI;
    public Image bossHealthBG;
    public Image bossHealthBar, bossHealthBarDark;
    [HideInInspector] public float bossHealthBarMaxWidth;
    public TextMeshProUGUI bossNameText;

    private PlayerInventoryInterface pii;


    public UnityEvent OnStart;

    private void Start()
    {
        if (cursorScript == null) cursorScript = GetComponent<CursorScript>();
        if (cursorScript == null) cursorScript = FindObjectOfType<CursorScript>();

        if (healthBar != null) healthBarMaxWidth = healthBar.rectTransform.sizeDelta.x;
        if (bossHealthBar != null) bossHealthBarMaxWidth = bossHealthBar.rectTransform.sizeDelta.x;

        OnStart.Invoke();
    }

    private void Update()
    {
        UpdateHud();
    }

    /// <summary>
    /// Updates the ing ame hud with weapos and equipment.
    /// </summary>
    private void UpdateHud()
    {
        if (InventoryManager.instance)
        {
            if (pii == null) pii = FindObjectOfType<PlayerInventoryInterface>();
            Item currentWeapon = pii.selectedWeapon;

            // update ammo text
            if (currentWeapon != null && (RangedWeapon)currentWeapon && ammoText_1 != null)
            {
                string spareAmmoText = "";
                if (((RangedWeapon)currentWeapon).m_unlimitedAmmo) spareAmmoText = "∞";
                else spareAmmoText = ((RangedWeapon)currentWeapon).m_spareAmmo.ToString();
                ammoText_1.text = ((RangedWeapon)currentWeapon).m_clipAmmo + "/" + spareAmmoText;
            }

            // get player inventory
            Inventory inventory = InventoryManager.instance.GetInventory("player");
            if (inventory != null)
            {
                //set icon
                RangedWeapon primaryWeapon = inventory.slots[0].item as RangedWeapon;
                if (primaryWeapon != null)
                {
                    primaryWeaponIcon.SetActive(true);
                    primaryWeaponIcon.GetComponent<Image>().sprite = primaryWeapon.m_icon;

                    string spareAmmoText = "";
                    if (((RangedWeapon)currentWeapon).m_unlimitedAmmo) spareAmmoText = "∞";
                    else spareAmmoText = ((RangedWeapon)currentWeapon).m_spareAmmo.ToString();
                    ammoText_1.text = ((RangedWeapon)currentWeapon).m_clipAmmo + "/" + spareAmmoText;

                    // enable/disable
                    if (currentWeapon && primaryWeapon.id == currentWeapon.id)
                    {
                        primaryWeaponIcon.transform.parent.gameObject.SetActive(true);
                        secondaryWeaponIcon.transform.parent.gameObject.SetActive(false);
                    }
                }
                else
                {
                    primaryWeaponIcon.SetActive(false);
                }

                //set icon
                RangedWeapon secondaryWeapon = inventory.slots[1].item as RangedWeapon;
                if (secondaryWeapon != null)
                {
                    secondaryWeaponIcon.SetActive(true);
                    secondaryWeaponIcon.GetComponent<Image>().sprite = secondaryWeapon.m_icon;

                    string spareAmmoText = "";
                    if (((RangedWeapon)currentWeapon).m_unlimitedAmmo) spareAmmoText = "∞";
                    else spareAmmoText = ((RangedWeapon)currentWeapon).m_spareAmmo.ToString();
                    ammoText_2.text = ((RangedWeapon)currentWeapon).m_clipAmmo + "/" + spareAmmoText;

                    // enable/disable
                    if (currentWeapon && secondaryWeapon.id == currentWeapon.id)
                    {
                        primaryWeaponIcon.transform.parent.gameObject.SetActive(false);
                        secondaryWeaponIcon.transform.parent.gameObject.SetActive(true);
                    }
                }
                else
                {
                    secondaryWeaponIcon.SetActive(false);
                }


                // equipment 1
                Equipment equipment = inventory.slots[2].item as Equipment;
                if (equipment != null)
                {
                    equipmentIcon_1.SetActive(true);
                    equipmentIcon_1.GetComponent<Image>().sprite = equipment.m_icon;

                    equipmentAmmoText_1.text = equipment.currentStackSize.ToString() + "/" + equipment.maxStackSize.ToString() + " " + equipment.m_displayName;

                    //if equipment count > 0 make white
                    if (equipment.currentStackSize > 0 && pii.selectedEquipment == equipment)
                    {
                        equipmentIcon_1.GetComponent<Image>().color = Color.white;
                    }
                    else
                    {
                        equipmentIcon_1.GetComponent<Image>().color = Color.gray;
                    }
                }
                else
                {
                    equipmentIcon_1.SetActive(false);
                    equipmentAmmoText_1.text = "NO EQUIPMENT";
                }

                // equipment 2
                equipment = inventory.slots[3].item as Equipment;
                if (equipment != null)
                {
                    equipmentIcon_2.SetActive(true);
                    equipmentIcon_2.GetComponent<Image>().sprite = equipment.m_icon;

                    equipmentAmmoText_2.text = equipment.currentStackSize.ToString() + "/" + equipment.maxStackSize.ToString() + " " + equipment.m_displayName;

                    //if equipment count > 0 make white
                    if (equipment.currentStackSize > 0 && pii.selectedEquipment == equipment)
                    {
                        equipmentIcon_2.GetComponent<Image>().color = Color.white;
                    }
                    else
                    {
                        equipmentIcon_2.GetComponent<Image>().color = Color.gray;
                    }
                }
                else
                {
                    equipmentIcon_2.SetActive(false);
                    equipmentAmmoText_2.text = "NO EQUIPMENT";
                }


            }
        }

        if (MissionManager.instance && JournalManager.instance && JournalManager.instance.m_showHUDConditions)
        {
            m_conditionText.transform.parent.gameObject.SetActive(true);
            Mission mission = MissionManager.instance.GetCurrentMission();
            if (mission != null && mission.m_conditions.Count > 0)
            {
                m_conditionText.text = "Conditions:";
                foreach (MissionCondition condition in mission.m_conditions)
                {
                    Color conditionCol = Color.white;
                    switch (condition.GetState())
                    {
                        case MissionCondition.ConditionState.COMPLETE:
                            conditionCol = Color.green;
                            break;
                        case MissionCondition.ConditionState.INCOMPLETE:
                            conditionCol = Color.white;
                            break;
                        case MissionCondition.ConditionState.FAILED:
                            conditionCol = Color.red;
                            break;
                    }

                    // show condition, and coloured sprite to show state
                    m_conditionText.text += "\n" + "<sprite=0 color=#" + ColorUtility.ToHtmlStringRGB(conditionCol) + ">" + condition.GetShortDescription();
                }
            }
            else
            {
                m_conditionText.text = "No Mission Conditions";
            }
        }
        else
        {
            m_conditionText.transform.parent.gameObject.SetActive(false);
        }
    }
}
