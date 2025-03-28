using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;

/// <summary>
/// Manages the health of props.
/// </summary>
public class PropHealth : Health_Base
{
    public GameObject m_brokenPrefab = null;

    public int m_silverReward = 1;

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

        // get econ manager, and give money
        EconomyManager.instance.AddMoney(m_silverReward);
        // notify
        if (m_silverReward > 0) NotificationManager.instance?.AddIcon("silver", transform.position + Vector3.up * 2f);

        // spawn broken prefab
        if (m_brokenPrefab != null)
        {
            // disable renderer, collider
            GetComponent<Renderer>().enabled = false;
            GetComponent<Collider>().enabled = false;
            Destroy(gameObject, 5.0f);

            GameObject broken = Instantiate(m_brokenPrefab, transform.position, transform.rotation);
            broken.transform.parent = transform.parent;
            //scale match
            broken.transform.localScale = transform.localScale;

            // make parts fly away
            Rigidbody[] rigidbodies = broken.GetComponentsInChildren<Rigidbody>();
            foreach (Rigidbody rigidbody in rigidbodies)
            {
                DamageStat lastDamage = m_damageHistory[m_damageHistory.Count - 1];
                rigidbody.AddForce(lastDamage.direction * 0.1f, ForceMode.Impulse);
            }

            // disable player collision with parts
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
