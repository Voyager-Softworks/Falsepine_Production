using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>  @todo comment
public class SeekingProjectile : MonoBehaviour
{
    public float speed;
    public float lifeTime;
    public float rotationSpeed;
    GameObject target;
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
            transform.position += transform.forward * speed * Time.deltaTime;
            return;
        }
        if (target != null)
        {
            Vector3 desired = target.transform.position - transform.position;
            desired.Normalize();
            desired *= speed;
            transform.position += transform.forward * speed * Time.deltaTime;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(desired), rotationSpeed * Time.deltaTime);
            lifeTime -= Time.deltaTime;
        }


    }
}
