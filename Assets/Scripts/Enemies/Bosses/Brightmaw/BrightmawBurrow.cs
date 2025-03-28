using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeAI;

/// <summary>
///  Script that enables and disables particles and audio when the boss/enemy burrows and unburrows.
/// </summary>
public class BrightmawBurrow : MonoBehaviour
{
    public NodeAI_Agent agent;
    public AudioSource audioSource;
    public AudioClip burrowSound;
    public ParticleSystem burrowParticles, unburrowParticles;

    public bool isBoss = false;


    bool burrowing = false;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Health_Base>().Death += (ctx) =>
        {
            audioSource.Stop();
            burrowParticles.Stop();
            unburrowParticles.Stop();
            agent.enabled = false;
            foreach (var child in GetComponentsInChildren<Collider>())
            {
                child.enabled = false;
            }
        };
    }

    // Update is called once per frame
    void Update()
    {

        bool newBurrowVal = agent.GetParameter<bool>("Burrowing");
        bool playFX = isBoss && agent.GetParameter<bool>("Stage3");
        if (!isBoss) playFX = true;

        if (newBurrowVal != burrowing)
        {
            if (newBurrowVal)
            {
                if (playFX)
                {
                    audioSource.loop = true;
                    audioSource.clip = burrowSound;
                    audioSource.pitch = Random.Range(0.8f, 1.2f);
                    audioSource.time = Random.Range(0, audioSource.clip.length);
                    audioSource.Play();
                    burrowParticles.Play();
                }
                foreach (var child in GetComponentsInChildren<Collider>())
                {
                    child.enabled = false;
                }
            }
            else
            {
                burrowParticles.Stop();
                audioSource.loop = false;
                audioSource.Stop();
                unburrowParticles.Play();
                foreach (var child in GetComponentsInChildren<Collider>())
                {
                    child.enabled = true;
                }
            }
            burrowing = newBurrowVal;
        }
    }
}
