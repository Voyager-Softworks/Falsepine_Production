using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 0.125f;
    public Vector3 offset;

    // Start is called before the first frame update
    void Start()
    {
        offset = transform.position - target.position;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 currentDifference = target.position - (transform.forward + offset);
        float dist = currentDifference.magnitude;

        float speedMulti = 1.0f;
        speedMulti = Mathf.Lerp(speedMulti, 2.0f, dist / 10.0f);

        transform.position = Vector3.Lerp(transform.position, target.position + offset, Time.deltaTime * smoothSpeed * speedMulti);
    }
}
