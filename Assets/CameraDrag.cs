using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraDrag : MonoBehaviour
{
    public float dragSpeed = 2;
    private Vector2 dragOrigin;
 
 
    void Update()
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
 
        transform.Translate(move, Space.World);
    }
 
 
}
