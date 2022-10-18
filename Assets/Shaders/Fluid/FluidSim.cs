using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FluidSim : MonoBehaviour
{
    public ComputeShader fluidSimShader;

    public int resolution = 128;
    public RenderTexture water, waterOld, collision, collisionOld;

    // Start is called before the first frame update
    void Start()
    {
        InitialiseTexture(ref water);
        InitialiseTexture(ref waterOld);
        InitialiseTexture(ref collisionOld);
        collisionOld.enableRandomWrite = true;


        // Set the a value for simulation
        fluidSimShader.SetFloat("a", 0.1f);

        // Set the amplitude for the collision
        fluidSimShader.SetFloat("amplitude", 0.5f);
    }

    void InitialiseTexture(ref RenderTexture tex)
    {
        if (tex != null)
        {
            tex.Release();
        }

        tex = new RenderTexture(resolution, resolution, 0, RenderTextureFormat.ARGBFloat);
        tex.enableRandomWrite = true;
        tex.Create();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        int kernel = fluidSimShader.FindKernel("CSMain");

        fluidSimShader.SetTexture(kernel, "Water", water);
        fluidSimShader.SetTexture(kernel, "Water_Old", waterOld);
        fluidSimShader.SetTexture(kernel, "Collision_Tex", collision);
        fluidSimShader.SetTexture(kernel, "Collision_Tex_Old", collisionOld);

        fluidSimShader.Dispatch(kernel, resolution / 8, resolution / 8, 1);

        // Update the old water and collision textures
        Graphics.Blit(water, waterOld);
        Graphics.Blit(collision, collisionOld);

    }
}
