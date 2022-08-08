using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;
using System;
using UnityEngine.Events;

/// <summary>
/// @deprecated not used anymore. <br/>
/// Class to manage old UI for the message popup.
/// </summary>
public class MessageScript : MonoBehaviour
{
    public float startFadeDelay = 3.0f;
    private float startFadeDelayTimer = 0f;
    public float fadeTime = 1.0f;
    private float fadeTimer = 0.0f;

    public Image container;
    public TextMeshProUGUI text;

    // Start is called before the first frame update
    void Start()
    {
        if (container == null) container = GetComponent<Image>();
        if (text == null) text = GetComponentInChildren<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if (startFadeDelayTimer < startFadeDelay)
        {
            startFadeDelayTimer += Time.deltaTime;
        }
        else
        {
            if (fadeTimer < fadeTime)
            {
                fadeTimer += Time.deltaTime;

                float alpha = 1.0f - (fadeTimer / fadeTime);
                container.color = new Color(container.color.r, container.color.g, container.color.b, alpha);
                text.color = new Color(text.color.r, text.color.g, text.color.b, alpha);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    public void SetMessage(string _message)
    {
        if (text == null) text = GetComponentInChildren<TextMeshProUGUI>();
        if (text == null) return;
        
        text.text = _message;
    }
}
