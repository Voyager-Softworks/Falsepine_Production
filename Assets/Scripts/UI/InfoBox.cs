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

    [Header("Box Refs")]
    public Image background;
    public TextMeshProUGUI title;
    public Image icon;
    public TextMeshProUGUI description;

    [Header("Economy Refs")]
    public Image backgroundEconomy;
    public TextMeshProUGUI titleEconomy;
    public TextMeshProUGUI amountText;
    public Image iconEconomy;

    [Header("Mods Refs")]
    public Image modBackground;
    public Image modIcon;
    public TextMeshProUGUI modTitle;
    public TextMeshProUGUI modList;


    // Start is called before the first frame update
    void Start()
    {
        DisableBox();
        DisableEconomyBox();
        DisableModsBox();
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
                DisableEconomyBox();
                DisableModsBox();
            }
        }
    }

    private void UpdateModOpacity(float opacity)
    {
        modBackground.color = new Color(modBackground.color.r, modBackground.color.g, modBackground.color.b, opacity);
        modTitle.color = new Color(modTitle.color.r, modTitle.color.g, modTitle.color.b, opacity);
        modList.color = new Color(modList.color.r, modList.color.g, modList.color.b, opacity);
    }

    private void UpdateEconomyOpacity(float opacity)
    {
        backgroundEconomy.color = new Color(backgroundEconomy.color.r, backgroundEconomy.color.g, backgroundEconomy.color.b, opacity);
        titleEconomy.color = new Color(titleEconomy.color.r, titleEconomy.color.g, titleEconomy.color.b, opacity);
        amountText.color = new Color(amountText.color.r, amountText.color.g, amountText.color.b, opacity);
        iconEconomy.color = new Color(iconEconomy.color.r, iconEconomy.color.g, iconEconomy.color.b, opacity);
    }

    private void UpdateInfoOpacity(float opacity)
    {
        background.color = new Color(background.color.r, background.color.g, background.color.b, opacity);
        title.color = new Color(title.color.r, title.color.g, title.color.b, opacity);
        icon.color = new Color(icon.color.r, icon.color.g, icon.color.b, opacity);
        description.color = new Color(description.color.r, description.color.g, description.color.b, opacity);
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

        this.title.text = title;
        this.icon.sprite = icon;
        this.description.text = description;

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
            EnableModsBox();

            modList.text = "";
            foreach (StatsManager.StatMod mod in mods)
            {
                modList.text += mod.ToText() + "\n";
            }
        }
        else
        {
            DisableModsBox();
        }
    }

    public void UpdateEconomy(EconomyManager.Purchasable purchasable)
    {
        EnableEconomyBox();
        amountText.text = purchasable.GetPrice().ToString();
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
        background.enabled = false;
        title.enabled = false;
        icon.enabled = false;
        description.enabled = false;
    }
    /// <summary>
    /// Shows the info box
    /// </summary>
    private void EnableBox()
    {
        background.enabled = true;
        title.enabled = true;
        icon.enabled = true;
        description.enabled = true;
    }

    /// <summary>
    /// Enables the gold box
    /// </summary>
    public void EnableEconomyBox(){
        backgroundEconomy.enabled = true;
        titleEconomy.enabled = true;
        amountText.enabled = true;
        iconEconomy.enabled = true;
    }

    /// <summary>
    /// Disables the gold box
    /// </summary>
    public void DisableEconomyBox(){
        backgroundEconomy.enabled = false;
        titleEconomy.enabled = false;
        amountText.enabled = false;
        iconEconomy.enabled = false;
    }

    /// <summary>
    /// Hides the modifier section of the info box
    /// </summary>
    private void DisableModsBox()
    {
        modBackground.enabled = false;
        modTitle.enabled = false;
        modList.enabled = false;
        modIcon.enabled = false;
    }
    /// <summary>
    /// Shows the modifier section of the info box
    /// </summary>
    private void EnableModsBox()
    {
        modBackground.enabled = true;
        modTitle.enabled = true;
        modList.enabled = true;
        modIcon.enabled = true;
    }
}
