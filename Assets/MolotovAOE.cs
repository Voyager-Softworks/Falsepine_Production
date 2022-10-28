using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

/// <summary>
///  Class to disable VFX for the motolov explosion after a certain amount of time
/// </summary>
public class MolotovAOE : MonoBehaviour
{
    public List<VisualEffect> effects = new List<VisualEffect>();
    public float duration = 5f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        duration -= Time.deltaTime;
        if (duration <= 0)
        {
            foreach (VisualEffect effect in effects)
            {
                effect.Stop();
            }
        }
    }
}
