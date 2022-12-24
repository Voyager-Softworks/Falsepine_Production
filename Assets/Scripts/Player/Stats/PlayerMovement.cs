using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//inputsystem
using UnityEngine.InputSystem;

/// <summary>
/// Controls the player movement and some mouse control
/// </summary>
public class PlayerMovement : MonoBehaviour
{

    [Header("Movement")]
    public bool m_disableGamepadMovement = false;
    public Animator _animator; ///< The animator for the player.
    CharacterController controller; ///< The character controller for the player.
    DynamicVaulting dynamicVaulting; ///< The dynamic vaulting script for the player.
    public InputAction moveAction; ///< The action for moving the player.
    public InputAction sprintAction; ///< The action for sprinting.
    public float walkSpeed = 6f; ///< The speed of the player when walking.
    public float jogSpeed = 10f; ///< The speed of the player when jogging.
    public float sprintSpeed = 14f; ///< The speed of the player when sprinting.
    private Vector3 lastMoveDir = Vector3.zero; ///< The last direction the player moved.

    private Vector3 animVelocity = Vector3.zero; ///< The velocity of the player for the animator.

    [Header("Roll")]
    public InputAction rollAction; ///< The action for rolling.
    private Vector3 rollDir = Vector3.zero; ///< The direction of the roll.
    public float rollSpeed = 10f; ///< The speed of the roll.
    public float rollTime = 1f; ///< The time the roll takes.
    private float rollTimer = 0f; ///< The timer for the roll.
    public float rollInvincibilityTime = 0.35f; ///< The time the player is invincible after rolling.
    private float rollInvincibilityTimer = 0f; ///< The timer for the invincibility after rolling.
    public float rollDelay = 1.5f; ///< The delay before the roll can be used again.
    private float rollDelayTimer = 0f; ///< The timer for the roll delay.
    private float postRollDelay = 0.25f; ///< The delay after the roll before the player is considered to not be rolling.
    public bool isRolling = false; ///< Whether or not the player is rolling.
    public bool isVaulting = false; ///< Whether or not the player is vaulting.

    public AudioClip rollSound; ///< The sound to play when the player rolls.
    private PlayerHealth playerHealth; ///< The player health script for the player.



    //public Vector3 mousePlanePoint = Vector3.zero;
    public Transform shootPoint; ///< The point the player shoots from.
    private Camera cam; ///< The camera for the player.
    private AudioSource audioSource; ///< The audio source for the player.

    public AudioClip dodgeSound; ///< The sound to play when the player dodges.
    public AudioClip[] footstepSounds; ///< The footstep sounds for the player.

    Vector3 camForward; ///< The forward direction of the camera.
    Vector3 camRight; ///< The right direction of the camera.

    [HideInInspector] public UIScript uiScript;

    [Header("AutoWalk")]
    private Vector3 startPos = Vector3.zero;
    public float walkDistance = 10.0f;
    public float autoWalkSpeed = 3.0f;
    public bool doAutoWalk = true;

    private bool m_ignoringEnemyCollisions = false;

    /// <summary>
    ///  Plays a footstep sound.
    /// </summary>
    public void DoFootstep()
    {
        int random = Random.Range(0, footstepSounds.Length);
        audioSource.PlayOneShot(footstepSounds[random]);
    }

    private void OnDrawGizmos()
    {
        // draw a line from the player showing the walk distance and direction
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * walkDistance);

