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

/// <summary>
/// Base class for interactable objects, such as clues, pikcups, objectives, etc.
/// </summary>
public class Interactable : MonoBehaviour
{
    static bool m_interactedThisFrame = false;

    /// <summary>
    /// How an interactable object should react after being interacted with.
    /// </summary>
    public enum InteractEffect {
        NONE,
        DISABLE_INTERACT,
        DISABLE,
        DESTROY,
        REPLACE
    }

    [Header("Interact")]
    //public InputAction interactAction;
    // DISABLED due to using E / South Button for all interactables
    public string interactText = "Interact";
    public string interactTextWithSprite{
        get
        {
            // remove any text between and including [], <>, or {}
            string text = RemoveButtonsFromText("[", "]", interactText);
            text = RemoveButtonsFromText("<", ">", text);
            text = RemoveButtonsFromText("{", "}", text);

            string newText = "";
            if (!CustomInputManager.LastInputWasGamepad){
                newText = "<sprite name=\"Btn_E\"> " + text;
            }
            else{
                newText = "<sprite name=\"Btn_South\"> " + text;
            }

            return newText;
        }
    }

    private static string RemoveButtonsFromText(string startOffender, string endOffender, string text)
    {
        int startIndex = text.IndexOf(startOffender);
        if (startIndex != -1)
        {
            int endIndex = text.IndexOf(endOffender, startIndex);
            if (endIndex != -1)
            {
                text = text.Remove(startIndex, endIndex - startIndex + 1);
            }
        }

        return text;
    }

    public Transform _transToCheck = null;
    public float interactDistance = 3.0f;
    public InteractEffect onInteractEffect = InteractEffect.NONE;
    public GameObject replacementObject = null;

    // interact c# delegate
    public System.Action onInteract;

    private InteractManager m_interactManager = null;
    private InteractManager interactManager {
        get {
            if (m_interactManager == null) {
                m_interactManager = FindObjectOfType<InteractManager>();
            }
            return m_interactManager;
        }
    }

    private void OnEnable() {
        //interactAction.Enable();
    }

    private void OnDisable() {
        //interactAction.Disable();
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

    /// <summary>
    /// Checks if the interaction button was pressed this frame, and does the interaction if it was.
    /// </summary>
    /// <returns></returns>
    public bool CheckActionPressed()
    {
        // E or South button is pressed
        if (Keyboard.current.eKey.wasPressedThisFrame || (CustomInputManager.LastInputWasGamepad && Gamepad.current.buttonSouth.wasPressedThisFrame))
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

    /// <summary>
    /// Virtual function to be overridden by child classes. Used to perform the action of the interactable.
    /// </summary>
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
            case InteractEffect.REPLACE:
                if (replacementObject != null)
                {
                    Instantiate(replacementObject, transform.position, transform.rotation);
                    // scale match
                    replacementObject.transform.localScale = transform.localScale;
                    Destroy(gameObject);
                }
                break;
        }

        onInteract?.Invoke();
    }

    /// <summary>
    /// Stops this interactable from being interacted with.
    /// </summary>
    virtual public void DisableInteract()
    {
        this.enabled = false;

        // get UI script
        if (interactManager == null) return;

        // remove request
        interactManager.RemoveRequest(new InteractManager.TextRequest(interactTextWithSprite, this, transform, interactDistance));
    }

    /// <summary>
    /// Updates the UI to show the interact text.
    /// </summary>
    virtual public void UpdateUI()
    {
        if (Vector3.Distance(transform.position, _transToCheck.position) <= interactDistance)
        {
            // get UI script
            if (interactManager == null) return;

            // send request
            interactManager.RequestBottomText(new InteractManager.TextRequest(interactTextWithSprite, this, transform, interactDistance));
        }
    }
}
