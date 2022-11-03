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



    [Header("Player Stats")]
    public Image healthBG;
    public Image healthBar, healthBarDark;
    [HideInInspector] public float healthBarMaxWidth;

    public Image staminaBG;
    public Image staminaBar, staminaBarDark;
    [HideInInspector] public float staminaBarMaxWidth;

    [Header("Game UI")]
    public GameObject primaryWeaponIcon;
    public GameObject secondaryWeaponIcon;

    public TextMeshProUGUI ammoText_1;
    public TextMeshProUGUI ammoText_2;

    public GameObject equipmentIcon_1;
    public GameObject equipmentIcon_2;
    public TextMeshProUGUI equipmentAmmoText_1;
    public TextMeshProUGUI equipmentAmmoText_2;

    public Image weaponEffectIcon_1;
    public Image weaponEffectIcon_2;

    public TextMeshProUGUI m_conditionText;


    [Serializable]
    public class EffectImage
    {
        public StatsManager.StatType statType;
        public Sprite image;
    }

    [Header("Effects")]
    public List<EffectImage> effectImages;

    [Header("Interact Text")]
    public TextMeshProUGUI interactText;
    public Image interactBackground;


    [Header("Boss UI")]
    public GameObject bossUI;
    public Image bossHealthBG;
    public Image bossHealthBar, bossHealthBarDark;
    public Sprite bossHealthTex, bossHealthTexInvuln;
    [HideInInspector] public float bossHealthBarMaxWidth;

    private PlayerInventoryInterface pii;


    public UnityEvent OnStart;

    private void Start()
    {
        if (cursorScript == null) cursorScript = GetComponent<CursorScript>();
        if (cursorScript == null) cursorScript = FindObjectOfType<CursorScript>();

        if (healthBar != null) healthBarMaxWidth = healthBar.rectTransform.sizeDelta.x;
        if (staminaBar != null) staminaBarMaxWidth = staminaBar.rectTransform.sizeDelta.x;
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

            // get player inventory
            Inventory inventory = InventoryManager.instance.GetInventory("player");
            if (inventory != null)
            {
                //set primary icon
                RangedWeapon primaryWeapon = inventory.slots[0].item as RangedWeapon;
                if (primaryWeapon != null)
                {
                    primaryWeaponIcon.SetActive(true);
                    primaryWeaponIcon.GetComponent<Image>().sprite = primaryWeapon.m_icon;

                    string spareAmmoText = "";
                    if (primaryWeapon.m_unlimitedAmmo) spareAmmoText = "∞";
                    else spareAmmoText = primaryWeapon.m_spareAmmo.ToString();
                    ammoText_1.text = primaryWeapon.m_clipAmmo + "/" + spareAmmoText;

                    // set weapon effect icon
                    if (primaryWeapon.m_tempAmmoStats.Count > 0)
                    {
                        weaponEffectIcon_1.transform.parent.gameObject.SetActive(true);
                        weaponEffectIcon_1.sprite = effectImages.Find(x => x.statType == primaryWeapon.m_tempAmmoStats[0]).image;
                    }
                    else
                    {
                        weaponEffectIcon_1.transform.parent.gameObject.SetActive(false);
                    }

                    // enable/disable
                    if (currentWeapon && primaryWeapon.id == currentWeapon.id)
                    {
                        primaryWeaponIcon.transform.parent.gameObject.SetActive(true);
                        secondaryWeaponIcon.transform.parent.gameObject.SetActive(false);
                    }
                }
                else
                {
                    primaryWeaponIcon.transform.parent.gameObject.SetActive(false);
                    primaryWeaponIcon.SetActive(false);
                }

                //set seconday icon
                RangedWeapon secondaryWeapon = inventory.slots[1].item as RangedWeapon;
                if (secondaryWeapon != null)
                {
                    secondaryWeaponIcon.SetActive(true);
                    secondaryWeaponIcon.GetComponent<Image>().sprite = secondaryWeapon.m_icon;

                    string spareAmmoText = "";
                    if (secondaryWeapon.m_unlimitedAmmo) spareAmmoText = "∞";
                    else spareAmmoText = secondaryWeapon.m_spareAmmo.ToString();
                    ammoText_2.text = secondaryWeapon.m_clipAmmo + "/" + spareAmmoText;

                    // set weapon effect icon
                    if (secondaryWeapon.m_tempAmmoStats.Count > 0)
                    {
                        weaponEffectIcon_2.transform.parent.gameObject.SetActive(true);
                        weaponEffectIcon_2.sprite = effectImages.Find(x => x.statType == secondaryWeapon.m_tempAmmoStats[0]).image;
                    }
                    else
                    {
                        weaponEffectIcon_2.transform.parent.gameObject.SetActive(false);
                    }

                    // enable/disable
                    if (currentWeapon && secondaryWeapon.id == currentWeapon.id)
                    {
                        primaryWeaponIcon.transform.parent.gameObject.SetActive(false);
                        secondaryWeaponIcon.transform.parent.gameObject.SetActive(true);
                    }
                }
                else
                {
                    secondaryWeaponIcon.transform.parent.gameObject.SetActive(false);
                    secondaryWeaponIcon.SetActive(false);
                }

                // if primary and secondary icon parent are disabled, enable the secondary icon parent
                if (!primaryWeaponIcon.transform.parent.gameObject.activeSelf && !secondaryWeaponIcon.transform.parent.gameObject.activeSelf)
                {
                    secondaryWeaponIcon.transform.parent.gameObject.SetActive(true);
                    ammoText_2.text = "No Weapon";

                    // disable weapon effect icon
                    weaponEffectIcon_2.transform.parent.gameObject.SetActive(false);
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
                m_conditionText.text = "<size=110%>Conditions:";
                foreach (MissionCondition condition in mission.m_conditions)
                {
                    Color conditionCol = Color.white;
                    string spriteName = "cond";
                    switch (condition.GetState())
                    {
                        case MissionCondition.ConditionState.COMPLETE:
                            conditionCol = Color.green;
                            //spriteName = "tick";
                            break;
                        case MissionCondition.ConditionState.INCOMPLETE:
                            conditionCol = Color.white;
                            //spriteName = "circle";
                            break;
                        case MissionCondition.ConditionState.FAILED:
                            conditionCol = Color.red;
                            //spriteName = "x";
                            break;
                    }

                    // show condition, and coloured sprite to show state
                    m_conditionText.text += "\n" + "<size=90%><sprite name=\"" + spriteName + "\" color=#" + ColorUtility.ToHtmlStringRGB(conditionCol) + ">" + condition.GetDescription();
                }
            }
            else
            {
                m_conditionText.text = "<size=90%>No Mission Conditions";
            }
        }
        else
        {
            m_conditionText.transform.parent.gameObject.SetActive(false);
        }
    }
}
