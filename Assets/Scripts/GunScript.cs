using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using System;

public class GunScript : MonoBehaviour
{
    PlayerMovement playerMovement;
    UIScript uiScript;

    [Header("Shoot")]
    public InputAction shootAction;
    public float damage = 10.0f;
    public int clipSize = 10;
    public int currentClip = 10;
    public LayerMask shootMask;


    [Header("Aim")]
    public InputAction aimAction;
    public float maxAimAngle = 25.0f;
    public float minAimAngle = 5.0f;
    public float currentAimAngle = 5.0f;
    public float aimTime = 1.0f;
    float currentAimTime = 0.0f;
    private Vector3 mouseAimPoint = Vector3.zero;

    private Vector3 rightAimAngle = Vector3.zero;
    private Vector3 leftAimAngle = Vector3.zero;


    [Header("Reload")]
    public InputAction reloadAction;
    public float reloadTime = 1.0f;

    private void OnDrawGizmos() {
        //draw current aim angle
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + rightAimAngle * 5.0f);
        Gizmos.DrawLine(transform.position, transform.position + leftAimAngle * 5.0f);
    }


    // Start is called before the first frame update
    void Start()
    {
        playerMovement = FindObjectOfType<PlayerMovement>();
        uiScript = FindObjectOfType<UIScript>();

        shootAction.Enable();
        aimAction.Enable();
        reloadAction.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerMovement == null || uiScript == null)
        {
            Debug.LogError("GunScript: Missing player movement or ui script");
            return;
        }

        LookAtCursor();

        UpdateAimAngle();

        if (shootAction.triggered)
        {
            TryShoot();
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
            Reload();
        }

        UpdateUI();
    }

    private void Shoot()
    {
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

        currentClip--;
    }

    private void Reload()
    {
        currentClip = clipSize;
    }

    private void UpdateUI(){
        uiScript._ammoText.text = currentClip.ToString() + " / " + clipSize.ToString();
    }

    private void UpdateAimAngle()
    {
        if (aimAction.ReadValue<float>() > 0.0f) currentAimTime += Time.deltaTime;
        else currentAimTime -= Time.deltaTime;
        currentAimTime = Mathf.Clamp(currentAimTime, 0.0f, aimTime);

        currentAimAngle = Mathf.Lerp(maxAimAngle, minAimAngle, currentAimTime / aimTime);
        rightAimAngle = Quaternion.AngleAxis(currentAimAngle, Vector3.up) * transform.forward;
        leftAimAngle = Quaternion.AngleAxis(-currentAimAngle, Vector3.up) * transform.forward;

        CursorScript cs = uiScript._cursorScript;
        if (cs)
        {
            cs.SetCursor(cs.aimCursor, 1.5f - (currentAimTime / aimTime) / 2.0f);
            cs.cursorImage.color = new Color(1, 1, 1, (currentAimTime / aimTime) * 0.85f + 0.15f);
        }
    }

    private void LookAtCursor()
    {
        mouseAimPoint = playerMovement.GetMouseAimPoint();
        if (Vector3.Distance(playerMovement.transform.position, mouseAimPoint) > 1.5f)
        {
            transform.LookAt(mouseAimPoint);
        }
    }
}
