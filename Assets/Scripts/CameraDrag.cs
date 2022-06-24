using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraDrag : MonoBehaviour
{
    public float dragSpeed = 2;
    private Vector2 dragOrigin;

    public Vector3 center = new Vector3(0, 0, 0);
    public float maxDistance = 10;

    private void Start() {
        center = transform.position;
    }
 
 
    void Update()
    {
        if ((transform.position - center).magnitude > maxDistance)
        {
            // lerp to the max distance
            transform.position = Vector3.Lerp(transform.position, center, Time.deltaTime);
        }

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
 
        transform.Translate(move, Space.World);
    }
 
 
}
