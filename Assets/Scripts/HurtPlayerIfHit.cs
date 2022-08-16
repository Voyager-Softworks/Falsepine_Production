using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Hurts the player if they interact with this collider.
/// </summary>
public class HurtPlayerIfHit : MonoBehaviour
{
    public GameObject particle;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<PlayerHealth>().TakeDamage(5);
        }
        Instantiate(particle, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
