using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurtPlayerIfHit : MonoBehaviour /// @todo Comment
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
