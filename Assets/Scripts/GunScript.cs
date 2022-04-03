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

    [Header("Shoot")]
    public InputAction shootAction;
    public AudioClip shootClip;
    public AudioClip failedShootClip;
    public float damage = 10.0f;
    public int clipSize = 10;
    public int currentClip = 10;
    public float shootTime = 0.1f;
    public float shootTimer = 0.0f;
    public LayerMask shootMask;


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
    public Image leftAimLineImage;
    public Image rightAimLineImage;


    [Header("Reload")]
    public InputAction reloadAction;
    public AudioClip reloadClip;
    public float reloadTime = 1.0f;
    private float reloadTimer = 0.0f;

    private void OnDrawGizmos() {
        //draw current aim angle
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + rightAimAngle * 5.0f);
        Gizmos.DrawLine(transform.position, transform.position + leftAimAngle * 5.0f);
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

        float opacity = 1.0f;
        if (reloadTimer <= 0.0f && shootTimer <= 0.0f){
            opacity = (currentAimTime / aimTime) * 0.5f;
        }
        else{
            opacity = 0f;
        }

        leftAimLineImage.color = new Color(1.0f, 1.0f, 1.0f, opacity);
        rightAimLineImage.color = new Color(1.0f, 1.0f, 1.0f, opacity);

        float length = Vector3.Distance(transform.position, mouseAimPoint);

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
            transform.LookAt(playerMovement.transform.position + playerMovement.transform.forward*2.0f - playerMovement.transform.up);
        }
        else if (shootTimer > 0.0f)
        {
            shootTimer -= Time.deltaTime;
            transform.LookAt(playerMovement.transform.position + playerMovement.transform.forward*2.0f + playerMovement.transform.up);
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
        Vector3 shootDirection = Quaternion.AngleAxis(randomAngle, Vector3.up) * transform.forward;

        //shoot ray using shootmask
        Ray ray = new Ray(transform.position, shootDirection);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100.0f, shootMask))
        {
            //if hit something, apply damage
            HealthScript healthScript = hit.collider.GetComponent<HealthScript>();
            if (healthScript != null)
            {
                healthScript.TakeDamage(damage);
            }
        }

        audioSource.PlayOneShot(shootClip);

        currentClip--;
    }

    private void TryReload()
    {
        if (currentClip < clipSize)
        {
            reloadTimer = reloadTime;
            currentClip = clipSize;

            audioSource.PlayOneShot(reloadClip);
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
            transform.LookAt(mouseAimPoint);
        }
    }
}
