using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class BlurController : MonoBehaviour
{
    public Volume volume;

    public float m_blurInSpeed = 0.5f;
    public float m_blurOutSpeed = 2.0f;

    public float m_minBlur = 0.01f;
    public float m_maxBlur = 0.95f;

    public float m_minDistance = 1.0f;
    public float m_maxDistance = 60.0f;


    // Start is called before the first frame update
    void Start()
    {
        volume = GetComponent<Volume>();
    }

    // Update is called once per frame
    private void Update()
    {
        // if any toggleable windows are open, lerp the volume weight to 1
        if (ToggleableTownWindow.AnyWindowOpen()) {
            // weight
            volume.weight = Mathf.Clamp(volume.weight + Time.deltaTime * m_blurInSpeed, m_minBlur, m_maxBlur);
        }
        // if no toggleable windows are open, lerp the volume weight to 0
        else {
            // weight
            volume.weight = Mathf.Clamp(volume.weight - Time.deltaTime * m_blurOutSpeed, m_minBlur, m_maxBlur);
        }
    }
}
