using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using System;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

/// <summary>
/// Singleton donotdestroy script that handles the journal system
/// </summary>
public class JournalManager : MonoBehaviour
{
    public static JournalManager instance;

    [Header("Keys")]
    public InputAction openJournalAction;
    public InputAction closeAction;

    [Header("Sounds")]
    public AudioClip openJournalSound;
    public AudioClip closeJournalSound;

    [Header("References")]
    public GameObject journalPanel;

    
    private AudioSource _audioSource;

    private void Awake()
    {
        if (instance == null) {
            instance = this;
            //do not destroy this object
            DontDestroyOnLoad(this);
        } else {
            Destroy(this);
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();

        openJournalAction.performed += ctx => ToggleJournal();
        closeAction.performed += ctx => CloseJournal();
    }

    void OnEnable()
    {
        openJournalAction.Enable();
        closeAction.Enable();
    }

    void OnDisable()
    {
        openJournalAction.Disable();
        closeAction.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        //if pressed 0,1,2, go to that scene
        if (Keyboard.current.numpad0Key.wasPressedThisFrame)
        {
            SceneManager.LoadScene(0);
        }
        if (Keyboard.current.numpad1Key.wasPressedThisFrame)
        {
            SceneManager.LoadScene(1);
        }
        if (Keyboard.current.numpad2Key.wasPressedThisFrame)
        {
            SceneManager.LoadScene(2);
        }

    }

    public void UpdateJournalUI(){
        //update mission cards
        MissionManager missionManager = FindObjectOfType<MissionManager>();

        if (missionManager == null)
        {
            Debug.Log("No MissionManager found in the scene");
            return;
        }
        else{
            missionManager.UpdateAllMissionCards();
        }
    }

    public void ToggleJournal()
    {
        if (journalPanel.activeSelf)
        {
            CloseJournal();
        }
        else
        {
            OpenJournal();
        }
    }

    public void OpenJournal()
    {
        if (journalPanel.activeSelf )return;

        journalPanel.SetActive(true);
        if (_audioSource && openJournalSound) _audioSource.PlayOneShot(openJournalSound);

        UpdateJournalUI();
    }

    public void CloseJournal()
    {
        if (!journalPanel.activeSelf) return;

        journalPanel.SetActive(false);
        if (_audioSource && closeJournalSound) _audioSource.PlayOneShot(closeJournalSound);
    }
}
