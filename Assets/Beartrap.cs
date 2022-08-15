using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Beartrap : MonoBehaviour
{
    public float m_damage = 5.0f;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<TouchTrigger>().Triggered += CloseBeartrap;
    }

    void CloseBeartrap()
    {
        gameObject.transform.root.gameObject.GetComponent<Animator>().SetTrigger("Close");
        gameObject.transform.root.gameObject.GetComponentsInChildren<Collider>().All(collider => collider.enabled = false);

        Collider hitCollider = GetComponent<TouchTrigger>().hitCollider;
        if (hitCollider != null)
        {
            // try deal damage to the object
            Health_Base health = hitCollider.GetComponentInParent<Health_Base>();
            if (health == null) health = hitCollider.GetComponentInChildren<Health_Base>();
            if (health != null)
            {
                health.TakeDamage(new Health_Base.DamageStat(m_damage, gameObject, transform.position, hitCollider.transform.position));
            }
        }
    }
}
