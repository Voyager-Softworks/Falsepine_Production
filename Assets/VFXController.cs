using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeAI;

[NodeAI.Parameterisable]
public class VFXController : MonoBehaviour
{
    [System.Serializable]
    public struct VFX
    {
        public ParticleSystem[] particleSystems;
        public string name;
    }

    public List<VFX> vfxs = new List<VFX>();


    public void PlayVFX(string name)
    {
        Debug.Log("Playing VFX: " + name);
        foreach (var vfx in vfxs)
        {
            if (vfx.name == name)
            {
                Debug.Log("Found VFX: " + name);
                foreach (var particleSystem in vfx.particleSystems)
                {
                    particleSystem.Play();
                }
            }
        }
    }
}
