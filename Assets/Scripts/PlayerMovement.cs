using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//inputsystem
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    private Animator _animator;
    CharacterController controller;
    public InputAction moveAction;
    public float speed = 6f;
    private Vector3 lastMoveDir = Vector3.zero;

    private Vector3 animVelocity = Vector3.zero;

    [Header("Roll")]
    public InputAction rollAction;
    private Vector3 rollDir = Vector3.zero;
    public float rollSpeed = 10f;
    public float rollTime = 1f;
    private float rollTimer = 0f;
    public bool isRolling = false;
    private PlayerHealth playerHealth;
    private GunScript gunScript;



    public Vector3 mouseAimPoint = Vector3.zero;
    public Transform shootPoint;
    private Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        moveAction.Enable();
        rollAction.Enable();

        rollAction.performed += ctx => StartRoll();

        controller = GetComponent<CharacterController>();

        if (cam == null){
            cam = Camera.main;
        }

        playerHealth = GetComponent<PlayerHealth>();

        gunScript = GetComponentInChildren<GunScript>();

        _animator = GetComponentInChildren<Animator>();
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
        moveDir.y = 0;
        moveDir.Normalize();
        moveDir *= speed;
        moveDir.y = -2.0f;
        
        animVelocity = Vector3.Lerp(animVelocity, moveDir, Time.deltaTime * 10f);
        
        //roll
        if (rollTimer > 0)
        {
            rollTimer -= Time.deltaTime;

            //make invulnerable
            playerHealth.isInvulnerable = true;
            isRolling = true;

            //calc roll direction
            moveDir = rollDir * rollSpeed;
            moveDir.y = -2.0f;

            //roll
            controller.Move(moveDir * Time.deltaTime);

            lastMoveDir = rollDir;
        }
        else{
            rollTimer = 0;

            //make vulnerable
            playerHealth.isInvulnerable = false;
            isRolling = false;

            _animator.SetBool("Walking", false);
            _animator.SetBool("Aiming", false);



            if (!gunScript.isAiming){
                //apply movement
                //transform.position += (moveDir * speed * Time.deltaTime);
                controller.Move(moveDir * Time.deltaTime);

                

                //calc signed magnitude of movement for right and forward
                float rightMag = Vector3.Dot(transform.right, animVelocity.normalized);
                float forwardMag = Vector3.Dot(transform.forward, animVelocity.normalized);

                

                _animator.SetFloat("MoveX", rightMag);
                _animator.SetFloat("MoveZ", forwardMag);
                if (move.magnitude > 0.1f)
                {
                    _animator.SetBool("Walking", true);
                }
            }
            else{
                _animator.SetFloat("MoveX", 0);
                _animator.SetFloat("MoveZ", 0);
                _animator.SetBool("Aiming", true);
            }

            

            //calc the direction to look
            Vector3 lookDir = GetMouseAimPoint() - transform.position;
            //remove vertical
            lookDir.y = 0;
            lookDir.Normalize();
            //apply rotation
            transform.rotation = Quaternion.LookRotation(lookDir);

            lastMoveDir = moveDir;
        }
    }

    public void StartRoll(){
        isRolling = true;

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
        moveDir.y = 0;
        moveDir.Normalize();

        if (move != Vector2.zero){
            rollDir = moveDir;
        }
        else{
            rollDir = mouseAimPoint - transform.position;
            rollDir.y = 0;
            rollDir.Normalize();
        }

        transform.rotation = Quaternion.LookRotation(rollDir);

        rollTimer = rollTime;

        _animator.SetTrigger("Dodge");
    }

    public Vector3 GetMouseAimPoint(){
        //mouse raycast to get direction
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        //find where ray intersects on the plane of the player
        Plane playerPlane = new Plane(Vector3.up, shootPoint.position);
        float rayDistance;
        if (playerPlane.Raycast(ray, out rayDistance)){
            //get mouse hit pos
            Vector3 hitPoint = ray.GetPoint(rayDistance);
            mouseAimPoint = hitPoint;
        }
        return mouseAimPoint;
    }
}
