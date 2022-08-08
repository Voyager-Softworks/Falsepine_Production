using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// @deprecated not used anymore. <br/>
/// Class to manage the outline shader on objects.
/// </summary>
public class Outline : MonoBehaviour  /// @todo Comment
{
    public Material outlineMaterial;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnValidate() {
        //try add outline material
        if (outlineMaterial == null) {
        }
        else{
            TryAddOutline();
        }
    }

    void TryAddOutline()
    {
        MeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer meshRenderer in meshRenderers)
        {
            Material[] materials = meshRenderer.sharedMaterials;

            bool hasOutline = false;
            foreach (Material material in materials)
            {
                if (material == outlineMaterial)
                {
                    hasOutline = true;
                    break;
                }
            }

            if (!hasOutline)
            {
                //add material to end of list
                Material[] newMaterials = new Material[materials.Length + 1];
                for (int i = 0; i < materials.Length; i++)
                {
                    newMaterials[i] = materials[i];
                }
                newMaterials[newMaterials.Length - 1] = outlineMaterial;
                meshRenderer.sharedMaterials = newMaterials;
            }
        }
    }
}
