using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class GunScript : MonoBehaviour
{
    PlayerMovement playerMovement;
    UIScript uiScript;

    [Header("Shoot")]
    public InputAction shootAction;
    public float damage = 10.0f;
    public int clipSize = 10;
    public int currentClip = 10;


    [Header("Aim")]
    public InputAction aimAction;
    public float maxAimAngle = 25.0f;
    public float minAimAngle = 5.0f;
    public float currentAimAngle = 5.0f;
    public float aimTime = 1.0f;
    float currentAimTime = 0.0f;
    private Vector3 mouseAimPoint = Vector3.zero;


    [Header("Reload")]
    public InputAction reloadAction;
    public float reloadTime = 1.0f;

    private void OnDrawGizmos() {
        //draw aim point
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(mouseAimPoint, 0.1f);

        //draw aim vec
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, mouseAimPoint);

        //draw current aim angle
        Gizmos.color = Color.blue;
        Vector3 rightAngle = Quaternion.AngleAxis(currentAimAngle, Vector3.up) * transform.forward;
        Gizmos.DrawLine(transform.position, transform.position + rightAngle * 5.0f);
        Gizmos.color = Color.yellow;
        Vector3 leftAngle = Quaternion.AngleAxis(-currentAimAngle, Vector3.up) * transform.forward;
        Gizmos.DrawLine(transform.position, transform.position + leftAngle * 5.0f);

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
        if (playerMovement == null || uiScript == null) {
            Debug.LogError("GunScript: Missing player movement or ui script");
            return;
        }

        LookAtCursor();

        if (aimAction.ReadValue<float>() > 0.0f) currentAimTime += Time.deltaTime;
        else currentAimTime -= Time.deltaTime;
        currentAimTime = Mathf.Clamp(currentAimTime, 0.0f, aimTime);

        currentAimAngle = Mathf.Lerp(maxAimAngle, minAimAngle, currentAimTime/aimTime);
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
