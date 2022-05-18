using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using System;
using UnityEngine.Events;
public class JournalManager : MonoBehaviour
{
    [Header("Keys")]
    public InputAction openJournalAction;
    public InputAction closeAction;

    [Header("Sounds")]
    public AudioClip openJournalSound;
    public AudioClip closeJournalSound;

    [Header("References")]
    public GameObject journalPanel;

    
    private AudioSource _audioSource;

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
    }

    public void CloseJournal()
    {
        if (!journalPanel.activeSelf) return;
        
        journalPanel.SetActive(false);
        if (_audioSource && closeJournalSound) _audioSource.PlayOneShot(closeJournalSound);
    }
}
