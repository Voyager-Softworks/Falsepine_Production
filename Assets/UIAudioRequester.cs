using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class UIAudioRequester : MonoBehaviour
{
    // store a specific field in the UIAudioManager
    // [SerializeField] public UIAudioManager.SoundType sound;
    // private Button button;

    // private void Awake() {
    //     button = GetComponent<Button>();
    // }

    // private void OnEnable() {
    //     if (button != null)
    //     {
    //         button.onClick.AddListener(PlaySound);
    //     }
    // }

    // private void OnDisable() {
    //     if (button != null)
    //     {
    //         button.onClick.RemoveListener(PlaySound);
    //     }
    // }

    // private void PlaySound() {
    //     UIAudioManager.PlaySound(sound);
    // }
}
