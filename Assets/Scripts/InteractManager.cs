using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

/// <summary>
/// This class manages the interaction between the player and the interactable objects.
/// </summary>
public class InteractManager : MonoBehaviour
{
    static InteractManager instance = null;

    [Serializable]
    public class TextRequest{
        public string m_text;
        public Interactable m_requester;
        public float m_interactDistance = 1f;
        public float? m_persistTime = null;
        public float? m_fadeInSpeed = null;
        public float? m_fadeOutSpeed = null;

        public TextRequest(string _text, Interactable _requester, float _interactDistance, float? _persistTime = null, float? _fadeInSpeed = null, float? _fadeOutSpeed = null){
            this.m_text = _text;
            this.m_requester = _requester;
            this.m_interactDistance = _interactDistance;
            this.m_persistTime = _persistTime;
            this.m_fadeInSpeed = _fadeInSpeed;
            this.m_fadeOutSpeed = _fadeOutSpeed;
        }

        // custom IsTheSame method to compare two TextRequests
        public bool IsTheSame(TextRequest _other){
            if (this.m_text != _other.m_text) return false;
            if (this.m_requester != _other.m_requester) return false;
            return true;
        }
    }

    [Header("On Screen Text")]
    private float m_persistTime = 0.25f;
    private float m_persistTimer = 0.0f;
    public float m_defaultPersistTime = 0.25f;

    private float m_fadeInSpeed = 10.0f;
    public float m_defaultFadeInSpeed = 10.0f;

    private float m_fadeOutSpeed = 10.0f;
    public float m_defaultFadeOutSpeed = 10.0f;

    private TextMeshProUGUI m_text = null;
    private Image m_background = null;

    private List<TextRequest> m_textRequests = new List<TextRequest>();


    private bool m_interactedThisFrame = false;


    [Header("Refs")]
    Transform m_playerTransform = null;

    void Awake() {
        if (instance == null) {
            instance = this;
            //do not destroy this object
            DontDestroyOnLoad(this);
        } else {
            Destroy(this);
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        UIScript ui = FindObjectOfType<UIScript>();
        if (ui != null) {
            m_text = ui.interactText;
            m_background = ui.interactBackground;
        }

        m_playerTransform = GameObject.FindObjectOfType<PlayerMovement>()?.transform;
    }

    // Update is called once per frame
    void Update()
    {
        // remove any text requests that are too far away or invalid
        for (int i = 0; i < m_textRequests.Count; i++){
            if (m_textRequests[i].m_requester == null || Vector3.Distance(m_playerTransform.position, m_textRequests[i].m_requester.transform.position) > m_textRequests[i].m_interactDistance){
                m_textRequests.RemoveAt(i);
                i--;
            }
        }

        // sort list by distance from player
        m_textRequests.Sort((x, y) => {
            float xDist = Vector3.Distance(m_playerTransform.position, x.m_requester.transform.position);
            float yDist = Vector3.Distance(m_playerTransform.position, y.m_requester.transform.position);
            return xDist.CompareTo(yDist);
        });


        // check if any actions are pressed
        m_interactedThisFrame = false;
        for (int i = 0; i < m_textRequests.Count; i++){
            if (m_interactedThisFrame) break;
            
            Interactable requester = m_textRequests[i].m_requester;
            if (requester == null) continue;
            m_interactedThisFrame = requester.CheckActionPressed();
        }

        // set text
        if (m_textRequests.Count > 0)
        {
            SetBottomText(m_textRequests[0]);
            // remove text request
            m_textRequests.RemoveAt(0);
        }

        if (m_persistTimer > 0.0f)
        {
            m_persistTimer -= Time.deltaTime;

            // fade in to full alpha
            float alpha = Mathf.Lerp(m_text.color.a, 1.0f, Time.deltaTime * m_fadeInSpeed);
            m_text.color = new Color(m_text.color.r, m_text.color.g, m_text.color.b, alpha);
            m_background.color = new Color(m_background.color.r, m_background.color.g, m_background.color.b, alpha);
        }
        else{
            // fade out to 0 alpha
            float alpha = Mathf.Lerp(m_text.color.a, 0.0f, Time.deltaTime * m_fadeOutSpeed);
            m_text.color = new Color(m_text.color.r, m_text.color.g, m_text.color.b, alpha);
            m_background.color = new Color(m_background.color.r, m_background.color.g, m_background.color.b, alpha);
        }
    }

    /// <summary>
    /// Sets the text on screen based on the given request.
    /// </summary>
    /// <param name="_trq"></param>
    private void SetBottomText(TextRequest _trq)
    {
        if (m_text == null) return;

        m_text.text = _trq.m_text;
        m_persistTime = _trq.m_persistTime == null ? m_defaultPersistTime : (float)_trq.m_persistTime;
        m_persistTimer = m_persistTime;

        m_fadeInSpeed = _trq.m_fadeInSpeed == null ? m_defaultFadeInSpeed : (float)_trq.m_fadeInSpeed;
        m_fadeOutSpeed = _trq.m_fadeOutSpeed == null ? m_defaultFadeOutSpeed : (float)_trq.m_fadeOutSpeed;
    }

    /// <summary>
    /// Request some text to be shown on screen (requests are stored and sorted by distance from player).
    /// </summary>
    /// <param name="_trq"></param>
    public void RequestBottomText(TextRequest _trq)
    {
        // if list already contains the same text request, don't add it again
        foreach (TextRequest tr in m_textRequests)
        {
            if (tr.IsTheSame(_trq)) return;
        }
        m_textRequests.Add(_trq);
    }

    /// <summary>
    /// Remove a request from the list.
    /// </summary>
    /// <param name="_trq"></param>
    public void RemoveRequest(TextRequest _trq)
    {
        foreach (TextRequest tr in m_textRequests)
        {
            if (tr.IsTheSame(_trq))
            {
                m_textRequests.Remove(tr);
                return;
            }
        }
    }
}
