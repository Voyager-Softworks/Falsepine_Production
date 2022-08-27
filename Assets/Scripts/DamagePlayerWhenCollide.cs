using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///  Script that damages the player when they intersect with a collider.
/// </summary>
public class DamagePlayerWhenCollide : MonoBehaviour
{
    public float damage = 10f; ///< Damage done by the attack
    public bool destroyOnCollision = true; ///< Whether or not the object should be destroyed after it collides with the player.
    public bool destroyOnGround = true; ///< Whether or not the object should be destroyed after it collides with the ground.
    public bool destroyOnWall = true; ///< Whether or not the object should be destroyed after it collides with a wall.
    public float tickRate = 0.5f; ///< The amount of time between each damage tick.
    float tickTimer = 0f;
    SphereCollider sphereCollider;
    public bool setInactiveOnCollision = false; ///< Whether or not the object should be set inactive after it collides with the player.
    public bool isActive = true;
    public GameObject destroyParticle;
    public float timeBeforeDetection = 0.5f;
    float timeBeforeDetectionTimer = 0f;

    public float radius = 0.5f; ///< The radius of the sphere.

    // Start is called before the first frame update
    void Start()
    {
        sphereCollider = GetComponent<SphereCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        tickTimer += Time.deltaTime;
        if (tickTimer >= tickRate)
        {
            tickTimer = 0f;
            if (!isActive) return;
            timeBeforeDetectionTimer += Time.deltaTime;
            if (timeBeforeDetectionTimer < timeBeforeDetection) return;
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, sphereCollider.radius * transform.localScale.magnitude, Vector3.down, 0.0f);
            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.gameObject.tag == "Player")
                {
                    hit.collider.gameObject.GetComponent<PlayerHealth>().TakeDamage(damage);
                    if (destroyOnCollision)
                    {
                        Destroy(gameObject);
                        if (destroyParticle != null)
                        {
                            Instantiate(destroyParticle, transform.position, Quaternion.LookRotation(hit.normal));
                        }
                    }
                    else if (setInactiveOnCollision)
                    {
                        isActive = false;
                        timeBeforeDetectionTimer = 0f;
                    }
                }
                else if (destroyOnGround)
                {
                    Destroy(gameObject);
                    if (destroyParticle != null)
                    {
                        Instantiate(destroyParticle, transform.position, Quaternion.identity);
                    }
                }
                else if (destroyOnWall)
                {
                    //Check the normal of the hit to see if it's a wall
                    if (hit.normal.y < 0.5f)
                    {
                        Destroy(gameObject);
                        if (destroyParticle != null)
                        {
                            Instantiate(destroyParticle, transform.position, Quaternion.LookRotation(hit.normal));
                        }
                    }
                }
            }
        }


    }




}
