using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//inputsystem
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour  /// @todo Comment
{

    [Header("Movement")]
    public Animator _animator;
    CharacterController controller;
    DynamicVaulting dynamicVaulting;
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



    public Vector3 mouseAimPoint = Vector3.zero;
    public Transform shootPoint;
    private Camera cam;
    private AudioSource audioSource;

    public AudioClip dodgeSound;
    public AudioClip[] footstepSounds;

    Vector3 camForward;
    Vector3 camRight;


    Vector3 planeIntersect;
    private void OnDrawGizmos() {
        // draw mouse plane aim point
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(mouseAimPoint, 0.1f);

        //draw plane intersect point
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(planeIntersect, 0.2f);
    }

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

        

        controller = GetComponent<CharacterController>();
        dynamicVaulting = GetComponent<DynamicVaulting>();

        rollAction.performed += ctx => { if(dynamicVaulting.canVault) StartVault(); else StartRoll(); };

        if (cam == null){
            cam = Camera.main;
        }

        playerHealth = GetComponent<PlayerHealth>();

        //_animator = GetComponentInChildren<Animator>();

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
        if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Player_Rig_BearTrap_Revolver") ||
            _animator.GetCurrentAnimatorStateInfo(0).IsName("Player_Rig_BearTrap_Rifle") ||
            _animator.GetCurrentAnimatorStateInfo(0).IsName("Player_Rig_BearTrap_Shotgun"))
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

    public void StartVault()
    {
        if(!dynamicVaulting.canVault) return;
        rollDir = dynamicVaulting.GetVaultingDirection();
        _animator.SetFloat("VaultHeight", dynamicVaulting.GetVaultingHeight());
        _animator.SetTrigger("Vault");
        
        _animator.SetLayerWeight(1, 0);
        _animator.SetLayerWeight(2, 0);

        
        StartCoroutine(VaultCoroutine());

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

IEnumerator VaultCoroutine()
{
    GetComponent<CapsuleCollider>().enabled = false;
    GetComponent<CharacterController>().enabled = false;
    Vector3 startPos = transform.position;
    Vector3 endPos = transform.position + (rollDir * 2.2f);
    float t = 0;
    while (t < 1)
    {
        t += Time.deltaTime / 0.7f;
        transform.position = Vector3.Lerp(startPos, endPos, t);
        yield return null;
    }
    GetComponent<CapsuleCollider>().enabled = true;
    GetComponent<CharacterController>().enabled = true;
}

public Vector3 GetMouseAimPoint(){
        //mouse raycast to get direction
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        PlayerInventoryInterface pii = GetComponent<PlayerInventoryInterface>();
        if (!pii) return mouseAimPoint;
        GameObject weaponFirepoint = pii.GetWeaponFirepoint(pii.selectedWeapon);
        if (!weaponFirepoint) return mouseAimPoint;

        Vector3 mousePlanePoint = new Vector3();

        Vector3 firePoint = weaponFirepoint.transform.position;

        //find where ray intersects on the plane at gun height
        Plane playerPlane = new Plane(Vector3.up, firePoint);
        float rayDistance;
        if (playerPlane.Raycast(ray, out rayDistance)){
            //get mouse hit pos
            Vector3 hitPoint = ray.GetPoint(rayDistance);
            mousePlanePoint = hitPoint;
        }
        return mousePlanePoint;

        // if dist between fire point and mousePlaneAimPoint is less than 1, return it
        // if (Vector3.Distance(firePoint, mousePlanePoint) < 1)
        // {
        //     return mousePlanePoint;
        // }

        //return mousePlaneAimPoint;


        /// Following code treats the aiming like a cone, allowing some vertical/horiz movement.

        // Vector3 exactMouseAimPoint = new Vector3();
        // RaycastHit hit;
        // if (Physics.Raycast(ray, out hit, 1000f)){
        //     exactMouseAimPoint = hit.point;
        // }

        // //Vector3 cp = ClosestPointOnLine(firePoint, mousePlanePoint, exactMouseAimPoint);
        // Vector3 cp = LineIntersectsPlane(exactMouseAimPoint - Vector3.up * 100, exactMouseAimPoint + Vector3.up * 100, firePoint, Vector3.up);
        // planeIntersect = cp;

        // Vector3 cpToExact = exactMouseAimPoint - cp;
        // float cpToExactMag = cpToExact.magnitude;

        // float angle = 20.0f;
        // float adjacent = (cp - firePoint).magnitude;
        // // calc opposite using angle and adjacent
        // float opposite = Mathf.Tan(angle * Mathf.Deg2Rad) * adjacent;

        // //clamp cpToExactMag to opposite
        // float newDist = Mathf.Clamp(cpToExactMag, 0, opposite);

        // mouseAimPoint = cp + cpToExact.normalized * newDist;

        // return mouseAimPoint;
    }

    public Vector3 ClosestPointOnLine(Vector3 lineStart, Vector3 lineEnd, Vector3 point){
        Vector3 lineDir = lineEnd - lineStart;
        lineDir.Normalize();
        float dot = Vector3.Dot(lineDir, point - lineStart);
        return lineStart + lineDir * dot;
    }

    public Vector3 LineIntersectsPlane(Vector3 lineStart, Vector3 lineEnd, Vector3 planePoint, Vector3 planeNormal){
        Vector3 lineDir = lineEnd - lineStart;
        lineDir.Normalize();
        float dot = Vector3.Dot(planeNormal, lineDir);
        if (Mathf.Abs(dot) < 0.00001f){
            return Vector3.zero;
        }
        float dot2 = Vector3.Dot(planeNormal, lineStart - planePoint);
        float t = -dot2 / dot;
        return lineStart + lineDir * t;
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
