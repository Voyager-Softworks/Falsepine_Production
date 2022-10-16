using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeAI;
using System.Linq;

/// <summary>
///  Class to implement functionality specific to shadow enemies summoned by the <see cref="PrimaevalSpellManager"/>.
/// </summary>
public class ShadowEnemy : MonoBehaviour
{
    Material[] mats; /// < The materials of the shadow enemy.
    public float maxDuration = 8.0f; /// < The maximum duration of the shadow enemy.
    bool timedOut = false; /// < Whether the shadow enemy has timed out.


    // Start is called before the first frame update
    void Start()
    {
        GetComponent<NodeAI_Agent>().SetParameter("Aware", true); // Set the shadow enemy to be aware of the player.
        mats = GetComponentInChildren<SkinnedMeshRenderer>().materials; // Get the materials of the shadow enemy.
        StartCoroutine(DissolveIn()); // Start the dissolve in coroutine.
        GetComponent<EnemyHealth>().Death += (ctx) => StartCoroutine(DissolveOut()); // Start the dissolve out coroutine when the shadow enemy dies.
    }

    // Update is called once per frame
    void Update()
    {
        maxDuration -= Time.deltaTime; // Decrement the maximum duration.
        if (maxDuration <= 0.0f && !timedOut)
        {
            timedOut = true; // Set the timed out flag to true.
            StartCoroutine(DissolveOut()); // Start the dissolve out coroutine.
            GetComponent<NodeAI_Agent>().enabled = false; // Disable the shadow enemy's NodeAI agent.
            GetComponentsInChildren<AudioSource>().ToList().ForEach(a => a.Stop()); // Stop all audio sources.
        }
    }

    /// <summary>
    ///  Coroutine to dissolve the shadow enemy in.
    /// </summary>
    /// <returns></returns>
    IEnumerator DissolveIn()
    {
        float t = 0.0f; // The time variable.
        while (t < 0.6f)
        {
            t += Time.deltaTime * 0.4f; // Increment the time variable.
            foreach (var mat in mats)
            {
                mat.SetFloat("_Threshhold", t); // Set the dissolve threshold of the material.
                mat.SetFloat("_Fade", t / 0.6f); // Set the fade of the material.
            }

            yield return null;
        }
    }

    /// <summary>
    ///  Coroutine to dissolve the shadow enemy out.
    /// </summary>
    /// <returns></returns>
    IEnumerator DissolveOut()
    {
        float t = 0.6f; // The time variable.
        while (t > 0.0f)
        {
            t -= Time.deltaTime * 0.2f; // Decrement the time variable.
            foreach (var mat in mats)
            {
                mat.SetFloat("_Threshhold", t); // Set the dissolve threshold of the material.
                mat.SetFloat("_Fade", t / 0.6f); // Set the fade of the material.
            }
            yield return null;
        }
        foreach (var mat in mats)
        {
            mat.SetFloat("_Threshhold", 0.0f); // Set the dissolve threshold of the material.
            mat.SetFloat("_Fade", 0.0f); // Set the fade of the material.
        }
        Destroy(gameObject); // Destroy the shadow enemy.
    }
}
