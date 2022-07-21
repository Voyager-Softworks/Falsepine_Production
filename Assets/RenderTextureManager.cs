using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RenderTextureManager : MonoBehaviour  /// @todo Comment
{
    public bool forceEanble = false;
    public GameObject rightFoot;
    public GameObject leftFoot;

    // list of levels to enable the render textures on
    [SerializeField] public List<int> scenes = new List<int>();

    private void Awake() {
        // get current scene
        Scene currentScene = SceneManager.GetActiveScene();
        int sceneIndex = currentScene.buildIndex;
        // check if current scene is in the list of scenes
        EnableRenderTexture(scenes.Contains(sceneIndex) || forceEanble);
    }

    public void EnableRenderTexture(bool enable)
    {
        rightFoot?.SetActive(enable);
        leftFoot?.SetActive(enable);
    }
}
