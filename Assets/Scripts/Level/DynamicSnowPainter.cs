using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

/// <summary>
/// Automatically enables/disables this dynamic snow painer if there is/isnt snow in the scene.
/// </summary>
public class DynamicSnowPainter : MonoBehaviour
{
    // Start is called before the first frame update
    private void Awake()
    {
        DynamicSnow ds = FindObjectOfType<DynamicSnow>();
        if (ds != null)
        {
            EnableVFX(true);
        }
        else
        {
            EnableVFX(false);
        }
    }

    public void EnableVFX(bool enable)
    {
        VisualEffect vfx = GetComponent<VisualEffect>();
        if (vfx == null) return;
        vfx.enabled = enable;
    }
}
