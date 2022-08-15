using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;
using System;
using UnityEngine.Events;

public class Interactable : MonoBehaviour  /// @todo Comment
{
    static bool m_interactedThisFrame = false;

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
    public InteractEffect onInteractEffect = InteractEffect.NONE;

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
    }

    // Update is called once per frame
    virtual public void Update()
    {
        if (_transToCheck == null) return;

        UpdateUI();
    }

    public bool CheckActionPressed()
    {
        if (interactAction.triggered)
        {
            if (Vector3.Distance(transform.position, _transToCheck.position) <= interactDistance)
            {
                DoInteract();
                return true;
            }
        }
        return false;
    }

    private void LateUpdate() {
        m_interactedThisFrame = false;
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
    }

    virtual public void DisableInteract()
    {
        this.enabled = false;

        // get UI script
        InteractManager bt = FindObjectOfType<InteractManager>();
        if (bt == null) return;

        // remove request
        bt.RemoveRequest(new InteractManager.TextRequest(interactText, this, interactDistance));
    }

    virtual public void UpdateUI()
    {
        if (Vector3.Distance(transform.position, _transToCheck.position) <= interactDistance)
        {
            // get UI script
            InteractManager bt = FindObjectOfType<InteractManager>();
            if (bt == null) return;

            // send request
            bt.RequestBottomText(new InteractManager.TextRequest(interactText, this, interactDistance));
        }
    }
}
