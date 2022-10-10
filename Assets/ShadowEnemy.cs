using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeAI;
using System.Linq;

public class ShadowEnemy : MonoBehaviour
{
    Material[] mats;
    public float maxDuration = 8.0f;
    bool timedOut = false;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<NodeAI_Agent>().SetParameter("Aware", true);
        mats = GetComponentInChildren<SkinnedMeshRenderer>().materials;
        StartCoroutine(DissolveIn());
        GetComponent<EnemyHealth>().Death += (ctx) => StartCoroutine(DissolveOut());
    }

    // Update is called once per frame
    void Update()
    {
        maxDuration -= Time.deltaTime;
        if (maxDuration <= 0.0f && !timedOut)
        {
            timedOut = true;
            StartCoroutine(DissolveOut());
            Destroy(gameObject, 8.0f);
            GetComponent<NodeAI_Agent>().enabled = false;
            GetComponentsInChildren<AudioSource>().ToList().ForEach(a => a.Stop());
        }
    }

    IEnumerator DissolveIn()
    {
        float t = 0.0f;
        while (t < 0.6f)
        {
            t += Time.deltaTime * 0.4f;
            foreach (var mat in mats)
            {
                mat.SetFloat("_Threshhold", t);
                mat.SetFloat("_Fade", t / 0.6f);
            }

            yield return null;
        }
    }

    IEnumerator DissolveOut()
    {
        float t = 0.6f;
        while (t > 0.0f)
        {
            t -= Time.deltaTime * 0.2f;
            foreach (var mat in mats)
            {
                mat.SetFloat("_Threshhold", t);
                mat.SetFloat("_Fade", t / 0.6f);
            }
            yield return null;
        }
        foreach (var mat in mats)
        {
            mat.SetFloat("_Threshhold", 0.0f);
            mat.SetFloat("_Fade", 0.0f);
        }
    }
}
