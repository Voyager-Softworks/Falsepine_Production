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
    public InputAction sprintAction;
    public float walkSpeed = 6f;
    public float jogSpeed = 10f;
    public float sprintSpeed = 14f;
    private Vector3 lastMoveDir = Vector3.zero;

    private Vector3 animVelocity = Vector3.zero;

    [Header("Roll")]
    public InputAction rollAction;
    private Vector3 rollDir = Vector3.zero;
    public float rollSpeed = 10f;
    public float rollTime = 1f;
    private float rollTimer = 0f;
    public float rollDelay = 1.5f;
    private float rollDelayTimer = 0f;
    public bool isRolling = false;
    public AudioClip rollSound;
    private PlayerHealth playerHealth;
    private GunScript gunScript;



    public Vector3 mouseAimPoint = Vector3.zero;
    public Transform shootPoint;
    private Camera cam;
    private AudioSource audioSource;

    public AudioClip dodgeSound;
    public AudioClip[] footstepSounds;

    Vector3 camForward;
    Vector3 camRight;

    public void DoFootstep()
    {
        int random = Random.Range(0, footstepSounds.Length);
        audioSource.PlayOneShot(footstepSounds[random]);
    }


    // Start is called before the first frame update
    void Start()
    {
        moveAction.Enable();
        rollAction.Enable();
        sprintAction.Enable();

        rollAction.performed += ctx => StartRoll();

        controller = GetComponent<CharacterController>();

        if (cam == null){
            cam = Camera.main;
        }

        playerHealth = GetComponent<PlayerHealth>();

        gunScript = GetComponentInChildren<GunScript>();

        _animator = GetComponentInChildren<Animator>();

        audioSource = GetComponent<AudioSource>();

        //get cam direction
        camForward = cam.transform.forward;
        camForward.y = 0;
        camForward.Normalize();
        camRight = cam.transform.right;
        camRight.y = 0;
        camRight.Normalize();
    }

    private void OnDestroy() {
        DisableInput();
    }

    private void OnDisable() {
        DisableInput();
    }

    public void DisableInput(){
        moveAction.Disable();
        rollAction.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        if (cam == null || controller == null) {
            Debug.LogError("PlayerMovement: Missing camera or controller");
            return;
        }

        //set the _WorldCutPos of the shader to the player's position
        Shader.SetGlobalVector("WorldCutPos", transform.position);

        Move();
    }

    void Move()
    {
        //get movement direction using input and cam direction
        Vector2 move = moveAction.ReadValue<Vector2>();
        Vector3 moveDir = move.x * camRight + move.y * camForward;
        moveDir.y = 0;
        moveDir.Normalize();

        bool isAiming = false;
        PlayerInventoryInterface pii = GetComponent<PlayerInventoryInterface>();
        if (pii){
            RangedWeapon rangedWeapon = pii.selectedWeapon as RangedWeapon;
            if (rangedWeapon){
                isAiming = rangedWeapon.m_isAiming;
            }
        }

        if(isAiming)
        { 
            moveDir *= walkSpeed;
        }
        else if(sprintAction.ReadValue<float>() > 0.1f)
        {
            moveDir *= sprintSpeed;
        }
        else
        {
            moveDir *= jogSpeed;
        }
        
        moveDir.y = -2.0f;
        
        animVelocity = Vector3.Lerp(animVelocity, moveDir, Time.deltaTime * 10f);
        if(playerHealth.isStunned) return;
        //check if placing bear trap anim is playing
        if (_animator.GetCurrentAnimatorStateInfo(3).IsName("Player|PLACE TRAP (ALL)") ||
            _animator.GetCurrentAnimatorStateInfo(3).IsName("Player|PAIN (ALL)"))
        {
            return;
        }
        
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

            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(rollDir), Time.deltaTime * 15f);

            //roll
            controller.Move(moveDir * Time.deltaTime);

            lastMoveDir = rollDir;
        }
        else{
            rollTimer = 0;
            if (rollDelayTimer > 0){
                rollDelayTimer -= Time.deltaTime;
            }

            //make vulnerable
            playerHealth.isInvulnerable = false;
            isRolling = false;
            

            _animator.SetBool("Walking", false);
            _animator.SetBool("Aiming", isAiming);
            _animator.SetBool("Jogging", !isAiming && move.magnitude > 0.1f);
            _animator.SetBool("Running", sprintAction.ReadValue<float>() > 0.1f);

            controller.Move(moveDir * Time.deltaTime);
            if (isAiming){
                //apply movement
                //transform.position += (moveDir * speed * Time.deltaTime);
                _animator.SetLayerWeight(1, Mathf.Lerp(_animator.GetLayerWeight(1), 1, Time.deltaTime * 10f));
                _animator.SetLayerWeight(2, Mathf.Lerp(_animator.GetLayerWeight(2), 1, Time.deltaTime * 10f));
                

                

                //calc signed magnitude of movement for right and forward
                float rightMag = Vector3.Dot(transform.right, animVelocity.normalized);
                float forwardMag = Vector3.Dot(transform.forward, animVelocity.normalized);

                

                _animator.SetFloat("MoveSide", rightMag);
                _animator.SetFloat("MoveForward", forwardMag);
                if (move.magnitude > 0.1f)
                {
                    _animator.SetBool("Walking", true);
                }
                //calc the direction to look
                Vector3 lookDir;
                if(Gamepad.current != null)
                {
                    lookDir = GetGamepadAimPoint() - transform.position;
                }
                else
                {
                    lookDir = GetMouseAimPoint() - transform.position;
                }
                

                //remove vertical
                lookDir.y = 0;
                lookDir.Normalize();
                //apply rotation
                //transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(lookDir), Time.deltaTime * 20f);
                transform.rotation = Quaternion.LookRotation(lookDir);
            }
            else{
                _animator.SetFloat("MoveSide", 0);
                _animator.SetFloat("MoveForward", 0);
                _animator.SetLayerWeight(1, Mathf.Lerp(_animator.GetLayerWeight(1), 0, Time.deltaTime * 10f));
                _animator.SetLayerWeight(2, Mathf.Lerp(_animator.GetLayerWeight(2), 0, Time.deltaTime * 10f));
                if(move.magnitude > 0.1f)
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(new Vector3(moveDir.x, 0.0f, moveDir.z)), Time.deltaTime * 10f);
            }

            

            

            lastMoveDir = moveDir;
        }
    }

    public void StartRoll(){
        if (rollDelayTimer > 0){
            return;
        }
        if (isRolling){
            return;
        }
        //check if placing bear trap anim is playing
        if (_animator.GetCurrentAnimatorStateInfo(3).IsName("Player|PLACE TRAP (ALL)") ||
            _animator.GetCurrentAnimatorStateInfo(3).IsName("Player|PAIN (ALL)"))
        {
            return;
        }
        

        isRolling = true;
        rollDelayTimer = rollDelay;

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

        //transform.rotation = Quaternion.LookRotation(rollDir);

        rollTimer = rollTime;

        _animator.SetTrigger("Dodge");
        _animator.SetLayerWeight(1, 0);
        _animator.SetLayerWeight(2, 0);
        audioSource.PlayOneShot(rollSound);
    }

    public Vector3 GetMouseAimPoint(){
        //mouse raycast to get direction
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        PlayerInventoryInterface pii = GetComponent<PlayerInventoryInterface>();
        if (!pii) return mouseAimPoint;
        GameObject weaponFirepoint = pii.GetWeaponFirepoint(pii.selectedWeapon);
        if (!weaponFirepoint) return mouseAimPoint;
        //find where ray intersects on the plane of the player
        Plane playerPlane = new Plane(Vector3.up, weaponFirepoint.transform.position);
        float rayDistance;
        if (playerPlane.Raycast(ray, out rayDistance)){
            //get mouse hit pos
            Vector3 hitPoint = ray.GetPoint(rayDistance);
            mouseAimPoint = hitPoint;
        }
        return mouseAimPoint;
    }

    public Vector3 GetGamepadAimPoint(){
        //Get the right stick value
        Vector2 rightStick = Gamepad.current.rightStick.ReadValue();
        //Get the direction of the right stick
        Vector3 rightStickDir = rightStick.x * camRight + rightStick.y * camForward;
        
        Vector3 aimPoint = rightStickDir * 10f + transform.position;
        aimPoint.y = shootPoint.position.y;
        return aimPoint;
        
    }

    
}
