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

    public float m_keySpeed = 2.0f;


    public float dragSpeed = 2;
    private Vector2 dragOrigin;

    public Vector3 center = new Vector3(0, 0, 0);
    public float maxDistance = 10;

    private Vector3 m_targetPosition;

    private void Start() {
        center = transform.position;
        m_targetPosition = transform.position;
    }

    private void OnEnable() {
        m_horizInput.Enable();
        m_vertInput.Enable();
    }

    private void OnDisable() {
        m_horizInput.Disable();
        m_vertInput.Disable();
    }
 
 
    void Update()
    {
        if ((transform.position - center).magnitude > maxDistance)
        {
            // lerp to the max distance
            transform.position = Vector3.Lerp(transform.position, center, Time.deltaTime);
        }

        CheckKeys();

        CheckDrag();

        //lerp to update position
        transform.position = Vector3.Lerp(transform.position, m_targetPosition, Time.deltaTime * 50.0f);
    }

    /// <summary>
    /// Tries to move the camera using mouse drag.
    /// </summary>
    private void CheckDrag()
    {
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            dragOrigin = Mouse.current.position.ReadValue();
            return;
        }

        if (!Mouse.current.rightButton.isPressed) return;

        Vector3 pos = Camera.main.ScreenToViewportPoint(Mouse.current.delta.ReadValue());
        Vector3 move = new Vector3(pos.x * dragSpeed, 0, pos.y * dragSpeed);
        //rotate move arounjd the y axis
        move = Quaternion.Euler(0, -45, 0) * -move;

        m_targetPosition = transform.position + move;
    }

    /// <summary>
    /// Tries to move the cam with keys
    /// </summary>
    private void CheckKeys()
    {
        Vector3 xMove = Vector3.zero;
        Vector3 zMove = Vector3.zero;

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

        Vector3 move = (xMove + zMove).normalized * Mathf.Max(xMove.magnitude, zMove.magnitude);
        
        m_targetPosition = transform.position + move;
    }

}
