using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System;

public class GunScript : MonoBehaviour
{
    private PlayerMovement playerMovement;
    private UIScript uiScript;
    private AudioSource audioSource;
    private Animator _animator;

    [Header("Shoot")]
    public InputAction shootAction;
    public List<AudioClip> shootClip;
    public AudioClip failedShootClip;
    public Transform shootPoint;
    public float damage = 10.0f;
    public int clipSize = 10;
    public int currentClip = 10;
    public float shootTime = 0.1f;
    public float shootTimer = 0.0f;
    public GameObject muzzleFlash;
    public GameObject hitEffect;
    public LayerMask shootMask;

    private struct Line
    {
        public Vector3 start;
        public Vector3 end;
        public float time;
    }

    private List<Line> lines = new List<Line>();


    [Header("Aim")]
    public InputAction aimAction;
    public AudioClip startAimClip;
    public AudioClip endAimClip;
    public float maxAimAngle = 25.0f;
    public float minAimAngle = 5.0f;
    public float currentAimAngle = 5.0f;
    public float aimTime = 1.0f;
    float currentAimTime = 0.0f;
    public bool isAiming = false;
    private Vector3 mouseAimPoint = Vector3.zero;
    private Vector3 rightAimAngle = Vector3.zero;
    private Vector3 leftAimAngle = Vector3.zero;
    public Color unAimedColor = Color.clear;
    public Color aimedColor = Color.red;
    public Image leftAimLineImage;
    public Image rightAimLineImage;
    public GameObject canvas;


    [Header("Reload")]
    public InputAction reloadAction;
    public AudioClip reloadClip;
    public float reloadTime = 1.0f;
    private float reloadTimer = 0.0f;

    private void OnDrawGizmos() {
        //draw current aim angle
        Gizmos.color = Color.red;
        //Gizmos.DrawLine(shootPoint.transform.position, shootPoint.transform.position + rightAimAngle * 5.0f);
        //Gizmos.DrawLine(shootPoint.transform.position, shootPoint.transform.position + leftAimAngle * 5.0f);

        //draw every line
        Gizmos.color = Color.blue;
        foreach (Line line in lines) {
            Gizmos.DrawLine(line.start, line.end);
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        shootAction.Enable();
        aimAction.Enable();
        reloadAction.Enable();

        playerMovement = FindObjectOfType<PlayerMovement>();
        uiScript = FindObjectOfType<UIScript>();
        audioSource = GetComponent<AudioSource>();
        _animator = GetComponentInChildren<Animator>();

        if (canvas != null){
            //if canvas, set scale to be 1 in relation to world
            canvas.transform.localScale = Vector3.one * (1.0f / transform.lossyScale.x);
        }

        UpdateUI();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerMovement == null || uiScript == null)
        {
            Debug.LogError("GunScript: Missing player movement or ui script");
            return;
        }

        UpdateAimAngle();

        UpdateAimLineUI();

        TimedActions();

        UpdateUI();
    }

    private void UpdateAimLineUI()
    {
        if (leftAimLineImage == null || rightAimLineImage == null)
        {
            Debug.LogError("GunScript: Missing aim line UI");
            return;
        }

        Color aimCol = isAiming ? aimedColor : unAimedColor;
        if (reloadTimer <= 0.0f && shootTimer <= 0.0f){
            aimCol = Color.Lerp(unAimedColor, aimedColor, (currentAimTime / aimTime));
        }
        else{
            aimCol = unAimedColor;
        }

        //check if placing bear trap anim is playing
        if (_animator.GetCurrentAnimatorStateInfo(3).IsName("Player|PLACE TRAP (ALL)") || 
            _animator.GetCurrentAnimatorStateInfo(3).IsName("Player|DODGE (ALL)") ||
            _animator.GetCurrentAnimatorStateInfo(3).IsName("Player|PAIN (ALL)"))
        {
            aimCol = Color.clear;
        }

        leftAimLineImage.color = aimCol;
        rightAimLineImage.color = aimCol;

        float length = Vector3.Distance(shootPoint.transform.position, mouseAimPoint) * 0.9f;

        leftAimLineImage.rectTransform.sizeDelta = new Vector2(leftAimLineImage.rectTransform.sizeDelta.x, length);
        rightAimLineImage.rectTransform.sizeDelta = new Vector2(rightAimLineImage.rectTransform.sizeDelta.x, length);

        leftAimLineImage.transform.localRotation = Quaternion.Euler(0, 0, -currentAimAngle);
        rightAimLineImage.transform.localRotation = Quaternion.Euler(0, 0, currentAimAngle);
    }

    private void TimedActions()
    {
        if (reloadTimer > 0.0f)
        {
            reloadTimer -= Time.deltaTime;
            //transform.LookAt(playerMovement.transform.position + playerMovement.transform.forward*2.0f - playerMovement.transform.up);
        }
        else if (shootTimer > 0.0f)
        {
            shootTimer -= Time.deltaTime;
            //transform.LookAt(playerMovement.transform.position + playerMovement.transform.forward*2.0f + playerMovement.transform.up);
        }

        //check if placing bear trap anim is playing
        if (_animator.GetCurrentAnimatorStateInfo(3).IsName("Player|PLACE TRAP (ALL)") || 
            _animator.GetCurrentAnimatorStateInfo(3).IsName("Player|DODGE (ALL)") ||
            _animator.GetCurrentAnimatorStateInfo(3).IsName("Player|PAIN (ALL)"))
        {
            return;
        }

        if (reloadTimer <= 0.0f && shootTimer <= 0.0f && !playerMovement.isRolling)
        {
            LookAtCursor();

            if (reloadAction.triggered)
            {
                TryReload();
            }
            if (shootAction.triggered)
            {
                TryShoot();
            }
        }
    }

