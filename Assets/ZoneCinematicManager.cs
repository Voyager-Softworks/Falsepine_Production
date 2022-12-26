using System.Collections;
using System.Collections.Generic;
using Achievements;
using UnityEngine;
using UnityEngine.InputSystem;

public class ZoneCinematicManager : MonoBehaviour
{
    public MissionZone.ZoneArea zoneArea;
    public bool m_disableCinematic = true;
    public Animator m_animator;
    [Tooltip("If no animator is given, the timer will be used instead")] public float m_fallbackTime = 5.0f;
    private float m_fallbackTimer = 0.0f;
    private bool m_isEnding = false;

    [Header("Credits")]
    public bool m_isCredits = false;

    // Start is called before the first frame update
    void Start()
    {
        m_isEnding = false;
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

        // if escape is pressed, skip cinematic
        if (Keyboard.current.escapeKey.wasPressedThisFrame || (CustomInputManager.LastInputWasGamepad && Gamepad.current.buttonEast.wasPressedThisFrame))
        {
            OnCinematicEnd();
        }
    }

    public void OnCinematicEnd()
    {
        if (m_isEnding) return;
        m_isEnding = true;
        if (FindObjectOfType<AchievementsManager>() is AchievementsManager am)
        {
            switch (zoneArea)
            {
                case MissionZone.ZoneArea.SNOW:
                    am.UnlockAchievement(AchievementsManager.Achievement.DiscoverBonestag);
                    break;
                case MissionZone.ZoneArea.DESERT:
                    am.UnlockAchievement(AchievementsManager.Achievement.DiscoverOmen);
                    break;
                case MissionZone.ZoneArea.REDWOOD:
                    am.UnlockAchievement(AchievementsManager.Achievement.DiscoverBrightmaw);
                    break;
                case MissionZone.ZoneArea.SWAMP:
                    am.UnlockAchievement(AchievementsManager.Achievement.DiscoverWailingTree);
                    break;
                case MissionZone.ZoneArea.PRIME:
                    am.UnlockAchievement(AchievementsManager.Achievement.DiscoverPrimaeval);
                    break;
            }
        }
        if (m_isCredits)
        {
            LevelController.LoadGameComplete();
            return;
        }

        MissionManager.instance.LoadNextScene();

        if (m_disableCinematic)
        {
            MissionManager.instance.GetZone(zoneArea).m_doCinematic = false;
        }
    }
}
