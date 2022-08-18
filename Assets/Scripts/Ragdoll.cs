using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///  Script to handle ragdolling models.
/// </summary>
public class Ragdoll : MonoBehaviour
{
    /// <summary>
    ///  Enable the ragdoll.
    /// </summary>
    public void EnableRagdoll()
    {
        var animator = GetComponentInChildren<Animator>();
        if (animator)
        {
            animator.enabled = false;
        }
        var colliders = GetComponents<Collider>();
        foreach (var collider in colliders)
        {
            collider.enabled = false;
        }
        var rigidbodies = GetComponentsInChildren<Rigidbody>();
        foreach (var rb in rigidbodies)
        {
            rb.isKinematic = false;
        }
    }

    /// <summary>
    /// "When the player dies, enable the ragdoll and add a force to the rigidbodies in the direction of
    /// the hit point."
    /// </summary>
    /// <param name="context">The context of the death.</param>
    public void RagdollOnDeath(Health_Base.DeathContext context)
    {
        EnableRagdoll();
        // Add force to rigidbody in the direction of the hit point.
        var rigidbodies = GetComponentsInChildren<Rigidbody>();
        foreach (var rb in rigidbodies)
        {
            rb.velocity = context.Direction.normalized * 4.0f;
        }
    }

    /// <summary>
    /// Disable the ragdoll.
    /// </summary>
    public void DisableRagdoll()
    {
        var animator = GetComponentInChildren<Animator>();
        if (animator)
        {
            animator.enabled = true;
        }
        var rigidbodies = GetComponentsInChildren<Rigidbody>();
        foreach (var rb in rigidbodies)
        {
            rb.isKinematic = true;
        }
    }

    void Start()
    {
        DisableRagdoll();
        GetComponent<EnemyHealth>().Death += RagdollOnDeath;

    }
}
