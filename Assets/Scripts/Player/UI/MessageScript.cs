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

    public Image m_container;
    public TextMeshProUGUI m_text;

    // Start is called before the first frame update
    void Start()
    {
        if (m_container == null) m_container = GetComponent<Image>();
        if (m_text == null) m_text = GetComponentInChildren<TextMeshProUGUI>();
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
                m_container.color = new Color(m_container.color.r, m_container.color.g, m_container.color.b, alpha);
                m_text.color = new Color(m_text.color.r, m_text.color.g, m_text.color.b, alpha);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    public void SetMessage(string _message, string _icon = "", bool _before = false)
    {
        if (m_text == null) m_text = GetComponentInChildren<TextMeshProUGUI>();
        if (m_text == null) return;

        string sprite = (string.IsNullOrEmpty(_icon) ? "" : " <sprite name=\"" + _icon + "\">");

        m_text.text = (_before ? sprite + " " : "") + _message + (_before ? "" : " " + sprite);
    }
}
