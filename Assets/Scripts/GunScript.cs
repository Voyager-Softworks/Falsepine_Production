using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunScript : MonoBehaviour
{
    PlayerMovement playerMovement;
    private Vector3 mouseAimPoint = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        playerMovement = FindObjectOfType<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerMovement == null) return;

        mouseAimPoint = playerMovement.GetMouseAimPoint();

        if (Vector3.Distance(playerMovement.transform.position, mouseAimPoint) > 1.5f)
        {
            transform.LookAt(mouseAimPoint);
        }
        
    }
}
