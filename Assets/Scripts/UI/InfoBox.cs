using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System;

/// <summary>
/// Called by other scripts to display information at the cursor
/// </summary>
public class InfoBox : MonoBehaviour
{
    public float fullBrightTime = 1.0f;
    private float fullBrightTimer = 0.0f;

    public float fadeTime = 1.0f;
    private float fadeTimer = 0.0f;

    public bool m_showCost = false;

    [Header("Box Refs")]
    public Image m_backgroundImage;
    public Image m_iconImage;
    public TextMeshProUGUI m_titleText;
    public TextMeshProUGUI m_descriptionText;

    public TextMeshProUGUI m_costTypeText;
    public Image m_costTypeImage;

    [Header("Stats Refs")]
    public GameObject m_statsPanel;
    public Image m_statsImage;
    public TextMeshProUGUI m_statsTitleText;
    public TextMeshProUGUI m_statsDescriptionText;

    [Header("Mods Refs")]
    public GameObject m_modsPanel;
    public Image m_modsImage;
    public TextMeshProUGUI m_modsTitleText;
    public TextMeshProUGUI m_modsDescriptionText;

    [Header("Prefabs")]
    public Sprite m_priceIcon;
    public Sprite m_typeIcon;



    // Start is called before the first frame update
    void Start()
    {
        DisableBox();
        //DisableEconomyBox();
        //DisableModsBox();
    }

    // Update is called once per frame
    void Update()
    {
        // set to mouse position
        Vector3 mousePos = Mouse.current.position.ReadValue();
        transform.position = mousePos;
        // move upward
        transform.position += new Vector3(0, 20.0f, 0);


        if (fullBrightTimer > 0.0f)
        {
            fullBrightTimer -= Time.deltaTime;

            // set opacity of all to 1
            UpdateInfoOpacity(1.0f);
            UpdateEconomyOpacity(1.0f);
            UpdateModOpacity(1.0f);

            if (fullBrightTimer <= 0.0f)
            {
                fullBrightTimer = 0.0f;
                fadeTimer = fadeTime;
            }
        }
        else if (fadeTimer > 0.0f)
        {
            fadeTimer -= Time.deltaTime;

            // fade opacity of all to 0 using timer
            float opacity = (fadeTimer / fadeTime);
            UpdateInfoOpacity(opacity);
            UpdateEconomyOpacity(opacity);
            UpdateModOpacity(opacity);

            if (fadeTimer <= 0.0f)
            {
                fadeTimer = 0.0f;
                DisableBox();
                //DisableEconomyBox();
                //DisableModsBox();
            }
        }
    }

    private void UpdateModOpacity(float opacity)
    {
        // modBackground.color = new Color(modBackground.color.r, modBackground.color.g, modBackground.color.b, opacity);
        // modTitle.color = new Color(modTitle.color.r, modTitle.color.g, modTitle.color.b, opacity);
        // modList.color = new Color(modList.color.r, modList.color.g, modList.color.b, opacity);
    }

    private void UpdateEconomyOpacity(float opacity)
    {
        // backgroundEconomy.color = new Color(backgroundEconomy.color.r, backgroundEconomy.color.g, backgroundEconomy.color.b, opacity);
        // titleEconomy.color = new Color(titleEconomy.color.r, titleEconomy.color.g, titleEconomy.color.b, opacity);
        // amountText.color = new Color(amountText.color.r, amountText.color.g, amountText.color.b, opacity);
        // iconEconomy.color = new Color(iconEconomy.color.r, iconEconomy.color.g, iconEconomy.color.b, opacity);
    }

