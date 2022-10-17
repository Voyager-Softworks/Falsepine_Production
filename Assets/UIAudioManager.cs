using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Singleton do not destroy class that manages the audio for the UI
/// </summary>
public class UIAudioManager : MonoBehaviour
{
    public static UIAudioManager instance;

    [System.Serializable]
    public class UISound
    {
        [ReadOnly] public string name;
        public List<AudioClip> clips = new List<AudioClip>();
        public float minVolume = 1.0f;
        public float maxVolume = 1.0f;
        public float minPitch = 1.0f;
        public float maxPitch = 1.0f;

        public void Play(){
            if (UIAudioManager.instance == null) return;
            if (UIAudioManager.instance.m_autoSoundPrefab == null) return;
            if (clips.Count == 0) return;

            GameObject soundObject = Instantiate(UIAudioManager.instance.m_autoSoundPrefab, UIAudioManager.instance.transform);
            AutoSound autoSound = soundObject.GetComponent<AutoSound>();
            autoSound.clips = new List<AudioClip>(clips).ToArray();
            autoSound.playOnStart = true;
            autoSound.destroyOnEnd = true;
            autoSound.audioSource.volume = Random.Range(minVolume, maxVolume);
            autoSound.audioSource.pitch = Random.Range(minPitch, maxPitch);
        }
    }

    public UISound buttonClick;
    public UISound purchase;
    public UISound error;
    public UISound upgrade;

    public enum SoundType
    {
        ButtonClick,
        Purchase,
        Error
    }

    public GameObject m_autoSoundPrefab;

    private void Awake() {
        if (instance == null)
        {
            instance = this;
            //do not destroy this object
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
            Destroy(gameObject);
        }
    }

    public static void PlaySound(SoundType soundType){
        if (instance == null) return;
        switch (soundType)
        {
            case SoundType.ButtonClick:
                instance.buttonClick.Play();
                break;
            case SoundType.Purchase:
                instance.purchase.Play();
                break;
            case SoundType.Error:
                instance.error.Play();
                break;
            default:
                break;
        }
    }

    public static void PlaySound(UISound sound){
        if (instance == null) return;
        sound.Play();
    }

    private void OnValidate() {
        // set name of sound to the name of the field
        FieldInfo[] fields = this.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
        foreach (FieldInfo field in fields)
        {
            if (field.FieldType == typeof(UISound))
            {
                UISound sound = (UISound)field.GetValue(this);
                sound.name = field.Name;
            }
        }
    }
}
