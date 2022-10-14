using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DissolveController : MonoBehaviour
{
    public float integrity = 1.0f;
    List<Material> mats = new List<Material>();

    // Start is called before the first frame update
    void Start()
    {
        SkinnedMeshRenderer[] renderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        MeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer>();
        foreach (var renderer in renderers)
        {
            mats.AddRange(renderer.materials);
        }
        foreach (var renderer in meshRenderers)
        {
            mats.AddRange(renderer.materials);
        }
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var mat in mats)
        {
            mat.SetFloat("_Threshhold", integrity);
            mat.SetFloat("_Fade", integrity);
        }
    }
}
