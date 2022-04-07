using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class BossArenaController : MonoBehaviour
{
    public Transform arenaCentre;
    public float arenaRadius;

    public AudioClip baitedSound;

    public AudioClip bossMusic, bossMusicSecondPhase;
    public float fadeOutTime;


    public UnityEvent onBattleStart;

    private UIScript _uiScript;

    public InputAction correctBait, incorrectBait;
    // Start is called before the first frame update
    void Start()
    {
        correctBait.performed += ctx => UseCorrectBait();
        incorrectBait.performed += ctx => UseWrongBait();
        correctBait.Enable();
        incorrectBait.Enable();

        _uiScript = FindObjectOfType<UIScript>();

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
        
    }

    public void EnableBossUI(){
        if (_uiScript != null){
            _uiScript.bossUI.SetActive(true);
        }
    }

    public void UseCorrectBait()
    {
        GameObject boss = FindObjectOfType<NodeAI.NodeAI_Agent>().gameObject;
        boss.transform.position = arenaCentre.position + Vector3.back;
        FindObjectOfType<NodeAI.NodeAI_Agent>().SetBool("CorrectBait", true);
        FindObjectOfType<NodeAI.NodeAI_Agent>().SetBool("BossStarted", true);
        FindObjectOfType<NodeAI.NodeAI_Agent>().agent.isStopped = true;
        boss.GetComponent<AudioSource>().PlayOneShot(baitedSound);
        GetComponent<AudioSource>().clip = bossMusic;
        GetComponent<AudioSource>().Play();
        onBattleStart.Invoke();

    }

    public void StartSecondPhase()
    {
        StartCoroutine(FadeToSecondPhaseCoroutine(fadeOutTime));
    }

    public void UseWrongBait()
    {
        GameObject boss = FindObjectOfType<NodeAI.NodeAI_Agent>().gameObject;
        FindObjectOfType<NodeAI.NodeAI_Agent>().SetBool("CorrectBait", false);
        FindObjectOfType<NodeAI.NodeAI_Agent>().SetBool("BossStarted", true);
        GetComponent<AudioSource>().clip = bossMusic;
        GetComponent<AudioSource>().Play();
        onBattleStart.Invoke();
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(arenaCentre.position, arenaRadius);
    }

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
