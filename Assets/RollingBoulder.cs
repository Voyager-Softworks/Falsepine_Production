using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollingBoulder : MonoBehaviour
{
    [SerializeField] Vector3 rollDirection;
    [SerializeField] float rollSpeed;
    [SerializeField] float rotationMultiplier;
    // Start is called before the first frame update
    void Start()
    {

    }

    public void Roll(Vector3 direction)
    {
        rollDirection = direction.normalized;
        rollDirection.y = 0;
        rollDirection *= 10.0f;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 rollEuler = new Vector3(rollDirection.z * rollSpeed, rollDirection.y * rollSpeed, -rollDirection.x * rollSpeed);
        // Rotate the boulder
        transform.Rotate(rollEuler * rollSpeed * rotationMultiplier * Time.deltaTime, Space.World);


        // Move the boulder
        transform.Translate(rollDirection * rollSpeed * Time.deltaTime, Space.World);
    }
}
