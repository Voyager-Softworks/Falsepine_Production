using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;

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

    [Header("Mods Refs")]
    public Image modBackground;
    public Image modIcon;
    public TextMeshProUGUI modTitle;
    public TextMeshProUGUI modList;


    // Start is called before the first frame update
    void Start()
    {
        DisableBox();
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
            background.color = new Color(background.color.r, background.color.g, background.color.b, 1.0f);
            title.color = new Color(title.color.r, title.color.g, title.color.b, 1.0f);
            icon.color = new Color(icon.color.r, icon.color.g, icon.color.b, 1.0f);
            description.color = new Color(description.color.r, description.color.g, description.color.b, 1.0f);

            modBackground.color = new Color(modBackground.color.r, modBackground.color.g, modBackground.color.b, 1.0f);
            modTitle.color = new Color(modTitle.color.r, modTitle.color.g, modTitle.color.b, 1.0f);
            modList.color = new Color(modList.color.r, modList.color.g, modList.color.b, 1.0f);
            modIcon.color = new Color(modIcon.color.r, modIcon.color.g, modIcon.color.b, 1.0f);

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

            background.color = new Color(background.color.r, background.color.g, background.color.b, opacity);
            title.color = new Color(title.color.r, title.color.g, title.color.b, opacity);
            icon.color = new Color(icon.color.r, icon.color.g, icon.color.b, opacity);
            description.color = new Color(description.color.r, description.color.g, description.color.b, opacity);

            modBackground.color = new Color(modBackground.color.r, modBackground.color.g, modBackground.color.b, opacity);
            modTitle.color = new Color(modTitle.color.r, modTitle.color.g, modTitle.color.b, opacity);
            modList.color = new Color(modList.color.r, modList.color.g, modList.color.b, opacity);

            if (fadeTimer <= 0.0f)
            {
                fadeTimer = 0.0f;
                DisableBox();
                DisableModsBox();
            }
        }
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

    /// <summary>
    /// Display an Item in the info box at cursor.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="onTime"></param>
    /// <param name="offTime"></param>
    public void Display(Item item, float onTime = 1, float offTime = 1)
    {
        if (!item) return;
        DisplayInfo(item.m_displayName, item.m_icon, item.m_description, onTime, offTime);
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
