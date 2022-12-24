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


    [Header("Main Refs")]
    public Image m_backgroundImage;
    public Image m_iconImage;
    public TextMeshProUGUI m_titleText;
    public TextMeshProUGUI m_descriptionText;

    public TextMeshProUGUI m_costTypeText;
    public Image m_costTypeImage;

    public Image m_dividerImage;

    public GameObject m_bottomPanel;

    [Header("Left Refs")]
    public GameObject m_leftPanel;
    public Image m_leftImage;
    public TextMeshProUGUI m_leftTitleText;
    public TextMeshProUGUI m_leftDescriptionText;

    [Header("Right Refs")]
    public GameObject m_rightPanel;
    public Image m_rightImage;
    public TextMeshProUGUI m_rightTitleText;
    public TextMeshProUGUI m_rightDescriptionText;

    [Header("Prefabs")]
    public Sprite m_priceIcon;
    public Sprite m_typeIcon;
    public Sprite m_statsIcon;
    public Sprite m_modifierIcon;



    // Start is called before the first frame update
    void Start()
    {
        DisableAll();
        //DisableEconomyBox();
        //DisableModsBox();
    }

    // Update is called once per frame
    void Update()
    {
        // set to mouse position
        Vector3 mousePos = Mouse.current.position.ReadValue();
        transform.position = mousePos;

        // set anchor to bottom left
        GetComponent<RectTransform>().pivot = new Vector2(0, 0);

        // move 20 units away from cursor in direction of anchor (e.g. 0,0 = up 20, right 20, 1,1 = down 20, left 20)
        Vector2 anchor = GetComponent<RectTransform>().pivot;
        Vector2 relativeAnchor = new Vector2(0.5f - anchor.x, 0.5f - anchor.y);
        transform.position = mousePos + (Vector3)(relativeAnchor * 20.0f);


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
                DisableAll();
            }
        }

        // keep on screen:
        // get right most point
        float right = transform.position.x + GetComponent<RectTransform>().rect.xMax * transform.lossyScale.x;
        // get left most point
        float left = transform.position.x + GetComponent<RectTransform>().rect.xMin * transform.lossyScale.x;
        // get top most point
        float top = transform.position.y + GetComponent<RectTransform>().rect.yMax * transform.lossyScale.y;
        // get bottom most point
        float bottom = transform.position.y + GetComponent<RectTransform>().rect.yMin * transform.lossyScale.y;


        // get screen width
        float screenWidth = Screen.width;
        // get screen height
        float screenHeight = Screen.height;

        // if right most point is off screen, set anchor to right
        if (right > screenWidth)
        {
            GetComponent<RectTransform>().pivot = new Vector2(1.0f, GetComponent<RectTransform>().pivot.y);
        }
        // if left most point is off screen, set anchor to left
        else if (left < 0)
        {
            GetComponent<RectTransform>().pivot = new Vector2(0.0f, GetComponent<RectTransform>().pivot.y);
        }

        // if top most point is off screen, set anchor to top
        if (top > screenHeight)
        {
            GetComponent<RectTransform>().pivot = new Vector2(GetComponent<RectTransform>().pivot.x, 1.0f);
        }
        // if bottom most point is off screen, set anchor to bottom
        else if (bottom < 0)
        {
            GetComponent<RectTransform>().pivot = new Vector2(GetComponent<RectTransform>().pivot.x, 0.0f);
        }

        // move 20 units away from cursor in direction of anchor (e.g. 0,0 = up 20, right 20, 1,1 = down 20, left 20)
        anchor = GetComponent<RectTransform>().pivot;
        relativeAnchor = new Vector2(0.5f - anchor.x, 0.5f - anchor.y);
        transform.position = mousePos + (Vector3)(relativeAnchor * 20.0f);

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
    /// Enables and displays the main info box <br/>
    /// ALWAYS call this before left or right info box
    /// </summary>
    /// <param name="_title"></param>
    /// <param name="_icon"></param>
    /// <param name="_description"></param>
    /// <param name="_costType"></param>
    /// <param name="_costIcon"></param>
    /// <param name="_onTime"></param>
    /// <param name="_offTime"></param>
    public void DisplayMain(string _title, Sprite _icon, string _description, string _costType, Sprite _costIcon, float _onTime = 0.1f, float _offTime = 0.1f)
    {
        EnableAll();

        this.m_titleText.text = _title;
        this.m_iconImage.sprite = _icon ?? null;
        this.m_descriptionText.text = _description;

        this.m_costTypeText.text = _costType;
        this.m_costTypeImage.sprite = _costIcon ?? null;
        if (_costIcon == null)
        {
            this.m_costTypeImage.enabled = false;
        }
        else
        {
            this.m_costTypeImage.enabled = true;
        }

        fullBrightTime = _onTime;
        fullBrightTimer = fullBrightTime;

        fadeTime = _offTime;
        fadeTimer = fadeTime;

        // set bottom to disabled by default, must call left or right to enable
        m_dividerImage.gameObject.SetActive(false);
        m_bottomPanel.SetActive(false);
        m_leftPanel.SetActive(false);
        m_rightPanel.SetActive(false);
    }

    /// <summary>
    /// Enables and displays the left portion of the info box <br/>
    /// ALWAYS call DisplayMain before this
    /// </summary>
    /// <param name="_title"></param>
    /// <param name="_icon"></param>
    /// <param name="_description"></param>
    /// <param name="_onTime"></param>
    /// <param name="_offTime"></param>
    public void DisplayLeft(string _title, Sprite _icon, string _description)
    {
        m_dividerImage.gameObject.SetActive(true);
        m_bottomPanel.SetActive(true);
        m_leftPanel.SetActive(true);

        this.m_leftTitleText.text = _title;
        this.m_leftImage.sprite = _icon ?? null;
        this.m_leftDescriptionText.text = _description;

    }

    /// <summary>
    /// Enables and displays the right portion of the info box <br/>
    /// ALWAYS call DisplayMain before this
    /// </summary>
    /// <param name="_title"></param>
    /// <param name="_icon"></param>
    /// <param name="_description"></param>
    /// <param name="_onTime"></param>
    /// <param name="_offTime"></param>
    public void DisplayRight(string _title, Sprite _icon, string _description)
    {
        m_dividerImage.gameObject.SetActive(true);
        m_bottomPanel.SetActive(true);
        m_rightPanel.SetActive(true);

        this.m_rightTitleText.text = _title;
        this.m_rightImage.sprite = _icon ?? null;
        this.m_rightDescriptionText.text = _description;
    }

    /// <summary>
    /// Display an Item in the info box at cursor.
    /// </summary>
    /// <param name="_item"></param>
    /// <param name="_onTime"></param>
    /// <param name="_offTime"></param>
    public void Display(Item _item, bool _showCost = false, float _onTime = 0.1f, float _offTime = 0.1f)
    {
        if (!_item) return;

        DisplayMain(
            _item.m_displayName, 
            _item.m_icon, 
            "Count: " + _item.currentStackSize + "/" + _item.maxStackSize + ", " + _item.GetTypeDisplayName() + "\n" + _item.m_description, 
            _showCost ? _item.GetPrice().ToString() : _item.GetTypeDisplayName(), 
            _showCost ? m_priceIcon : m_typeIcon, 
            _onTime, 
            _offTime
        );

        // stats and mods panel
        DisplayStatsLeft(_item);
        DisplayModsRight(_item.GetStatMods());
    }

    /// <summary>
    /// Update the stats based on the item type
    /// @todo - add more types
    /// </summary>
    /// <param name="_item"></param>
    private void DisplayStatsLeft(Item _item)
    {
        if (!_item) return;

        System.Type type = _item.GetType();

        // RangedWeapon
        if (type.IsSubclassOf(typeof(RangedWeapon)) || type == typeof(RangedWeapon)){

            RangedWeapon weapon = (RangedWeapon)_item;

            string newDesc = "";

            // damage
            float calcDamage = StatsManager.CalculateDamage(weapon, weapon.m_damage);
            float damageDifference = calcDamage - weapon.m_damage;
            string damageModString = damageDifference != 0 ? " (" + StatsManager.SignedFloatString(damageDifference) + ")" : "";
            // red for negative, green for positive, none for 0
            string calcWithColor = damageDifference > 0 ? "<color=\"green\">" + calcDamage.ToString("0.#") + "</color>" : damageDifference < 0 ? "<color=\"red\">" + calcDamage.ToString("0.#") + "</color>" : calcDamage.ToString("0.#");
            newDesc += "Damage: " + calcWithColor + damageModString + "\n";

            // range
            float calcRange = StatsManager.CalculateRange(weapon, weapon.m_range);
            float rangeDifference = calcRange - weapon.m_range;
            string rangeModString = rangeDifference != 0 ? " (" + StatsManager.SignedFloatString(rangeDifference) + ")" : "";
            // red for negative, green for positive, none for 0
            calcWithColor = rangeDifference > 0 ? "<color=\"green\">" + calcRange.ToString("0.#") + "</color>" : rangeDifference < 0 ? "<color=\"red\">" + calcRange.ToString("0.#") + "</color>" : calcRange.ToString("0.#");
            newDesc += "Range: " + calcWithColor + rangeModString + "\n";

            // Clip
            newDesc += "Clip Size: " + weapon.m_clipAmmo + "/" + weapon.m_clipSize + "\n";

            // max spare ammo
            int calcMaxSpare = weapon.CalcedMaxSpareAmmo;
            float maxSpareDifference = calcMaxSpare - weapon.m_maxSpareAmmo;
            string maxSpareModString = maxSpareDifference != 0 ? " (" + StatsManager.SignedFloatString(maxSpareDifference) + ")" : "";
            // red for negative, green for positive, none for 0
            calcWithColor = maxSpareDifference > 0 ? "<color=\"green\">" + calcMaxSpare + "</color>" : maxSpareDifference < 0 ? "<color=\"red\">" + calcMaxSpare + "</color>" : calcMaxSpare.ToString();
            newDesc += "Spare Ammo: " + weapon.m_spareAmmo + "/" + calcWithColor + maxSpareModString + "\n";

            DisplayLeft("Stats", m_statsIcon, newDesc);
        }

        // melee
        else if (type.IsSubclassOf(typeof(MeleeWeapon)) || type == typeof(MeleeWeapon)){

            MeleeWeapon weapon = (MeleeWeapon)_item;

            string newDesc = "";

            // damage
            float calcDamage = StatsManager.CalculateDamage(weapon, weapon.m_damage);
            float damageDifference = calcDamage - weapon.m_damage;
            string damageModString = damageDifference != 0 ? " (" + StatsManager.SignedFloatString(damageDifference) + ")" : "";
            // red for negative, green for positive, none for 0
            string calcWithColor = damageDifference > 0 ? "<color=\"green\">" + calcDamage.ToString("0.#") + "</color>" : damageDifference < 0 ? "<color=\"red\">" + calcDamage.ToString("0.#") + "</color>" : calcDamage.ToString("0.#");
            newDesc += "Damage: " + calcWithColor + damageModString + "\n";

            // cooldown
            //float calcCooldown = StatsManager.CalculateCooldown(weapon, weapon.m_swingCooldown);
            //float cooldownDifference = calcCooldown - weapon.m_swingCooldown;
            //string cooldownModString = cooldownDifference != 0 ? " (" + StatsManager.SignedFloatString(cooldownDifference) + ")" : "";
            newDesc += "Cooldown: " + weapon.m_swingCooldown + "\n";

            DisplayLeft("Stats", m_statsIcon, newDesc);
        }

        // Equipment
        if (type.IsSubclassOf(typeof(Equipment)) || type == typeof(Equipment)){

            Equipment equipment = (Equipment)_item;

            // get prefab
            GameObject prefab = equipment.m_equipmentPrefab;
            if (!prefab) return;

            string newDesc = "";

            // medkit
            if (prefab.GetComponentInChildren<MedkitEquipment>()){
                MedkitEquipment medkit = prefab.GetComponentInChildren<MedkitEquipment>();
                
                // health
                // float calcHeal = StatsManager.CalculateHeal(medkit, medkit.m_heal);
                newDesc += "Heal Amount: " + medkit.healAmount.ToString("0.#") + "\n";
            }
            // dynamite
            else if (prefab.GetComponentInChildren<Dynamite>()){
                Dynamite dynamite = prefab.GetComponentInChildren<Dynamite>();

                // explosion prefab
                GameObject explosionPrefab = dynamite.explosionPrefab;
                if (explosionPrefab != null && explosionPrefab.GetComponentInChildren<AreaDamage>()){
                    AreaDamage areaDamage = explosionPrefab.GetComponentInChildren<AreaDamage>();

                    // DPS
                    float calcDPS = StatsManager.CalculateDamage(areaDamage.m_statsProfile, areaDamage.m_damage) /areaDamage.m_damageDelay;
                    float damageDifference = calcDPS - (areaDamage.m_damage /areaDamage.m_damageDelay);
                    string damageModString = damageDifference != 0 ? " (" + StatsManager.SignedFloatString(damageDifference) + ")" : "";
                    // red for negative, green for positive, none for 0
                    string calcWithColor = damageDifference > 0 ? "<color=\"green\">" + calcDPS.ToString("0.#") + "</color>" : damageDifference < 0 ? "<color=\"red\">" + calcDPS.ToString("0.#") + "</color>" : calcDPS.ToString("0.#");
                    newDesc += "DPS: " + calcWithColor + damageModString + "\n";

                    // radius
                    // float calcRadius = StatsManager.CalculateRadius(areaDamage.m_statsProfile, areaDamage.m_radius);
                    newDesc += "Radius: " + areaDamage.m_radius.ToString("0.#") + "\n";

                    // delay
                    // float calcDelay = StatsManager.CalculateDelay(areaDamage.m_statsProfile, areaDamage.m_damageDelay);
                    newDesc += "Duration: " + areaDamage.m_time.ToString("0.#") + "\n";
                }
                else{

                    // damage
                    float calcDamage = StatsManager.CalculateDamage(dynamite.m_statsProfile, dynamite.m_damage);
                    float damageDifference = calcDamage - dynamite.m_damage;
                    string damageModString = damageDifference != 0 ? " (" + StatsManager.SignedFloatString(damageDifference) + ")" : "";
                    // red for negative, green for positive, none for 0
                    string calcWithColor = damageDifference > 0 ? "<color=\"green\">" + calcDamage.ToString("0.#") + "</color>" : damageDifference < 0 ? "<color=\"red\">" + calcDamage.ToString("0.#") + "</color>" : calcDamage.ToString("0.#");
                    newDesc += "Damage: " + calcWithColor + damageModString + "\n";

                    // radius
                    // float calcRadius = StatsManager.CalculateRadius(dynamite.m_statsProfile, dynamite.m_radius);
                    newDesc += "Radius: " + dynamite.m_explosionRadius.ToString("0.#") + "\n";
                }

                
                newDesc += "Fuse Time: " + dynamite.m_fuseTime.ToString("0.#") + "\n";
            }
            // beartrap
            else if (prefab.GetComponentInChildren<Beartrap>()){
                Beartrap beartrap = prefab.GetComponentInChildren<Beartrap>();

                // damage
                float calcDamage = StatsManager.CalculateDamage(beartrap.m_statsProfile, beartrap.m_damage);
                float damageDifference = calcDamage - beartrap.m_damage;
                string damageModString = damageDifference != 0 ? " (" + StatsManager.SignedFloatString(damageDifference) + ")" : "";
                // red for negative, green for positive, none for 0
                string calcWithColor = damageDifference > 0 ? "<color=\"green\">" + calcDamage.ToString("0.#") + "</color>" : damageDifference < 0 ? "<color=\"red\">" + calcDamage.ToString("0.#") + "</color>" : calcDamage.ToString("0.#");
                newDesc += "Damage: " + calcWithColor + damageModString + "\n";
            }
            else {
                return;
            }

            DisplayLeft("Stats", m_statsIcon, newDesc);
        }
    }

    /// <summary>
    /// Updates the modifier section of the info box with current info
    /// </summary>
    /// <param name="_mods"></param>
    public void DisplayModsRight(List<StatsManager.StatMod> _mods)
    {
        if (_mods.Count > 0)
        {
            string newDesc = "";
            foreach (StatsManager.StatMod mod in _mods)
            {
                newDesc += mod.ToText() + "\n";
            }

            DisplayRight("Modifiers", m_modifierIcon, newDesc);
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
    private void DisableAll()
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
    private void EnableAll()
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
