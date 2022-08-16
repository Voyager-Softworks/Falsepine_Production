using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;

/// <summary>
/// Manages the health of enemies.
/// </summary>
public class PropHealth : Health_Base  /// @todo comment
{
    public GameObject m_brokenPrefab = null;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
    }

    public override void TakeDamage(Health_Base.DamageStat _damage)
    {
        base.TakeDamage(_damage);
    }

    public override void Die()
    {
        base.Die();

        if (m_brokenPrefab != null)
        {
            Destroy(gameObject);

            GameObject broken = Instantiate(m_brokenPrefab, transform.position, transform.rotation);
            broken.transform.parent = transform.parent;
            //scale
            broken.transform.localScale = transform.localScale;

            Rigidbody[] rigidbodies = broken.GetComponentsInChildren<Rigidbody>();
            foreach (Rigidbody rigidbody in rigidbodies)
            {
                DamageStat lastDamage = m_damageHistory[m_damageHistory.Count - 1];
                rigidbody.AddForce(lastDamage.direction * 0.1f, ForceMode.Impulse);
            }

            if (m_disablePlayerCollision)
            {
                Collider[] colliders = broken.GetComponentsInChildren<Collider>();
                foreach (Collider collider in colliders)
                {
                    collider.gameObject.layer = LayerMask.NameToLayer("NOPlayerCollide");
                }
            }
        }
    }
}
