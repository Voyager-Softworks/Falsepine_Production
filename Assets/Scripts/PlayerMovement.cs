using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//inputsystem
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public InputAction moveAction;
    public Camera cam;
    CharacterController controller;
    public float speed = 6f;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();

        if (cam == null){
            cam = Camera.main;
        }

        moveAction.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    void Move()
    {
        if (cam == null || controller == null) return;

        //get cam direction
        Vector3 camForward = cam.transform.forward;
        camForward.y = 0;
        camForward.Normalize();
        Vector3 camRight = cam.transform.right;
        camRight.y = 0;
        camRight.Normalize();

        //get movement direction using input and cam direction
        Vector2 move = moveAction.ReadValue<Vector2>();
        Vector3 moveDir = move.x * camRight + move.y * camForward;
        moveDir.Normalize();

        //apply movement
        //transform.position += (moveDir * speed * Time.deltaTime);
        controller.Move(moveDir * speed * Time.deltaTime);

        //mouse raycast to get direction
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            Vector3 lookDir = hit.point - transform.position;
            lookDir.y = 0;
            lookDir.Normalize();
            transform.rotation = Quaternion.LookRotation(lookDir);
        }
    }
}
