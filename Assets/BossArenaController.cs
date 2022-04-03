using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossArenaController : MonoBehaviour
{
    public Transform arenaCentre;
    public float arenaRadius;

    public AudioClip baitedSound;

    public AudioClip bossMusic;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UseCorrectBait()
    {
        GameObject boss = FindObjectOfType<NodeAI.NodeAI_Agent>().gameObject;
        boss.transform.position = arenaCentre.position;
        boss.SetActive(true);
        FindObjectOfType<NodeAI.NodeAI_Agent>().SetBool("UseCorrectBait", true);
        FindObjectOfType<NodeAI.NodeAI_Agent>().SetBool("BossStarted", true);
        boss.GetComponent<AudioSource>().PlayOneShot(baitedSound);
        GetComponent<AudioSource>().clip = bossMusic;
        GetComponent<AudioSource>().Play();

    }

    public void UseWrongBait()
    {
        GameObject boss = FindObjectOfType<NodeAI.NodeAI_Agent>().gameObject;
        boss.SetActive(true);
        FindObjectOfType<NodeAI.NodeAI_Agent>().SetBool("UseCorrectBait", false);
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
