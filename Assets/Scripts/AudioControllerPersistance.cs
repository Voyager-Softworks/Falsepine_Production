using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class AudioControllerPersistance : MonoBehaviour
{
    private AudioController audioController;
    private MissionManager missionManager;
    private bool playInScene = true;
    private int currentSceneIndex = 99;
    private void Awake()
    {
        audioController = GetComponent<AudioController>();
        missionManager = FindObjectOfType<MissionManager>();
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        int index = missionManager.GetCurrentSceneIndex();
        if (index == -1)
        {
            missionManager = FindObjectOfType<MissionManager>();
            return;
        }
        if (index != currentSceneIndex)
        {
            currentSceneIndex = index;
            if (index == 0) playInScene = true;
            Debug.Log("Current Scene Index: " + index);

        }
        audioController.audioChannels[0].layerIndex = Mathf.Clamp(missionManager.GetCurrentSceneIndex(), 0, 2);
        if (playInScene)
        {
            if (!audioController.audioChannels[0].playing && !audioController.audioChannels[1].playing)
            {
                PlayMusic();
            }
        }
        else
        {
            if (audioController.audioChannels[0].playing || audioController.audioChannels[1].playing)
            {
                StopMusic();
            }
        }
    }

    public void PlayMusic()
    {
        audioController.FadeIn(1, 1.0f);
    }

    public void StopMusic()
    {
        audioController.FadeOut(0, 0.5f);
    }

    public void StopBecauseOfBoss()
    {
        playInScene = false;
    }
}
