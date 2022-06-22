using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagePlayerWhenCollide : MonoBehaviour
{
    public float damage = 10f;
    public bool destroyOnCollision = true;
    public float tickRate = 0.5f;
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
