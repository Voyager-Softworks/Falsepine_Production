using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// @deprecated
/// </summary>
public class MessageManager : MonoBehaviour
{
    public static MessageManager instance;

    public GameObject m_contentPanel;
    public GameObject messagePrefab;

    public List<GameObject> m_messages = new List<GameObject>();
    public float m_messageDuration = 5f;
    public float m_fadeDuration = 1f;

    public AudioClip m_messageSound;

    [ReadOnly] public List<string> m_oldMessages = new List<string>();

    private void Awake() {
        if (instance == null)
        {
            instance = this;
            //do not destroy this object
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Makes new message instance appear on screen with text.
    /// </summary>
    /// <param name="_messsage"></param>
    /// <param name="_icon"></param>
    /// <param name="_doFade"></param>
    public void AddMessage(string _messsage, string _icon = "", bool _doFade = false, JounralEntry _linkedJournalEntry = null) {
        m_oldMessages.Add(_messsage);

        GameObject message = Instantiate(messagePrefab, m_contentPanel.transform);
        m_messages.Add(message);

        message.GetComponentInChildren<MessageScript>().SetMessage(_messsage, _icon, _doFade: _doFade, _linkedJournalEntry: _linkedJournalEntry);

        // force layout update for all children
        RectTransform[] children = m_contentPanel.GetComponentsInChildren<RectTransform>();
        foreach (RectTransform child in children)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(child);
        }
        // self
        LayoutRebuilder.ForceRebuildLayoutImmediate(m_contentPanel.GetComponent<RectTransform>());
    }
}
