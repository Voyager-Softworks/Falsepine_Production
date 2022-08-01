using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// Applies a vignette to the screen.
/// </summary>
public class VignetteScript : MonoBehaviour
{
    public float stayTime = 0.15f;
    private float stayTimer = 0f;
    public Color baseCol = Color.clear;
    public Color fullCol = new Color(1.0f, 0.0f, 0.0f, 0.05f);

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (stayTimer > 0f)
        {
            GetComponent<Image>().color = Color.Lerp(baseCol, fullCol, stayTimer / stayTime);
            stayTimer -= Time.deltaTime;
        }
        else
        {
            stayTimer = 0f;
            GetComponent<Image>().color = baseCol;
        }
    }

    public void StartVignette()
    {
        stayTimer = stayTime;
    }
}
