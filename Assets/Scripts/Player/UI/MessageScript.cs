using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;
using System;
using UnityEngine.Events;

/// <summary>
/// Class to manage UI for the message popup.
/// </summary>
public class MessageScript : MonoBehaviour
{
    public float m_startFadeDelay = 5.0f;
    private float m_startFadeDelayTimer = 0f;
    public float m_fadeTime = 1.0f;
    private float m_fadeTimer = 0.0f;
    public bool m_doFade = false;

    [Header("References")]
    public Image m_container;
    public TextMeshProUGUI m_text;
    public Button m_journalButton;
    [ReadOnly] public JounralEntry m_linkedJournalEntry;

    // Start is called before the first frame update
    void Start()
    {
        if (m_container == null) m_container = GetComponent<Image>();
        if (m_text == null) m_text = GetComponentInChildren<TextMeshProUGUI>();
    }

    private void OnEnable() {
        // bind button
        GetComponent<Button>().onClick.AddListener(ButtonClicked);

        // bind journal button
        m_journalButton.onClick.AddListener(JournalButtonPressed);
    }

    private void OnDisable() {
        // unbind button
        GetComponent<Button>().onClick.RemoveAllListeners();

        // unbind journal button
        m_journalButton.onClick.RemoveAllListeners();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_doFade)
        {
            if (m_startFadeDelayTimer < m_startFadeDelay)
            {
                m_startFadeDelayTimer += Time.deltaTime;
            }
            else
            {
                if (m_fadeTimer < m_fadeTime)
                {
                    m_fadeTimer += Time.deltaTime;

                    float alpha = 1.0f - (m_fadeTimer / m_fadeTime);
                    m_container.color = new Color(m_container.color.r, m_container.color.g, m_container.color.b, alpha);
                    m_text.color = new Color(m_text.color.r, m_text.color.g, m_text.color.b, alpha);
                }
                else
                {
                    Destroy(gameObject);
                }
            }
        }
    }

    /// <summary>
    /// Sets the text of the message.
    /// </summary>
    /// <param name="_message"></param>
    /// <param name="_icon"></param>
    /// <param name="_doFade"></param>
    public void SetMessage(string _message, string _icon = "", bool _doFade = false, JounralEntry _linkedJournalEntry = null)
    {
        if (m_text == null) m_text = GetComponentInChildren<TextMeshProUGUI>();
        if (m_text == null) return;

        m_doFade = _doFade;

        string sprite = (string.IsNullOrEmpty(_icon) ? "" : " <sprite name=\"" + _icon + "\">");

        m_text.text = _message + sprite + (!_doFade ? " <sprite name=\"cross\">" : "");

        m_linkedJournalEntry = _linkedJournalEntry;
        if (m_linkedJournalEntry != null)
        {
            m_journalButton.gameObject.SetActive(true);
        }
        else
        {
            m_journalButton.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Dismissses the message.
    /// </summary>
    public void ButtonClicked(){
        if (!m_doFade)
        {
            m_doFade = true;
            m_startFadeDelay = 0f;
            m_fadeTime = 0.5f;
        }
        else{
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Opens the journal entry.
    /// </summary>
    public void JournalButtonPressed(){
        if (m_linkedJournalEntry != null)
        {
            JournalManager.instance?.OpenToEntry(m_linkedJournalEntry);
        }
    }
}