    private void TryShoot()
    {
        if (currentClip > 0)
        {
            Shoot();
        }
        else
        {
            audioSource.PlayOneShot(failedShootClip);
        }

        UpdateUI();
    }


    private void Shoot()
    {
        shootTimer = shootTime;

        //get random angle using current aim angle
        float randomAngle = UnityEngine.Random.Range(-currentAimAngle, currentAimAngle);
        Vector3 shootDirection = Quaternion.AngleAxis(randomAngle, Vector3.up) * (mouseAimPoint - shootPoint.transform.position).normalized;

        //shoot ray using shootmask
        Ray ray = new Ray(shootPoint.transform.position, shootDirection);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100.0f, shootMask))
        {
            //store line
            Line line = new Line();
            line.start = shootPoint.transform.position;
            line.end = hit.point;
            line.time = 0.0f;
            lines.Add(line);

            //if hit something, apply damage
            HealthScript healthScript = hit.collider.GetComponentInChildren<HealthScript>();
            if (healthScript != null)
            {
                healthScript.TakeDamage(damage, this.transform.root.gameObject);
                Destroy(Instantiate(hitEffect, hit.point,Quaternion.FromToRotation(Vector3.up, hit.normal)), 2.0f);
            }
            hit.collider.GetComponentInChildren<NodeAI.NodeAI_Senses>()?.RegisterSensoryEvent(
                                                                                            this.transform.root.gameObject, 
                                                                                            hit.collider.gameObject, 
                                                                                            20.0f, 
                                                                                            NodeAI.SensoryEvent.SenseType.SOMATIC
                                                                                            );
            
        }

        audioSource.PlayOneShot(shootClip[UnityEngine.Random.Range(0, shootClip.Count)]);

        //Trigger auditory event on all sensors in range
        foreach (NodeAI.NodeAI_Senses sensor in FindObjectsOfType<NodeAI.NodeAI_Senses>())
        {
            if (Vector3.Distance(sensor.gameObject.transform.position, shootPoint.transform.position) < sensor.maxHearingRange)
            {
                sensor.RegisterSensoryEvent(this.transform.root.gameObject, sensor.gameObject, 10.0f, NodeAI.SensoryEvent.SenseType.AURAL);
            }
        }

        _animator.SetTrigger("Shoot");
        GameObject flash = Instantiate(muzzleFlash, shootPoint.transform.position, shootPoint.transform.rotation, shootPoint);
        Destroy(flash, 2.0f);

        currentClip--;
    }

    private void TryReload()
    {
        if (currentClip < clipSize)
        {
            reloadTimer = reloadTime;
            currentClip = clipSize;

            audioSource.PlayOneShot(reloadClip);
            _animator.SetTrigger("Reload");
        }
        else
        {
            //do nothing
        }

        UpdateUI();
    }

    private void UpdateUI(){
        uiScript.ammoText.text = currentClip.ToString() + " / " + clipSize.ToString();
    }

    private void UpdateAimAngle()
    {
        if (aimAction.ReadValue<float>() > 0.0f) {
            if (!isAiming){
                isAiming = true;

                audioSource.PlayOneShot(startAimClip);
            }

            currentAimTime += Time.deltaTime;
        }
        else {
            if (isAiming){
                isAiming = false;

                audioSource.PlayOneShot(endAimClip);
            }

            currentAimTime -= Time.deltaTime;
        }

        currentAimTime = Mathf.Clamp(currentAimTime, 0.0f, aimTime);

        currentAimAngle = Mathf.Lerp(maxAimAngle, minAimAngle, currentAimTime / aimTime);
        rightAimAngle = Quaternion.AngleAxis(currentAimAngle, Vector3.up) * transform.forward;
        leftAimAngle = Quaternion.AngleAxis(-currentAimAngle, Vector3.up) * transform.forward;

        CursorScript cs = uiScript.cursorScript;
        if (cs)
        {
            cs.SetCursor(cs.aimCursor, 1.5f - (currentAimTime / aimTime) / 2.0f);
            cs.cursorImage.color = new Color(1, 1, 1, (currentAimTime / aimTime) * 0.85f + 0.15f);
        }
    }

    private void LookAtCursor()
    {
        mouseAimPoint = playerMovement.GetMouseAimPoint();
        if (Vector3.Distance(playerMovement.transform.position, mouseAimPoint) > 1.5f && !playerMovement.isRolling)
        {
            canvas.transform.LookAt(mouseAimPoint);
            //rotate canvas 90 along right axis
            canvas.transform.RotateAround(canvas.transform.position, canvas.transform.right, 90.0f);
            //transform.LookAt(mouseAimPoint);
        }
    }
}
