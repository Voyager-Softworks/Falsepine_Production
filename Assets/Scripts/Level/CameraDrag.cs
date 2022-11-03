using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Allows the camera to be dragged in the town scene.
/// </summary>
public class CameraDrag : MonoBehaviour
{
    [Header("Input")]
    // horiz and vert input axes for the camera to be moved using keys
    public InputAction m_horizInput;
    public InputAction m_vertInput;
    public InputAction m_boostInput;

    public float m_keySpeed = 2.0f;
    public float m_boostMulti = 2.0f;
    public Vector3 m_velocity = Vector3.zero;


    public float dragSpeed = 2;
    private Vector3 dragOrigin;

    public Vector3 center = new Vector3(0, 0, 0);
    public float maxDistance = 10;

    private Vector3 m_targetPosition;

    private bool m_returnToCent = false;

    private TutorialPopup m_tutorialPopup;

    private void Start() {
        center = transform.position;
        m_targetPosition = transform.position;

        m_tutorialPopup = FindObjectOfType<TutorialPopup>();
    }

    private void OnEnable() {
        m_horizInput.Enable();
        m_vertInput.Enable();
        m_boostInput.Enable();
    }

    private void OnDisable() {
        m_horizInput.Disable();
        m_vertInput.Disable();
        m_boostInput.Disable();
    }
 
 
    void Update()
    {
        // if tutorial popup is open, return
        if (m_tutorialPopup && m_tutorialPopup.gameObject.activeSelf) {
            return;
        }
        // if toggleable windows are open, return
        if (ToggleableTownWindow.AnyWindowOpen()){
            return;
        }

        if ((transform.position - center).magnitude > maxDistance + 10) {
            m_returnToCent = true;
        }

        if (m_returnToCent)
        {
            if ((transform.position - center).magnitude <= maxDistance){
                m_returnToCent = false;
            }
            // lerp to the max distance
            transform.position = Vector3.Slerp(transform.position, center, Time.deltaTime * 0.5f);
            m_targetPosition = transform.position;
            dragOrigin = Vector3.zero;
            m_velocity = Vector3.zero;
        }
        else{
            CheckKeys();

            CheckDrag();

            //lerp to update position
            transform.position = m_targetPosition;
        }

        
    }

    /// <summary>
    /// Tries to move the camera using mouse drag.
    /// </summary>
    private void CheckDrag()
    {
        Vector3 currentMousePos = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(currentMousePos);

        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            // raycast into the world
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                // set the drag origin to the hit point
                dragOrigin = hit.point;
            }
            return;
        }

        if (!Mouse.current.rightButton.isPressed || dragOrigin == Vector3.zero) return;

        // make a plane at dragOrigin, with normal pointing up
        Plane plane = new Plane(Vector3.up, dragOrigin);

        // get the mouse position on the plane
        float distance;
        if (plane.Raycast(ray, out distance))
        {
            Vector3 point = ray.GetPoint(distance);

            // get the difference between the mouse position and the drag origin
            Vector3 difference = dragOrigin - point;
            difference.y = 0;
            // set the target position to the difference
            m_targetPosition = Vector3.Lerp(m_targetPosition, transform.position + difference, Time.deltaTime * 20.0f);
        }
    }

    /// <summary>
    /// Tries to move the cam with keys
    /// </summary>
    private void CheckKeys()
    {
        Vector3 xMove = Vector3.zero;
        Vector3 zMove = Vector3.zero;
        float multi = 1;

        if (m_horizInput.ReadValue<float>() != 0)
        {
            xMove = transform.right * m_horizInput.ReadValue<float>() * m_keySpeed * Time.deltaTime;
            xMove.y = 0;
        }
        if (m_vertInput.ReadValue<float>() != 0)
        {
            zMove = transform.forward * m_vertInput.ReadValue<float>() * m_keySpeed * Time.deltaTime;
            zMove.y = 0;
        }
        if (m_boostInput.ReadValue<float>() != 0)
        {
            multi = m_boostMulti;
        }

        Vector3 move = (xMove + zMove).normalized * Mathf.Max(xMove.magnitude, zMove.magnitude) * multi;
        m_velocity = Vector3.Lerp(m_velocity, move, Time.deltaTime * 10.0f);
        
        m_targetPosition = transform.position + m_velocity;
    }

}
