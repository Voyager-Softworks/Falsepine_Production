using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;
using System;
using UnityEngine.Events;

public class ClueScript : Interactable
{
    [Header("Clue")]
    public NotesManager.ClueType clueType;

    [HideInInspector] public NotesManager _notesManager = null;

    // Start is called before the first frame update
    override public void Start()
    {
        base.Start();

        if (_notesManager == null) _notesManager = FindObjectOfType<NotesManager>();
    }

    // Update is called once per frame
    override public void Update()
    {
        base.Update();
    }

    override public void DoInteract(){
        base.DoInteract();

        if (_notesManager != null)
        {
            _notesManager.ClueInspected(this);
        }
    }
}
