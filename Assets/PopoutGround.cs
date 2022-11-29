using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopoutGround : MonoBehaviour
{
    public float asleepHeight = -10.0f;
    private Vector3 startPos;
    public float awakeTime = 1.0f;

    public float triggerDistance = 9.0f;

    [ReadOnly] public Transform player;
    [ReadOnly] public bool playerInRange = false;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerMovement>().transform;

        startPos = transform.position;

        transform.position = startPos + Vector3.up * asleepHeight;
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null)
        {
            return;
        }

        if (Vector3.Distance(startPos, player.position) < triggerDistance)
        {
            playerInRange = true;
        }
        else
        {
            playerInRange = false;
        }

        if (playerInRange)
        {
            transform.position = Vector3.Lerp(transform.position, startPos + Vector3.up,  Time.deltaTime * (1.0f / awakeTime));
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, startPos + Vector3.up * asleepHeight, Time.deltaTime * (1.0f / awakeTime));
        }
    }
}
