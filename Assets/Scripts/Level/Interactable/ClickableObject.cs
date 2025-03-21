using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using System;
using UnityEngine.Events;
using UnityEngine.EventSystems;

/// <summary>
/// A base script for 3D objects that can be clicked on.
/// @todo make this work with the new UI bottomText?
/// </summary>
public class ClickableObject : MonoBehaviour
{
    //collider to check for click
    public Collider[] colliders;

    public GameObject worldPanel;

    //can the object be clicked on?
    public bool m_canBeClicked = true;

    //click event
    public UnityEvent OnClickEvent;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        if (colliders == null || colliders.Length <= 0)
        {
            colliders = GetComponentsInChildren<Collider>();
        }
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (worldPanel != null)
        {
            if (CheckMouseOver())
            {
                worldPanel.gameObject.SetActive(true);
                // increase x scale to 1 over 0.5 seconds
                worldPanel.transform.localScale = Vector3.Lerp(worldPanel.transform.localScale, new Vector3(1, 1, 1), 10.0f * Time.deltaTime);
            }
            else
            {
                // decrease x scale to 0 over 0.5 seconds
                worldPanel.transform.localScale = Vector3.Lerp(worldPanel.transform.localScale, new Vector3(0, 1, 1), 15.0f * Time.deltaTime);
                // then disable the panel
                if (worldPanel.transform.localScale.x < 0.01f)
                {
                    worldPanel.gameObject.SetActive(false);
                }
            }
        }

        if (m_canBeClicked && colliders != null)
        {
            if (
                (Mouse.current.leftButton.wasPressedThisFrame || (CustomInputManager.LastInputWasGamepad && (Gamepad.current.rightTrigger.wasPressedThisFrame || Gamepad.current.buttonSouth.wasPressedThisFrame))) && 
                CheckMouseOver()
            )
            {
                OnClick();
            }
        }
    }

    /// <summary>
    /// Checks if the mouse is pointing at the object.
    /// </summary>
    /// <returns></returns>
    public bool CheckMouseOver()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return false;
        }

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out hit))
        {
            //Debug.Log("Hit: " + hit.transform.name);

            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i] == hit.collider)
                {
                    return true;
                }
            }
        }
        return false;
    }

    /// <summary>
    /// When the object is clicked on.
    /// </summary>
    public virtual void OnClick()
    {
        OnClickEvent.Invoke();
    }
}
