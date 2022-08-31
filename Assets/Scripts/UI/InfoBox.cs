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
    public Image m_backgroundImage;
    public Image m_iconImage;
    public TextMeshProUGUI m_titleText;
    public TextMeshProUGUI m_descriptionText;

    public TextMeshProUGUI m_costTypeText;
    public Image m_costTypeImage;

    public Image m_dividerImage;

    public GameObject m_statsModsPanel;

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
            SetOpacity(1.0f);

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
            SetOpacity(opacity);

            if (fadeTimer <= 0.0f)
            {
                fadeTimer = 0.0f;
                DisableBox();
            }
        }
    }

    /// <summary>
    /// Sets the opacity of all UI elements in the info box
    /// </summary>
    /// <param name="_opacity"></param>
    private void SetOpacity(float _opacity)
    {
        // get all TextMeshProUGUI components and Image components, and set their opacity
        TextMeshProUGUI[] textComponents = GetComponentsInChildren<TextMeshProUGUI>();
        Image[] imageComponents = GetComponentsInChildren<Image>();
        foreach (TextMeshProUGUI text in textComponents)
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, _opacity);
        }
        foreach (Image image in imageComponents)
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, _opacity);
        }
    }

    /// <summary>
    /// Display some info in the info box at cursor.
    /// </summary>
    /// <param name="_title"></param>
    /// <param name="_icon"></param>
    /// <param name="_description"></param>
    /// <param name="_onTime"></param>
    /// <param name="_offTime"></param>
    public void DisplayInfo(string _title, Sprite _icon, string _description, float _onTime = 1, float _offTime = 1)
    {
        EnableBox();

        this.m_titleText.text = _title;
        this.m_iconImage.sprite = _icon;
        this.m_descriptionText.text = _description;

        fullBrightTime = _onTime;
        fullBrightTimer = fullBrightTime;

        fadeTime = _offTime;
        fadeTimer = fadeTime;

        m_dividerImage.gameObject.SetActive(false);
        m_statsModsPanel.SetActive(false);
    }

    /// <summary>
    /// Display an Item in the info box at cursor.
    /// </summary>
    /// <param name="_item"></param>
    /// <param name="_onTime"></param>
    /// <param name="_offTime"></param>
    public void Display(Item _item, bool _showCost = false, float _onTime = 1, float _offTime = 1)
    {
        if (!_item) return;

        DisplayInfo(_item.m_displayName, _item.m_icon, "Count: " + _item.currentStackSize + "/" + _item.maxStackSize + "\n" + _item.m_description, _onTime, _offTime);
        
        if (_showCost) {
            UpdateEconomy(_item);
        }
        else{
            UpdateType(_item);
        }

        // stats and mods panel
        m_dividerImage.gameObject.SetActive(true);
        m_statsModsPanel.SetActive(true);
        UpdateStats(_item);
        UpdateMods(_item.GetStatMods());
        // if the stats and mods panels are not active, disable the parent panel
        if (!m_statsPanel.activeSelf && !m_modsPanel.activeSelf)
        {
            m_dividerImage.gameObject.SetActive(false);
            m_statsModsPanel.SetActive(false);
        }
    }

    /// <summary>
    /// Update the stats based on the item type
    /// @todo - add more types
    /// </summary>
    /// <param name="_item"></param>
    private void UpdateStats(Item _item)
    {
        m_statsPanel.SetActive(false);
        m_statsDescriptionText.text = "";
        
        if (!_item) return;

        System.Type type = _item.GetType();

        if (type.IsSubclassOf(typeof(RangedWeapon)) || type == typeof(RangedWeapon)){
            m_statsPanel.SetActive(true);

            RangedWeapon weapon = (RangedWeapon)_item;

            // damage
            float calcDamage = StatsManager.CalculateDamage(weapon, weapon.m_damage);
            float damageDifference = calcDamage - weapon.m_damage;
            string damageModString = damageDifference != 0 ? " (" + StatsManager.SignedFloatString(damageDifference) + ")" : "";
            m_statsDescriptionText.text += "Damage: " + weapon.m_damage + damageModString + "\n";

            // range
            float calcRange = StatsManager.CalculateRange(weapon, weapon.m_range);
            float rangeDifference = calcRange - weapon.m_range;
            string rangeModString = rangeDifference != 0 ? " (" + StatsManager.SignedFloatString(rangeDifference) + ")" : "";
            m_statsDescriptionText.text += "Range: " + weapon.m_range + rangeModString + "\n";
        }
    }

    /// <summary>
    /// Updates the modifier section of the info box with current info
    /// </summary>
    /// <param name="_mods"></param>
    public void UpdateMods(List<StatsManager.StatMod> _mods)
    {
        if (_mods.Count > 0)
        {
            m_modsPanel.SetActive(true);

            m_modsDescriptionText.text = "";
            foreach (StatsManager.StatMod mod in _mods)
            {
                m_modsDescriptionText.text += mod.ToText() + "\n";
            }
        }
        else
        {
            m_modsPanel.SetActive(false);
        }
    }

    public void UpdateEconomy(EconomyManager.Purchasable _purchasable)
    {
        m_costTypeText.text = _purchasable.GetPrice().ToString();
        m_costTypeImage.sprite = m_priceIcon;
    }

    public void UpdateType(Item _item){
        m_costTypeText.text = _item.GetTypeDisplayName();
        m_costTypeImage.sprite = m_typeIcon;
    }

    /// <summary>
    /// Hides the info box
    /// </summary>
    private void DisableBox()
    {
        // disable all children (excluding this)
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
        // disable the background
        m_backgroundImage.enabled = false;
    }

    /// <summary>
    /// Shows the info box
    /// </summary>
    private void EnableBox()
    {
        // enable all children (excluding this)
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
        }
        // enable the background
        m_backgroundImage.enabled = true;
    }
}
