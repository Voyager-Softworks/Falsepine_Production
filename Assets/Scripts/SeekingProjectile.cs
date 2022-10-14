using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Projectile which seeks the player
/// </summary>  
public class SeekingProjectile : MonoBehaviour
{
    public float speed; // Speed of the projectile
    public float lifeTime; // How long the projectile will live
    public float rotationSpeed; // How fast the projectile will rotate
    public float startDelay; // How long the projectile will wait before moving
    GameObject target; // The target to seek
    bool expired = false; // Whether the projectile has expired

    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player"); // Find the player
    }

    // Update is called once per frame
    void Update()
    {
        if (startDelay > 0.0f) // If the projectile is still waiting
        {
            startDelay -= Time.deltaTime; // Decrement the start delay
            return; // Return
        }
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
