using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DamagePopup : MonoBehaviour
{
    public TextMeshProUGUI m_text;

    [Header("Appearance")]
    public float m_minSize = 50;
    public float m_maxSize = 75;
    public Color m_minColor = Color.white;
    public Color m_maxColor = Color.red;
    public float m_lowerDamageValue = 10;
    public float m_upperDamageValue = 40;

    [Header("Fading")]
    private float m_textAlpha;
    public float m_startFadeDelay = 1.0f;
    [ReadOnly] public float m_startFadeDelayTimer = 0f;
    public float m_fadeTime = 1.0f;
    [ReadOnly] public float m_fadeTimer = 0.0f;
    public bool m_doFade = true;

    [Header("Movement")]
    public float m_initialMoveSpeed = 3.0f;
    public float m_initialUpSpeed = 3.0f;
    public float m_gravity = -9.8f;


    private void Awake() {
        if (m_text == null) {
            m_text = GetComponent<TextMeshProUGUI>();
        }

        m_textAlpha = m_text.color.a;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
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

    private void FixedUpdate() {
        // gravity
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.AddForce(Vector3.up * m_gravity * rb.mass);
    }
    
    public void SetRelativeOpacity(float alpha)
    {
        m_text.color = new Color(m_text.color.r, m_text.color.g, m_text.color.b, m_textAlpha * alpha);
    }

    public void ShowPopup(Health_Base.DamageStat _damageStat)
    {
        if (_damageStat == null){
            // destroy if no damage stat
            Destroy(gameObject);
        }

        // set text
        m_text.text = _damageStat.m_damage.ToString("0.0");

        // set size
        float size = Mathf.Lerp(m_minSize, m_maxSize, (_damageStat.m_damage - m_lowerDamageValue) / (m_upperDamageValue - m_lowerDamageValue));
        m_text.fontSize = size;

        // set color
        Color color = Color.Lerp(m_minColor, m_maxColor, (_damageStat.m_damage - m_lowerDamageValue) / (m_upperDamageValue - m_lowerDamageValue));
        m_text.color = color;

        // store alpha
        m_textAlpha = m_text.color.a;

        // force
        Rigidbody rb = GetComponent<Rigidbody>();
        // set velocity to direction of damage
        Vector3 vel = _damageStat.direction.normalized * m_initialMoveSpeed;
        // set upwards velocity
        vel.y = m_initialUpSpeed;
        rb.velocity = vel;
    }
}
