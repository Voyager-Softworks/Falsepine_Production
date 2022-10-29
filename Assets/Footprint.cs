using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class Footprint : MonoBehaviour
{
    [Header("Fade")]
    public float m_startFadeDelay = 1.0f;
    [ReadOnly] public float m_startFadeDelayTimer = 0f;
    public float m_fadeTime = 1.0f;
    [ReadOnly] public float m_fadeTimer = 0.0f;
    public bool m_doFade = true;

    private DecalProjector m_decalProjector;
    public DecalProjector decalProjector { 
        get { 
            if (m_decalProjector == null) m_decalProjector = GetComponentInChildren<DecalProjector>();
            return m_decalProjector;
        } 
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
                    SetOpacity(alpha);
                }
                else
                {
                    Destroy(gameObject);
                }
            }
        }
    }

    public void SetOpacity(float alpha)
    {
        decalProjector.fadeFactor = alpha;
    }
}
