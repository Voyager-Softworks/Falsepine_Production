using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FloatingTextPopup : MonoBehaviour
{
    private TextMeshProUGUI m_text;
    private RectTransform m_transform;
    public float m_startFadeDelay = 5.0f;
    private float m_startFadeDelayTimer = 0f;
    public float m_fadeTime = 1.0f;
    [ReadOnly] public float m_fadeTimer = 0.0f;
    public bool m_doFade = true;
    // public bool m_doMove = true;

    // [Tooltip("How far up the text should go relative to its current size")] 
    // private float m_currentXVelocity = 0.0f;
    // private float m_desiredXVelocity = 0.0f;
    // public float m_xMoveSpeed = 1.0f;
    // public float m_currentYVelocity = 0.0f;
    // public float m_desiredYVelocity = 0.0f;
    // public float m_yMoveSpeed = 5.0f;

    // Start is called before the first frame update
    void Start()
    {
        m_text = GetComponent<TextMeshProUGUI>();
        m_transform = GetComponent<RectTransform>();

        //Destroy(gameObject, m_startFadeDelay + m_fadeTime);  

        //m_desiredXVelocity = Random.Range(-m_xMoveSpeed, m_xMoveSpeed);
        //m_desiredYVelocity = Random.Range(m_yMoveSpeed, m_yMoveSpeed * 2);
    }

    // Update is called once per frame
    void Update()
    {
        // move up proportional to size if should
        // if (m_doMove)
        // {
        //     m_currentXVelocity = Mathf.Lerp(m_currentXVelocity, m_desiredXVelocity, Time.deltaTime);
        //     m_currentYVelocity = Mathf.Lerp(m_currentYVelocity, m_desiredYVelocity, Time.deltaTime);

        //     m_transform.localPosition += new Vector3(m_currentXVelocity * Time.deltaTime, m_currentYVelocity * Time.deltaTime, 0.0f);
        // }

        // fade out if should
        if (m_doFade)
        {
            // update delay timer
            if (m_startFadeDelayTimer < m_startFadeDelay)
            {
                m_startFadeDelayTimer += Time.deltaTime;
            }
            else
            {
                // fade out, then destroy
                if (m_fadeTimer < m_fadeTime)
                {
                    m_fadeTimer += Time.deltaTime;

                    float alpha = 1.0f - (m_fadeTimer / m_fadeTime);
                    m_text.color = new Color(m_text.color.r, m_text.color.g, m_text.color.b, alpha);
                }
                else
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}
