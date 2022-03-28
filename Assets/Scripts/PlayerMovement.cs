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

    public Vector3 mouseAimPoint = Vector3.zero;

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
        if (cam == null || controller == null) {
            Debug.LogError("PlayerMovement: Missing camera or controller");
            return;
        }

        Move();
    }

    void Move()
    {
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
        moveDir *= speed;
        moveDir.y = -2.0f;

        //apply movement
        //transform.position += (moveDir * speed * Time.deltaTime);
        controller.Move(moveDir * Time.deltaTime);

        //calc the direction to look
        Vector3 lookDir = GetMouseAimPoint() - transform.position;
        //remove vertical
        lookDir.y = 0;
        lookDir.Normalize();
        //apply rotation
        transform.rotation = Quaternion.LookRotation(lookDir);
    }

    public Vector3 GetMouseAimPoint(){
        //mouse raycast to get direction
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        //find where ray intersects on the plane of the player
        Plane playerPlane = new Plane(Vector3.up, transform.position);
        float rayDistance;
        if (playerPlane.Raycast(ray, out rayDistance)){
            //get mouse hit pos
            Vector3 hitPoint = ray.GetPoint(rayDistance);
            mouseAimPoint = hitPoint;
        }
        return mouseAimPoint;
    }
}
