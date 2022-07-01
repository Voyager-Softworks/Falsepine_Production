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
/// </summary>
public class ClickableObject : MonoBehaviour
{
    //collider to check for click
    public Collider[] colliders;

    public TextMeshProUGUI worldText;

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
        if (worldText != null)
        {
            if (CheckMouseOver()){
                worldText.gameObject.SetActive(true);
            }
            else{
                worldText.gameObject.SetActive(false);
            }
        }

        if (m_canBeClicked && colliders != null)
        {
            if (Mouse.current.leftButton.isPressed && CheckMouseOver())
            {
                OnClick();
            }
        }
    }

    public bool CheckMouseOver(){
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

    public virtual void OnClick()
    {
        OnClickEvent.Invoke();
    }
}