    private void UpdateInfoOpacity(float opacity)
    {
        m_backgroundImage.color = new Color(m_backgroundImage.color.r, m_backgroundImage.color.g, m_backgroundImage.color.b, opacity);
        m_titleText.color = new Color(m_titleText.color.r, m_titleText.color.g, m_titleText.color.b, opacity);
        m_iconImage.color = new Color(m_iconImage.color.r, m_iconImage.color.g, m_iconImage.color.b, opacity);
        m_descriptionText.color = new Color(m_descriptionText.color.r, m_descriptionText.color.g, m_descriptionText.color.b, opacity);
    }

    /// <summary>
    /// Display some info in the info box at cursor.
    /// </summary>
    /// <param name="title"></param>
    /// <param name="icon"></param>
    /// <param name="description"></param>
    /// <param name="onTime"></param>
    /// <param name="offTime"></param>
    public void DisplayInfo(string title, Sprite icon, string description, float onTime = 1, float offTime = 1)
    {
        EnableBox();

        this.m_titleText.text = title;
        this.m_iconImage.sprite = icon;
        this.m_descriptionText.text = description;

        fullBrightTime = onTime;
        fullBrightTimer = fullBrightTime;

        fadeTime = offTime;
        fadeTimer = fadeTime;
    }

    /// <summary>
    /// Updates the modifier section of the info box with current info
    /// </summary>
    /// <param name="mods"></param>
    public void UpdateMods(List<StatsManager.StatMod> mods)
    {
        if (mods.Count > 0)
        {
            //EnableModsBox();

            m_modsDescriptionText.text = "";
            foreach (StatsManager.StatMod mod in mods)
            {
                m_modsDescriptionText.text += mod.ToText() + "\n";
            }
        }
        else
        {
            //DisableModsBox();
        }
    }

    public void UpdateEconomy(EconomyManager.Purchasable purchasable)
    {
        //EnableEconomyBox();
        m_costTypeText.text = purchasable.GetPrice().ToString();
    }

    /// <summary>
    /// Display an Item in the info box at cursor.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="onTime"></param>
    /// <param name="offTime"></param>
    public void Display(Item item, bool showCost = false, float onTime = 1, float offTime = 1)
    {
        if (!item) return;
        DisplayInfo(item.m_displayName, item.m_icon, "Count: " + item.currentStackSize + "/" + item.maxStackSize + "\n" + item.m_description, onTime, offTime);
        if (showCost) UpdateEconomy(item);
        UpdateMods(item.GetStatMods());
    }

    /// <summary>
    /// Hides the info box
    /// </summary>
    private void DisableBox()
    {
        m_backgroundImage.enabled = false;
        m_titleText.enabled = false;
        m_iconImage.enabled = false;
        m_descriptionText.enabled = false;
    }
    /// <summary>
    /// Shows the info box
    /// </summary>
    private void EnableBox()
    {
        m_backgroundImage.enabled = true;
        m_titleText.enabled = true;
        m_iconImage.enabled = true;
        m_descriptionText.enabled = true;
    }

    /// <summary>
    /// Enables the gold box
    /// </summary>
    // public void EnableEconomyBox(){
    //     backgroundEconomy.enabled = true;
    //     titleEconomy.enabled = true;
    //     amountText.enabled = true;
    //     iconEconomy.enabled = true;
    // }

    /// <summary>
    /// Disables the gold box
    /// </summary>
    // public void DisableEconomyBox(){
    //     backgroundEconomy.enabled = false;
    //     titleEconomy.enabled = false;
    //     amountText.enabled = false;
    //     iconEconomy.enabled = false;
    // }

    /// <summary>
    /// Hides the modifier section of the info box
    /// </summary>
    // private void DisableModsBox()
    // {
    //     modBackground.enabled = false;
    //     modTitle.enabled = false;
    //     modList.enabled = false;
    //     modIcon.enabled = false;
    // }
    /// <summary>
    /// Shows the modifier section of the info box
    /// </summary>
    // private void EnableModsBox()
    // {
    //     modBackground.enabled = true;
    //     modTitle.enabled = true;
    //     modList.enabled = true;
    //     modIcon.enabled = true;
    // }
}
