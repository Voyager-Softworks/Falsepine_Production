using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;
using System;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class NotesManager : MonoBehaviour
{
    //list of clues to find
    public enum ClueType {
        WallClawMarks5,
        FloorHollowBones,
        FloorDeerCarcass
    }


    //dictionary of clues and their long descriptions
    public Dictionary<ClueType, string> clueLongDesc = new Dictionary<ClueType, string>() {
        { ClueType.WallClawMarks5, "Claw Marks" },
        { ClueType.FloorHollowBones, "Hollowed Bones" },
        { ClueType.FloorDeerCarcass, "Deer Carcass" }
    };

    //dictionary of clues and their short descriptions
    public Dictionary<ClueType, string> clueShortDesc = new Dictionary<ClueType, string>() {
        { ClueType.WallClawMarks5, "Claw marks" },
        { ClueType.FloorHollowBones, "Hollowed bones" },
        { ClueType.FloorDeerCarcass, "Deer carcass" }
    };


    private UIScript _uiScript;
    private AudioSource _audioSource;

    public AudioClip clueFoundSound;

    public List<ClueType> cluesFound = new List<ClueType>();

    private MessageManager _messageManager;

    // Start is called before the first frame update
    void Start()
    {
        if (_messageManager == null) _messageManager = FindObjectOfType<MessageManager>();

        if (_uiScript == null) _uiScript = FindObjectOfType<UIScript>();

        if (_audioSource == null) _audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateNotesPaper();
    }

    public void UpdateNotesPaper(){
        if (_uiScript == null || _uiScript.journalUIList == null || _uiScript.journalUIList.notesText == null) return;

        string text = "";
        foreach (ClueType clue in cluesFound){
            text += clueShortDesc[clue] + "\n\n";
        }
        _uiScript.journalUIList.notesText.text = text;
    }

    public void ClueInspected(ClueScript _clue)
    {
        if (cluesFound.Contains(_clue.clueType)) return;

        cluesFound.Add(_clue.clueType);

        if (_messageManager != null) _messageManager.AddMessage("Clue Found!\n" + clueLongDesc[_clue.clueType]);

        if (_audioSource != null && clueFoundSound != null) _audioSource.PlayOneShot(clueFoundSound);
    }
}