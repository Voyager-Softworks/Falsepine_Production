using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class InfoBox : MonoBehaviour
{
    //can be called from other scripts to display a message for a certain amount of time

    public float fullBrightTime = 1.0f;
    private float fullBrightTimer = 0.0f;

    public float fadeTime = 1.0f;
    private float fadeTimer = 0.0f;

    [Header("Refs")]
    public Image background;
    public TextMeshProUGUI title;
    public Image icon;
    public TextMeshProUGUI description;


    // Start is called before the first frame update
    void Start()
    {
        DisableBox();
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

            EnableBox();

            // set opacity of all to 1
            background.color = new Color(background.color.r, background.color.g, background.color.b, 1.0f);
            title.color = new Color(title.color.r, title.color.g, title.color.b, 1.0f);
            icon.color = new Color(icon.color.r, icon.color.g, icon.color.b, 1.0f);
            description.color = new Color(description.color.r, description.color.g, description.color.b, 1.0f);

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

            if (fadeTimer <= 0.0f)
            {
                fadeTimer = 0.0f;
                DisableBox();
            }
        }
    }

    public void Display(string title, Sprite icon, string description, float onTime = 1, float offTime = 1)
    {
        this.title.text = title;
        this.icon.sprite = icon;
        this.description.text = description;

        fullBrightTime = onTime;
        fullBrightTimer = fullBrightTime;

        fadeTime = offTime;
        fadeTimer = fadeTime;

        EnableBox();
    }

    public void Display(Item item, float onTime = 1, float offTime = 1){
        if (!item) return;
        Display(item.m_displayName, item.m_icon, item.m_description, onTime, offTime);
    }

    private void DisableBox(){
        background.enabled = false;
        title.enabled = false;
        icon.enabled = false;
        description.enabled = false;
    }

    private void EnableBox(){
        background.enabled = true;
        title.enabled = true;
        icon.enabled = true;
        description.enabled = true;
    }
}
