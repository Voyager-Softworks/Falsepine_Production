using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;
using System;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    public enum InteractEffect {
        NONE,
        DISABLE_INTERACT,
        DISABLE,
        DESTROY
    }

    [Header("Interact")]
    public InputAction interactAction;
    public string interactText = "Interact";

    public Transform _transToCheck = null;
    public float interactDistance = 1f;
    public float fadeDistance = 2f;
    public InteractEffect onInteractEffect = InteractEffect.NONE;

    [Header("UI")]
    public TextMeshProUGUI _text;

    [Header("Events")]
    public UnityEvent OnInteract;

    private void OnEnable() {
        
        interactAction.Enable();
    }

    private void OnDisable() {
        interactAction.Disable();
    }

    // Start is called before the first frame update
    virtual public void Start()
    {

        if (_transToCheck == null && FindObjectOfType<PlayerMovement>()) _transToCheck = FindObjectOfType<PlayerMovement>().transform;

        if (_text == null) _text = GetComponentInChildren<TextMeshProUGUI>();
        if (_text != null) _text.text = /* "[" + interactAction.ToString() + "] " + */ interactText;
    }

    // Update is called once per frame
    virtual public void Update()
    {
        if (_transToCheck == null) return;

        UpdateUI();

        if (Vector3.Distance(transform.position, _transToCheck.position) <= interactDistance)
        {
            if (interactAction.triggered)
            {
                DoInteract();   
            }
        }
    }

    virtual public void DoInteract(){
        switch (onInteractEffect)
        {
            case InteractEffect.NONE:
                break;
            case InteractEffect.DISABLE_INTERACT:
                DisableInteract();
                break;
            case InteractEffect.DISABLE:
                gameObject.SetActive(false);
                break;
            case InteractEffect.DESTROY:
                Destroy(gameObject);
                break;
        }

        OnInteract.Invoke();
    }

    virtual public void DisableInteract()
    {
        if (_text) _text.enabled = false;
        this.enabled = false;
    }

    virtual public void UpdateUI()
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
