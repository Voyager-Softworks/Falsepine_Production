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
    new public Collider collider;

    public TextMeshProUGUI worldText;

    //can the object be clicked on?
    public bool m_canBeClicked = true;

    //click event
    public UnityEvent OnClickEvent;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        if (collider == null)
        {
            collider = GetComponentInChildren<Collider>();
        }
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (worldText != null)
        {
            if (CheckMouseOver()){
                worldText.gameObject.SetActive(true);
            }
            else{
                worldText.gameObject.SetActive(false);
            }
        }

        if (m_canBeClicked && collider != null)
        {
            if (Mouse.current.leftButton.isPressed && CheckMouseOver())
            {
                OnClick();
            }
        }
    }

    public bool CheckMouseOver(){
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider == collider)
            {
                return true;
            }
        }
        return false;
    }

    public virtual void OnClick()
    {
        OnClickEvent.Invoke();
    }
}
