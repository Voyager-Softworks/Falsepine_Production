using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using UnityEngine.AI;

/// <summary>
///  Script to handle initiation and phases of Boss fights.
/// </summary>
public class BossArenaController : MonoBehaviour
{
    public NodeAI.NodeAI_Agent m_boss; ///< The boss AI agent.
    EnemyHealth m_bossHealth; ///< The boss's health script.
    public Transform ArenaCentre
    {
        get
        {
            return m_arenaCentreTransforms[phase];
        }
    }

    public List<Transform> m_arenaCentreTransforms = new List<Transform>(); ///< The arena centre transforms.
    public List<ParticleSystem> m_arenaParticles = new List<ParticleSystem>(); ///< The arena particles.
    public List<float> m_arenaRadiuses = new List<float>(); ///< The arena radiis.

    public Transform m_wrongBaitSpawn; ///< The spawn point for the wrong bait.
    public float ArenaRadius
    {
        get
        {
            return m_arenaRadiuses[phase];

        }
    }

    public AudioClip m_baitedSound; ///< The sound to play when the boss is baited.

    public AudioClip bossMusic, bossMusicSecondPhase; ///< The music to play when the boss is in the arena.
    public float fadeOutTime; ///< The time it takes to fade out the music.

    public TouchTrigger startTrigger; ///< The trigger to start the boss fight.

    public TouchTrigger room2, room3;
    public UnityEvent onBattleStart; ///< The event to call when the boss starts a battle.

    private UIScript _uiScript; ///< The UI script.

    public InputAction correctBait, incorrectBait; ///< The correct and incorrect bait actions.

    public int phase = 0;
    public float phase1Threshold = 75f;
    public float phase2Threshold = 50f;
    public float phase3Threshold = 25f;


    /// <summary>
    /// We're setting up the input actions, and adding listeners to the triggers
    /// </summary>
    void Start()
    {
        correctBait.performed += ctx => UseCorrectBait();
        incorrectBait.performed += ctx => UseWrongBait();
        correctBait.Enable();
        incorrectBait.Enable();

        startTrigger.Triggered += UseCorrectBait;

        room2.Triggered += () =>
        {
            EnableBossUI();
            m_arenaParticles[1].Play();
            m_boss.SetParameter<bool>("BossStarted", true);
        };

        room3.Triggered += () =>
        {
            EnableBossUI();
            m_arenaParticles[2].Play();
            m_boss.SetParameter<bool>("BossStarted", true);
        };

        _uiScript = FindObjectOfType<UIScript>();

        m_bossHealth = m_boss.GetComponent<EnemyHealth>();


        if (_uiScript != null)
        {
            onBattleStart.AddListener(EnableBossUI);
        }
    }

    private void OnDestroy()
    {
        onBattleStart.RemoveListener(EnableBossUI);
    }


    /// <summary>
    /// If the boss's health is less than the threshold for the next phase, then change the phase and
    /// stop the arena particles
    /// </summary>
    void Update()
    {
        if (m_bossHealth.m_currentHealth < phase1Threshold && phase == 0)
        {
            phase = 1;
            m_boss.SetParameter<bool>("PhaseChange", true);
            foreach (ParticleSystem particle in m_arenaParticles)
            {
                particle.Stop();
            }
            _uiScript.bossUI.SetActive(false);
        }
        if (m_bossHealth.m_currentHealth < phase2Threshold && phase == 1)
        {
            phase = 2;
            m_boss.SetParameter<bool>("PhaseChange", true);
            foreach (ParticleSystem particle in m_arenaParticles)
            {
                particle.Stop();
            }
            _uiScript.bossUI.SetActive(false);
        }
        if (m_bossHealth.m_currentHealth < phase3Threshold && phase == 2)
        {
            phase = 3;
            m_boss.SetParameter<bool>("PhaseChange", true);
        }

    }

    /// <summary>
    ///  Enables UI related to the boss.
    /// </summary>
    public void EnableBossUI()
    {
        if (_uiScript != null)
        {
            _uiScript.bossUI.SetActive(true);
        }
    }

    /// <summary>
    ///  Starts the boss fight using the correct bait variant.
    /// </summary>
    public void UseCorrectBait()
    {

        m_boss.GetComponent<NavMeshAgent>()?.Warp(ArenaCentre.position + Vector3.back);
        m_boss.SetParameter<bool>("CorrectBait", true);
        m_boss.SetParameter<bool>("BossStarted", true);
        m_boss.GetComponent<AudioSource>().PlayOneShot(m_baitedSound);
        GetComponent<AudioSource>().clip = bossMusic;
        GetComponent<AudioSource>().Play();
        onBattleStart.Invoke();
        m_arenaParticles[0].Play();

    }

    /// <summary>
    /// Starts the second phase music
    /// </summary>
    public void StartSecondPhase()
    {
        StartCoroutine(FadeToSecondPhaseCoroutine(fadeOutTime));
    }

    /// <summary>
    ///  Starts the boss fight using the wrong bait variant.
    /// </summary>
    public void UseWrongBait()
    {
        m_boss.GetComponent<NavMeshAgent>()?.Warp(m_wrongBaitSpawn.position);
        //boss.transform.position = wrongBaitSpawn.position;
        m_boss.SetParameter<bool>("CorrectBait", false);
        m_boss.SetParameter<bool>("BossStarted", true);
        GetComponent<AudioSource>().clip = bossMusic;
        GetComponent<AudioSource>().Play();
        Debug.Log("Please recompile");
        onBattleStart.Invoke();
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        for (int i = 0; i < m_arenaCentreTransforms.Count; i++)
        {
            Gizmos.DrawWireSphere(m_arenaCentreTransforms[i].position, m_arenaRadiuses[i]);
        }

    }

    /// <summary>
    ///  Fades the music to the second phase music.
    /// </summary>
    /// <param name="fadeOutTime">The time it takes to fade out the music.</param>
    /// <returns></returns>
    IEnumerator FadeToSecondPhaseCoroutine(float fadeOutTime)
    {
        float startVolume = GetComponent<AudioSource>().volume;
        float currentTime = 0f;
        while (currentTime < fadeOutTime)
        {
            currentTime += Time.deltaTime;
            GetComponent<AudioSource>().volume = Mathf.Lerp(startVolume, 0.1f, currentTime / fadeOutTime);
            yield return null;
        }
        GetComponent<AudioSource>().Stop();
        GetComponent<AudioSource>().clip = bossMusicSecondPhase;
        GetComponent<AudioSource>().Play();
        GetComponent<AudioSource>().volume = startVolume;
        yield break;
    }

}
