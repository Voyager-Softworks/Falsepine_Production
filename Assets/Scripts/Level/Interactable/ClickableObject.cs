using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using System;
using UnityEngine.Events;

/// <summary>
/// A base script for 3D objects that can be clicked on.
/// </summary>
public class ClickableObject : MonoBehaviour
{
    //collider to check for click
    public Collider collider;

    //can the object be clicked on?
    public bool m_canBeClicked = true;

    //click event
    public UnityEvent OnClickEvent;

    // Start is called before the first frame update
    void Start()
    {
        if (collider == null)
        {
            collider = GetComponentInChildren<Collider>();
        }
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (m_canBeClicked && collider != null)
        {
            if (Mouse.current.leftButton.isPressed)
            {
                //raycast to see if we clicked on this object
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider == collider)
                    {
                        //if we clicked on this object, call the click function
                        OnClick();
                    }
                }
            }
        }
    }

    public virtual void OnClick()
    {
        OnClickEvent.Invoke();
    }
}
