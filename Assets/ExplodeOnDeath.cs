using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodeOnDeath : MonoBehaviour
{
    public GameObject m_explosion;
    public float m_damage = 10f;
    public float m_radius = 5f;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<EnemyHealth>().Death += Explode;
    }

    void Explode(Health_Base.DeathContext context)
    {
        var explosion = Instantiate(m_explosion, transform.position, Quaternion.identity);
        Destroy(explosion, 5f);
        Collider[] colliders = Physics.OverlapSphere(transform.position, m_radius);
        foreach (Collider collider in colliders)
        {
            if (collider.gameObject.GetComponent<EnemyHealth>() != null)
            {
                collider.gameObject.GetComponent<EnemyHealth>().TakeDamage(new Health_Base.DamageStat(damage: m_damage, sourceObject: gameObject, origin: transform.position, hitPoint: collider.transform.position));
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
