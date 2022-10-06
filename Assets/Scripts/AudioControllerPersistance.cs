using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioControllerPersistance : MonoBehaviour
{
    private AudioController audioController;
    private void Awake()
    {
        audioController = GetComponent<AudioController>();
        DontDestroyOnLoad(gameObject);
    }

    int GetMissionIndex()
    {

    }
}
