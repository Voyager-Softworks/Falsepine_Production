using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class FloatingTextPopup : MonoBehaviour
{
    [Header("References")]
    public TextMeshProUGUI m_text;
    public Image m_image;
    public Image m_background;
    private float m_textAlpha;
    private float m_backgroundAlpha;
    private float m_imageAlpha;

    [Header("Fade")]
    public float m_startFadeDelay = 5.0f;
    [ReadOnly] public float m_startFadeDelayTimer = 0f;
    public float m_fadeTime = 1.0f;
    [ReadOnly] public float m_fadeTimer = 0.0f;
    public bool m_doFade = true;

    [Header("Movement")]
    public float m_targetMoveSpeed = 1.0f;
    private float m_moveSpeed = 0f;
    public float m_accelTime = 1.0f;
    [ReadOnly] public float m_accelTimer = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        m_textAlpha = m_text.color.a;
        m_backgroundAlpha = m_background.color.a;
        m_imageAlpha = m_image.color.a;
    }

    // Update is called once per frame
    void Update()
    {
        // accelerate
        m_accelTimer += Time.deltaTime;
        m_moveSpeed = Mathf.Lerp(0f, m_targetMoveSpeed, m_accelTimer / m_accelTime);

        // move
        transform.position += new Vector3(0f, m_moveSpeed * Time.deltaTime, 0f);

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
                    SetRelativeOpacity(alpha);
                }
                else
                {
                    Destroy(gameObject);
                }
            }
        }
    }

    public void SetIcon(string _icon){
        string sprite = (string.IsNullOrEmpty(_icon) ? "" : " <sprite name=\"" + _icon + "\">");
        m_text.text = sprite;
    }

    public void SetAbsoluteOpacity(float alpha)
    {
        m_text.color = new Color(m_text.color.r, m_text.color.g, m_text.color.b, alpha);
        m_image.color = new Color(m_image.color.r, m_image.color.g, m_image.color.b, alpha);
        m_background.color = new Color(m_background.color.r, m_background.color.g, m_background.color.b, alpha);
    }
    
    public void SetRelativeOpacity(float alpha)
    {
        m_text.color = new Color(m_text.color.r, m_text.color.g, m_text.color.b, m_textAlpha * alpha);
        m_image.color = new Color(m_image.color.r, m_image.color.g, m_image.color.b, m_imageAlpha * alpha);
        m_background.color = new Color(m_background.color.r, m_background.color.g, m_background.color.b, m_backgroundAlpha * alpha);
    }

    public void ResetTimers(){
        m_startFadeDelayTimer = 0f;
        m_fadeTimer = 0.0f;
    }
}
