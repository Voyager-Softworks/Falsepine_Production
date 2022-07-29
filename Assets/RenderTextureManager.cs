using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class RenderTextureManager : MonoBehaviour  /// @todo Comment
{
    public bool forceEanble = false;
    public GameObject rightFoot;
    public GameObject leftFoot;

    private void Awake() {
        DynamicSnow ds = FindObjectOfType<DynamicSnow>();
        if (ds != null) {
            EnableRenderTexture(true);
        }
    }

    public void EnableRenderTexture(bool enable)
    {
        rightFoot?.SetActive(enable);
        leftFoot?.SetActive(enable);
    }
}