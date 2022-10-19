using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneCinematicManager : MonoBehaviour
{
    public MissionZone.ZoneArea zoneArea;
    public bool m_disableCinematic = true;
    public Animator m_animator;
    [Tooltip("If no animator is given, the timer will be used instead")] public float m_fallbackTime = 5.0f;
    private float m_fallbackTimer = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        // check if animation is done
        if (m_animator != null && m_animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !m_animator.IsInTransition(0))
        {
            OnCinematicEnd();
        }

        // if no animator, fallback timer
        if (m_animator == null)
        {
            m_fallbackTimer += Time.deltaTime;
            if (m_fallbackTimer >= m_fallbackTime)
            {
                OnCinematicEnd();
            }
        }
    }

    public void OnCinematicEnd()
    {
        MissionManager.instance.LoadNextScene();

        if (m_disableCinematic)
        {
            MissionManager.instance.GetZone(zoneArea).m_doCinematic = false;
        }
    }
}
