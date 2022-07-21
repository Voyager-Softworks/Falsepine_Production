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
    public NodeAI.NodeAI_Agent boss; ///< The boss AI agent.
    HealthScript bossHealth; ///< The boss's health script.
    public Transform arenaCentre { 
        get {
            return arenaCentreTransforms[phase];
        }
    }

    public List<Transform> arenaCentreTransforms = new List<Transform>(); ///< The arena centre transforms.
    public List<float> arenaRadiuses = new List<float>(); ///< The arena radiis.

    public Transform wrongBaitSpawn; ///< The spawn point for the wrong bait.
    public float arenaRadius{ 
        get {
            return arenaRadiuses[phase];
            
        }
    }

    public AudioClip baitedSound; ///< The sound to play when the boss is baited.

    public AudioClip bossMusic, bossMusicSecondPhase; ///< The music to play when the boss is in the arena.
    public float fadeOutTime; ///< The time it takes to fade out the music.


    public UnityEvent onBattleStart; ///< The event to call when the boss starts a battle.

    private UIScript _uiScript; ///< The UI script.

    public InputAction correctBait, incorrectBait; ///< The correct and incorrect bait actions.

    public int phase = 0;
    public float phase1Threshold = 75f;
    public float phase2Threshold = 50f;
    public float phase3Threshold = 25f;
    // Start is called before the first frame update
    void Start()
    {
        correctBait.performed += ctx => UseCorrectBait();
        incorrectBait.performed += ctx => UseWrongBait();
        correctBait.Enable();
        incorrectBait.Enable();

        _uiScript = FindObjectOfType<UIScript>();

        bossHealth = boss.GetComponent<HealthScript>();
        

        if (_uiScript != null){
            onBattleStart.AddListener(EnableBossUI);
        }
    }

    private void OnDestroy() {
        onBattleStart.RemoveListener(EnableBossUI);
    }

    // Update is called once per frame
    void Update()
    {
        if(bossHealth.currentHealth < phase1Threshold && phase == 0)
        {
            phase = 1;
            boss.SetParameter<bool>("PhaseChange", true);
        }
        if(bossHealth.currentHealth < phase2Threshold && phase == 1)
        {
            phase = 2;
            boss.SetParameter<bool>("PhaseChange", true);
        }
        if(bossHealth.currentHealth < phase3Threshold && phase == 2)
        {
            phase = 3;
            boss.SetParameter<bool>("PhaseChange", true);
        }

    }

    /// <summary>
    ///  Enables UI related to the boss.
    /// </summary>
    public void EnableBossUI(){
        if (_uiScript != null){
            _uiScript.bossUI.SetActive(true);
        }
    }

    /// <summary>
    ///  Starts the boss fight using the correct bait variant.
    /// </summary>
    public void UseCorrectBait()
    {
        
        boss.GetComponent<NavMeshAgent>()?.Warp(arenaCentre.position + Vector3.back);
        boss.SetParameter<bool>("CorrectBait", true);
        boss.SetParameter<bool>("BossStarted", true);
        boss.GetComponent<AudioSource>().PlayOneShot(baitedSound);
        GetComponent<AudioSource>().clip = bossMusic;
        GetComponent<AudioSource>().Play();
        onBattleStart.Invoke();

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
        boss.GetComponent<NavMeshAgent>()?.Warp(wrongBaitSpawn.position);
        //boss.transform.position = wrongBaitSpawn.position;
        boss.SetParameter<bool>("CorrectBait", false);
        boss.SetParameter<bool>("BossStarted", true);
        GetComponent<AudioSource>().clip = bossMusic;
        GetComponent<AudioSource>().Play();
        Debug.Log("Please recompile");
        onBattleStart.Invoke();
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        for(int i = 0; i < arenaCentreTransforms.Count; i++)
        {
            Gizmos.DrawWireSphere(arenaCentreTransforms[i].position, arenaRadiuses[i]);
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
