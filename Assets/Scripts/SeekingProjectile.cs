using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeekingProjectile : MonoBehaviour
{
    public float speed;
    public float lifeTime;
    public float rotationSpeed;
    GameObject target;
    Vector3 velocity;
    Vector3 acceleration;
    bool expired = false;

    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (lifeTime <= 0)
        {
            if (!expired)
            {
                expired = true;
                Destroy(gameObject, 20.0f);
            }
            transform.position += velocity * Time.deltaTime;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(velocity), rotationSpeed * Time.deltaTime);
            return;
        }
        acceleration = Vector3.zero;
        if (target != null)
        {
            Vector3 desired = target.transform.position - transform.position;
            desired.Normalize();
            desired *= speed;
            acceleration = desired - velocity;
        }
        velocity += acceleration * Time.deltaTime;
        transform.position += velocity * Time.deltaTime;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(velocity), rotationSpeed * Time.deltaTime);
        lifeTime -= Time.deltaTime;

    }
}