        // draw a line backwards to the nearest collider
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -transform.forward, out hit, 10.0f))
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, hit.point);
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        dynamicVaulting = GetComponent<DynamicVaulting>();

        rollAction.performed += ctx => { if (dynamicVaulting.canVault) StartVault(); else StartRoll(); };

        // LevelController.GamePaused += () => {
        //     DisableInput();
        // };
        // LevelController.GameUnpaused += () => {
        //     EnableInput();
        // };

        if (cam == null)
        {
            cam = Camera.main;
        }

        uiScript = FindObjectOfType<UIScript>();

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

        startPos = transform.position;
    }

    private void OnEnable()
    {
        EnableInput();

        // disable Gamepad input
        if (m_disableGamepadMovement)
        {
            Gamepad[] gamepads = Gamepad.all.ToArray();
            for (int i = 0; i < gamepads.Length; i++)
            {
                InputSystem.DisableDevice(gamepads[i]);
            }
        }
    }

    private void OnDisable()
    {
        DisableInput();
    }

    /// <summary>
    /// Enables the input actions for the player movement.
    /// </summary>
    public void EnableInput()
    {
        moveAction.Enable();
        rollAction.Enable();
        sprintAction.Enable();
    }

    /// <summary>
    ///  Disable input actions for the player movement.
    /// </summary>
    public void DisableInput()
    {
        moveAction.Disable();
        rollAction.Disable();
        sprintAction.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        if (doAutoWalk && AutoWalk())
        {
            return;
        }

        if (LevelController.IsPaused || ToggleableTownWindow.AnyWindowOpen())
        {
            DisableInput();
        }
        else
        {
            EnableInput();
        }


        if (cam == null || controller == null)
        {
            Debug.LogError("PlayerMovement: Missing camera or controller");
            return;
        }

        //set the _WorldCutPos of the shader to the player's position
        Shader.SetGlobalVector("WorldCutPos", transform.position);

        Move();

        UpdateUI();
    }

    /// <summary>
    /// Auto walks into position for the level.
    /// </summary>
    /// <returns></returns>
    private bool AutoWalk()
    {
        // if far enough away from back pos, stop
        if (Vector3.Distance(transform.position, startPos) >= walkDistance)
        {
            doAutoWalk = false;
            return false;
        }

        // if any input is pressed, stop auto walk
        if (moveAction.ReadValue<Vector2>().magnitude > 0.1f)
        {
            doAutoWalk = false;
            return false;
        }

        // auto walk into scene
        _animator.SetBool("Aiming", false);
        _animator.SetBool("Jogging", false);
        _animator.SetBool("Running", false);

        //apply movement
        //transform.position += (moveDir * speed * Time.deltaTime);
        _animator.SetLayerWeight(1, Mathf.Lerp(_animator.GetLayerWeight(1), 1, Time.deltaTime * 10f));
        _animator.SetLayerWeight(2, Mathf.Lerp(_animator.GetLayerWeight(2), 1, Time.deltaTime * 10f));

        Vector3 moveDir = transform.forward * autoWalkSpeed;
        moveDir.y = -2.0f;

        animVelocity = Vector3.Lerp(animVelocity, moveDir, Time.deltaTime * 10f);

        //calc signed magnitude of movement for right and forward
        float rightMag = Vector3.Dot(transform.right, animVelocity.normalized);
        float forwardMag = Vector3.Dot(transform.forward, animVelocity.normalized);
        _animator.SetFloat("MoveSide", rightMag);
        _animator.SetFloat("MoveForward", forwardMag);

        //set the look direction
        SetLookDirection(transform.forward);

        // move
        controller.Move(moveDir * Time.deltaTime);

        return true;
    }

    public void UpdateUI()
    {
        if (uiScript == null) return;

        Vector2 actualSize = new Vector2(uiScript.staminaBarMaxWidth * (1.0f - rollDelayTimer / rollDelay), uiScript.staminaBar.rectTransform.sizeDelta.y);

        uiScript.staminaBar.rectTransform.sizeDelta = actualSize;

        // fade out stamina bar when not rolling and stamina is full
        if (rollDelayTimer <= 0)
        {
            uiScript.staminaBG.color = Color.Lerp(uiScript.staminaBG.color, new Color(uiScript.staminaBG.color.r, uiScript.staminaBG.color.g, uiScript.staminaBG.color.b, 0), Time.deltaTime * 10);
            uiScript.staminaBar.color = Color.Lerp(uiScript.staminaBar.color, new Color(uiScript.staminaBar.color.r, uiScript.staminaBar.color.g, uiScript.staminaBar.color.b, 0), Time.deltaTime * 10);
            uiScript.staminaBarDark.color = Color.Lerp(uiScript.staminaBarDark.color, new Color(uiScript.staminaBarDark.color.r, uiScript.staminaBarDark.color.g, uiScript.staminaBarDark.color.b, 0), Time.deltaTime * 10);
        }
        else
        {
            uiScript.staminaBG.color = Color.Lerp(uiScript.staminaBG.color, new Color(uiScript.staminaBG.color.r, uiScript.staminaBG.color.g, uiScript.staminaBG.color.b, 1), Time.deltaTime * 20);
            uiScript.staminaBar.color = Color.Lerp(uiScript.staminaBar.color, new Color(uiScript.staminaBar.color.r, uiScript.staminaBar.color.g, uiScript.staminaBar.color.b, 1), Time.deltaTime * 20);
            uiScript.staminaBarDark.color = Color.Lerp(uiScript.staminaBarDark.color, new Color(uiScript.staminaBarDark.color.r, uiScript.staminaBarDark.color.g, uiScript.staminaBarDark.color.b, 1), Time.deltaTime * 20);
        }
    }

    /// <summary>
    ///  Moves the player according to input data.
    /// </summary>
    void Move()
    {
        //get movement direction using input and cam direction
        Vector2 move = moveAction.ReadValue<Vector2>();
        Vector3 moveDir = move.x * camRight + move.y * camForward;
        moveDir.y = 0;
        moveDir.Normalize();

        bool isAiming = false;
        bool isReloading = false;
        bool isThrowing = false;
        PlayerInventoryInterface pii = GetComponent<PlayerInventoryInterface>();
        if (pii)
        {
            RangedWeapon rangedWeapon = pii.selectedWeapon as RangedWeapon;
            if (rangedWeapon)
            {
                isAiming = rangedWeapon.m_isAiming;
                isReloading = rangedWeapon.m_isReloading;
            }

            if (pii.m_currentlyThrowingItem != null)
            {
                isThrowing = true;
            }
        }

        if (dynamicVaulting.canVault && !isVaulting && !isAiming && !isRolling)
        {
            if (Vector3.Dot(moveDir, dynamicVaulting.GetVaultingDirection()) > 0.5f)
            {
                StartVault();

                // DoReload false
                _animator.SetBool("DoReload", false);
            }
        }

        if (isAiming && !isRolling)
        {
            moveDir *= walkSpeed;
        }
        else if ((sprintAction.ReadValue<float>() > 0.1f || CustomInputManager.LastInputWasGamepad) && !isReloading)
        {
            moveDir *= sprintSpeed;
        }
        else
        {
            moveDir *= jogSpeed;
        }

        moveDir.y = -2.0f;

        animVelocity = Vector3.Lerp(animVelocity, moveDir, Time.deltaTime * 10f);
        if (playerHealth.isStunned) return;
        //check if placing bear trap anim is playing
        if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Player_Rig_BearTrap_Revolver") ||
            _animator.GetCurrentAnimatorStateInfo(0).IsName("Player_Rig_BearTrap_Rifle") ||
            _animator.GetCurrentAnimatorStateInfo(0).IsName("Player_Rig_BearTrap_Shotgun"))
        {
            return;
        }

        if (rollInvincibilityTimer > 0f)
        {
            rollInvincibilityTimer -= Time.deltaTime;

            //make invulnerable
            playerHealth.isInvulnerable = true;
        }

        //roll
        if (rollTimer > 0)
        {
            rollTimer -= Time.deltaTime;
            isRolling = true;

            //calc roll direction
            moveDir = rollDir * rollSpeed;
            moveDir.y = -2.0f;

            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(rollDir), Time.deltaTime * 15f);

            //roll
            controller.Move(moveDir * Time.deltaTime);

            lastMoveDir = rollDir;
        }
        else
        {
            IgnoreEnemyCollision(false);

            rollTimer = 0;
            if (rollDelayTimer > 0)
            {
                rollDelayTimer -= Time.deltaTime;
            }

            //make vulnerable
            playerHealth.isInvulnerable = false;

            // few sec after roll, set to false
            if (rollDelayTimer <= rollDelay - postRollDelay)
            {
                isRolling = false;
            }



            _animator.SetBool("Aiming", isAiming && !isRolling && !isVaulting);
            _animator.SetBool("Jogging", !isAiming && move.magnitude > 0.1f);
            _animator.SetBool("Running", (sprintAction.ReadValue<float>() > 0.1f || CustomInputManager.LastInputWasGamepad) && move.magnitude > 0.1f);

            bool isMeleeAttacking = false;
            if (pii)
            {
                MeleeWeapon meleeWeapon = pii.selectedMeleeWeapon as MeleeWeapon;
                if (meleeWeapon)
                {
                    isMeleeAttacking = meleeWeapon.m_comboTimer > 0;
                }
            }

            controller.Move(moveDir * Time.deltaTime);
            if ((isAiming || isReloading || isThrowing || isMeleeAttacking) && (!isRolling && !isVaulting))
            {
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
                if (CustomInputManager.LastInputWasGamepad)
                {
                    lookDir = GetGamepadAimPoint() - transform.position;
                }
                else
                {
                    lookDir = GetMouseAimPlanePoint() - transform.position;
                }
                //lookDir = GetMouseAimPlanePoint() - transform.position;


                // Look at the look direction
                SetLookDirection(lookDir);
            }
            else
            {
                _animator.SetFloat("MoveSide", 0);
                _animator.SetFloat("MoveForward", 0);
                _animator.SetLayerWeight(1, Mathf.Lerp(_animator.GetLayerWeight(1), 0, Time.deltaTime * 10f));
                _animator.SetLayerWeight(2, Mathf.Lerp(_animator.GetLayerWeight(2), 0, Time.deltaTime * 10f));
                if (move.magnitude > 0.1f)
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(new Vector3(moveDir.x, 0.0f, moveDir.z)), Time.deltaTime * 10f);
            }

            lastMoveDir = moveDir;
        }
    }

    /// <summary>
    /// Changes how the player colliders interact with enemy colliders
    /// </summary>
    private void IgnoreEnemyCollision(bool _ignore = true)
    {
        if (m_ignoringEnemyCollisions == _ignore) return;

        // change collision with enemies:
        // get all colliders in children
        List<Collider> playerColliders = new List<Collider>();
        playerColliders.AddRange(GetComponentsInChildren<Collider>());

        // get all enemy colliders
        List<Collider> enemyColliders = new List<Collider>();
        foreach (Health_Base enemy in Health_Base.allHealths)
        {
            if (enemy as EnemyHealth == null) continue;

            // add all children colliders
            enemyColliders.AddRange(enemy.GetComponentsInChildren<Collider>());
        }

        // change collisions
        foreach (Collider playerCollider in playerColliders)
        {
            foreach (Collider enemyCollider in enemyColliders)
            {
                Physics.IgnoreCollision(playerCollider, enemyCollider, _ignore);
            }
        }

        m_ignoringEnemyCollisions = _ignore;
    }

    /// <summary>
    ///  Sets the look direction of the player.
    /// </summary>
    /// <param name="lookDir">The direction to look.</param>
    public void SetLookDirection(Vector3 lookDir)
    {
        lookDir.y = 0;
        transform.rotation = Quaternion.LookRotation(lookDir.normalized);
    }

    /// <summary>
    /// Begin vaulting over an object.
    /// </summary>
    /// @todo Vaulting is currently janky, and needs to be fixed.
    /// Fixes needed include:
    /// - Vaulting is not working when the player is aiming: The player will phase through the object.
    /// - Vaulting is currently too slow, players find this annoying.
    /// - Vaulting currently messes up enemy pathfinding: this needs to be fixed.
    public void StartVault()
    {
        if (rollDelayTimer > 0)
        {
            return;
        }
        if (!dynamicVaulting.canVault || isRolling || isVaulting) return;
        _animator.SetFloat("VaultHeight", dynamicVaulting.GetVaultingHeight());
        _animator.SetTrigger("Vault");

        _animator.SetLayerWeight(1, 0);
        _animator.SetLayerWeight(2, 0);


        StartCoroutine(VaultCoroutine(dynamicVaulting.GetVaultingDirection()));

    }

    /// <summary>
    ///  Starts rolling.
    /// </summary>
    public void StartRoll()
    {
        if (rollDelayTimer > 0)
        {
            return;
        }
        if (isRolling || isVaulting)
        {
            return;
        }
        //check if placing bear trap anim is playing
        if (_animator.GetCurrentAnimatorStateInfo(3).IsName("Player|PLACE TRAP (ALL)") ||
            _animator.GetCurrentAnimatorStateInfo(3).IsName("Player|PAIN (ALL)"))
        {
            return;
        }

        // stop reloading
        if (FindObjectOfType<PlayerInventoryInterface>() is PlayerInventoryInterface pii)
        {
            pii.TryEndReload();
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

        if (move != Vector2.zero)
        {
            rollDir = moveDir;
        }
        else
        {
            rollDir = GetMouseAimPlanePoint() - transform.position;
            rollDir.y = 0;
            rollDir.Normalize();
        }

        //transform.rotation = Quaternion.LookRotation(rollDir);


        IgnoreEnemyCollision(true);

        rollTimer = rollTime;
        rollInvincibilityTimer = rollInvincibilityTime;

        _animator.SetTrigger("Dodge");
        _animator.SetLayerWeight(1, 0);
        _animator.SetLayerWeight(2, 0);
        audioSource.PlayOneShot(rollSound);
    }

    /// <summary>
    ///  Coroutine for vaulting.
    /// </summary>
    /// <returns></returns>
    IEnumerator VaultCoroutine(Vector3 dir)
    {
        isVaulting = true;
        GetComponent<CapsuleCollider>().enabled = false;
        GetComponent<CharacterController>().enabled = false;
        float vaultHeight = dynamicVaulting.GetVaultingHeight();
        Vector3 startPos = transform.position;
        Vector3 endPos = transform.position + (dir * dynamicVaulting.maxVaultingDepth);  //transform.position + (rollDir * 2.2f);
        float t = 0;
        while (t < 1)
        {
            isVaulting = true;
            t += Time.deltaTime / 0.7f;
            transform.position = Vector3.Lerp(startPos, endPos, dynamicVaulting.vaultingCurve.Evaluate(t)) + ((dynamicVaulting.vaultingHeightCurve.Evaluate(t) * vaultHeight) * Vector3.up);
            yield return null;
        }
        GetComponent<CapsuleCollider>().enabled = true;
        GetComponent<CharacterController>().enabled = true;
        isVaulting = false;
        rollDelayTimer = rollDelay;
    }


    /// <summary>
    /// Get the position of the mouse on the plane at the height of the weapon firepoint
    /// </summary>
    /// <returns>
    /// A Vector3 of the mouse position on the plane at the height of the weapon firepoint.
    /// @deprecated This function is deprecated, use GetMouseAimPlanePoint() instead.
    /// </returns>
    public Vector3 GetMouseWeaponPlanePoint()
    {
        //mouse raycast to get direction
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        Vector3 mousePlanePoint = new Vector3();

        PlayerInventoryInterface pii = GetComponent<PlayerInventoryInterface>();
        if (!pii) return mousePlanePoint;
        Transform weaponFirepoint = pii.GetWeaponFirepoint(pii.selectedWeapon);
        if (!weaponFirepoint) return mousePlanePoint;

        Vector3 firePoint = weaponFirepoint.position;

        //find where ray intersects on the plane at gun height
        Plane playerPlane = new Plane(Vector3.up, firePoint);
        float rayDistance;
        if (Gamepad.current != null && Gamepad.current.rightStick.ReadValue().magnitude > 0.1f)
        {
            mousePlanePoint = GetGamepadAimPoint();
        }
        else if (playerPlane.Raycast(ray, out rayDistance))
        {
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

    /// <summary>
    /// It takes the mouse position, casts a ray from the camera to the mouse position, and then finds
    /// the point where that ray intersects with a plane at the height of the gun
    /// </summary>
    /// <returns>
    /// A Vector3
    /// </returns>
    public Vector3 GetMouseAimPlanePoint()
    {
        //mouse raycast to get direction
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        Vector3 mousePlanePoint = new Vector3();

        PlayerInventoryInterface pii = GetComponent<PlayerInventoryInterface>();
        if (!pii) return mousePlanePoint;
        GameObject _aimZone = pii.m_aimZone?.gameObject;
        if (!_aimZone) return mousePlanePoint;

        Vector3 aimZonePosition = _aimZone.transform.position;

        //find where ray intersects on the plane at gun height
        Plane playerPlane = new Plane(Vector3.up, aimZonePosition);
        float rayDistance;
        if (playerPlane.Raycast(ray, out rayDistance))
        {
            //get mouse hit pos
            Vector3 hitPoint = ray.GetPoint(rayDistance);
            mousePlanePoint = hitPoint;
        }
        return mousePlanePoint;
    }

    /// <summary>
    ///  Find the closest point on a line to a given point.
    /// </summary>
    /// <param name="lineStart">The start position of the line.</param>
    /// <param name="lineEnd">The end position of the line.</param>
    /// <param name="point">The given point.</param>
    /// <returns>The point on the defined line which is closest to the given point.</returns>
    public Vector3 ClosestPointOnLine(Vector3 lineStart, Vector3 lineEnd, Vector3 point)
    {
        Vector3 lineDir = lineEnd - lineStart;
        lineDir.Normalize();
        float dot = Vector3.Dot(lineDir, point - lineStart);
        return lineStart + lineDir * dot;
    }

    /// <summary>
    ///  Find the intersect of a line and a plane.
    /// </summary>
    /// <param name="lineStart">The start position of the line.</param>
    /// <param name="lineEnd">The end position of the line.</param>
    /// <param name="planePoint">The origin of the plane.</param>
    /// <param name="planeNormal">The normal of the plane.</param>
    /// <returns>The point on the plane where the line intersects.</returns>
    public Vector3 LineIntersectsPlane(Vector3 lineStart, Vector3 lineEnd, Vector3 planePoint, Vector3 planeNormal)
    {
        Vector3 lineDir = lineEnd - lineStart;
        lineDir.Normalize();
        float dot = Vector3.Dot(planeNormal, lineDir);
        if (Mathf.Abs(dot) < 0.00001f)
        {
            return Vector3.zero;
        }
        float dot2 = Vector3.Dot(planeNormal, lineStart - planePoint);
        float t = -dot2 / dot;
        return lineStart + lineDir * t;
    }

    /// <summary>
    ///  Gets the aim point using Gamepad input.
    /// </summary>
    /// <returns>The point the player should aim at.</returns>
    /// @deprecated Gamepad Input is no longer maintained and is currently not supported. It may be added back in the future.
    public Vector3 GetGamepadAimPoint()
    {
        PlayerInventoryInterface pii = GetComponent<PlayerInventoryInterface>();
        if (!pii) return Vector3.zero;
        GameObject _aimZone = pii.m_aimZone?.gameObject;

        //Get the right stick value
        Vector2 rightStick = Gamepad.current.rightStick.ReadValue();
        //Get the direction of the right stick
        Vector3 rightStickDir = rightStick.x * camRight + rightStick.y * camForward;

        Vector3 aimPoint = rightStickDir * 10f + transform.position;
        aimPoint.y = _aimZone.transform.position.y;
        return aimPoint;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // if rolling, break props
        if (!isRolling) return;

        // get prophealth from parents and children
        PropHealth prop = hit.gameObject.GetComponentInParent<PropHealth>() ?? hit.gameObject.GetComponentInChildren<PropHealth>();
        // if not found, return
        if (prop == null) return;

        // ignore explosive props
        if (prop.GetComponent<ExplodeOnDeath>() != null) return;

        // check distance to prop
        if (Vector3.Distance(prop.transform.position, transform.position) > 4f) return;

        // deal damage equal to full health
        prop.TakeDamage(new Health_Base.DamageStat(
            prop.m_currentHealth,
            gameObject,
            transform.position,
            prop.transform.position,
            new List<StatsManager.StatType>() { StatsManager.StatType.PlayerDamage }
            ));
    }
}
