using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;
using System;
using UnityEngine.Events;

public class ClueScript : MonoBehaviour
{
    [Header("Clue")]
    public NotesManager.ClueType clueType;

    [Header("Interact")]
    public InputAction inspectAction;

    public Transform _transToCheck = null;
    public float interactDistance = 1f;
    public float fadeDistance = 2f;

    public bool hideOnInteract = true;

    [HideInInspector] public NotesManager _notesManager = null;

    [Header("UI")]
    public TextMeshProUGUI _text;

    //[Header("Events")]
    //public UnityEvent OnInspect;

    // Start is called before the first frame update
    void Start()
    {
        inspectAction.Enable();

        if (_transToCheck == null && FindObjectOfType<PlayerMovement>()) _transToCheck = FindObjectOfType<PlayerMovement>().transform;

        if (_text == null) _text = GetComponentInChildren<TextMeshProUGUI>();

        if (_notesManager == null) _notesManager = FindObjectOfType<NotesManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_transToCheck == null) return;

        UpdateUI();

        if (Vector3.Distance(transform.position, _transToCheck.position) <= interactDistance)
        {
            if (inspectAction.triggered)
            {
                //OnInspect.Invoke();
                if (_notesManager != null) _notesManager.ClueInspected(this);

                if (hideOnInteract){
                    HideClue();
                }
            }
        }
    }

    private void HideClue()
    {
        if (_text) _text.enabled = false;
        this.enabled = false;
    }

    private void UpdateUI()
    {
        if (_text)
        {
            //make text look at camera
            _text.transform.LookAt(Camera.main.transform);

            //set opacity
            _text.color = new Color(1f, 1f, 1f, 0f);
            if (Vector3.Distance(transform.position, _transToCheck.position) <= interactDistance) {
                //make text full opacity and gold
                _text.color = new Color(1f, 0.85f, 0f, 1f);
            }
            else if (Vector3.Distance(transform.position, _transToCheck.position) <= fadeDistance) {
                _text.color = new Color(1f, 1f, 1f, (1f - (Vector3.Distance(transform.position, _transToCheck.position) - interactDistance) / (fadeDistance - interactDistance)) * 0.1f);
            }
        }
    }
}
