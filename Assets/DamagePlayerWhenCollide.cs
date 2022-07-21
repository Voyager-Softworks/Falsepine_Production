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
    public float tickRate = 0.5f; ///< The amount of time between each damage tick.
    float tickTimer = 0f;
    SphereCollider sphereCollider;

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
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, sphereCollider.radius, Vector3.down, 0.1f);
            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.gameObject.tag == "Player")
                {
                    hit.collider.gameObject.GetComponent<PlayerHealth>().TakeDamage(damage);
                    if (destroyOnCollision)
                    {
                        Destroy(gameObject);
                    }
                }
            }
        }
        

    }

    


}
