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
    public float maxAimSize = 3.0f;
    public float minAimSize = 1.0f;
    public float currentAimSize = 1.0f;
    public float aimTime = 1.0f;
    float currentAimTime = 0.0f;
    private Vector3 mouseAimPoint = Vector3.zero;


    [Header("Reload")]
    public InputAction reloadAction;
    public float reloadTime = 1.0f;

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(mouseAimPoint, currentAimSize);
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

        currentAimSize = Mathf.Lerp(maxAimSize, minAimSize, currentAimTime/aimTime);
        currentAimSize += Vector3.Distance(transform.position, mouseAimPoint)/20.0f;
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
