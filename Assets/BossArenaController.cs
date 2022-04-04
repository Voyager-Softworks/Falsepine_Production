using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BossArenaController : MonoBehaviour
{
    public Transform arenaCentre;
    public float arenaRadius;

    public AudioClip baitedSound;

    public AudioClip bossMusic;

    public InputAction correctBait, incorrectBait;
    // Start is called before the first frame update
    void Start()
    {
        correctBait.performed += ctx => UseCorrectBait();
        incorrectBait.performed += ctx => UseWrongBait();
        correctBait.Enable();
        incorrectBait.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        
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

    }

    public void UseWrongBait()
    {
        GameObject boss = FindObjectOfType<NodeAI.NodeAI_Agent>().gameObject;
        FindObjectOfType<NodeAI.NodeAI_Agent>().SetBool("CorrectBait", false);
        FindObjectOfType<NodeAI.NodeAI_Agent>().SetBool("BossStarted", true);
        GetComponent<AudioSource>().clip = bossMusic;
        GetComponent<AudioSource>().Play();
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(arenaCentre.position, arenaRadius);
    }
}
