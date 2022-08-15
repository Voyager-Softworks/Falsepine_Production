using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class BottomText : MonoBehaviour
{
    [Serializable]
    public class TextRequest{
        public string m_text;
        public GameObject m_requester;
        public float m_interactDistance = 1f;
        public float? m_persistTime = null;
        public float? m_fadeInSpeed = null;
        public float? m_fadeOutSpeed = null;

        public TextRequest(string _text, GameObject _requester, float _interactDistance, float? _persistTime = null, float? _fadeInSpeed = null, float? _fadeOutSpeed = null){
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

    private float m_persistTime = 0.25f;
    private float m_persistTimer = 0.0f;
    public float m_defaultPersistTime = 0.25f;

    private float m_fadeInSpeed = 10.0f;
    public float m_defaultFadeInSpeed = 10.0f;

    private float m_fadeOutSpeed = 10.0f;
    public float m_defaultFadeOutSpeed = 10.0f;

    private TextMeshProUGUI m_text = null;

    private List<TextRequest> m_textRequests = new List<TextRequest>();

    [Header("Refs")]
    Transform m_playerTransform = null;

    // Start is called before the first frame update
    void Start()
    {
        m_text = GetComponent<TextMeshProUGUI>();

        m_playerTransform = GameObject.FindObjectOfType<PlayerMovement>()?.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_text == null) return;

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
            m_text.color = new Color(m_text.color.r, m_text.color.g, m_text.color.b, Mathf.Lerp(m_text.color.a, 1.0f, Time.deltaTime * m_fadeInSpeed));
        }
        else{
            // fade out to 0 alpha
            m_text.color = new Color(m_text.color.r, m_text.color.g, m_text.color.b, Mathf.Lerp(m_text.color.a, 0.0f, Time.deltaTime * m_fadeOutSpeed));
        }
    }

    private void SetBottomText(TextRequest _trq)
    {
        if (m_text == null) return;

        m_text.text = _trq.m_text;
        m_persistTime = _trq.m_persistTime == null ? m_defaultPersistTime : (float)_trq.m_persistTime;
        m_persistTimer = m_persistTime;

        m_fadeInSpeed = _trq.m_fadeInSpeed == null ? m_defaultFadeInSpeed : (float)_trq.m_fadeInSpeed;
        m_fadeOutSpeed = _trq.m_fadeOutSpeed == null ? m_defaultFadeOutSpeed : (float)_trq.m_fadeOutSpeed;
    }

    public void RequestBottomText(TextRequest _trq)
    {
        // if list already contains the same text request, don't add it again
        foreach (TextRequest tr in m_textRequests)
        {
            if (tr.IsTheSame(_trq)) return;
        }
        m_textRequests.Add(_trq);
    }

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
