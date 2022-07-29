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
        var animator = GetComponent<Animator>();
        if(animator)
        {
            animator.enabled = false;
        }
        var rigidbodies = GetComponentsInChildren<Rigidbody>();
        foreach(var rb in rigidbodies)
        {
            rb.isKinematic = false;
        }
    }

    /// <summary>
    /// Disable the ragdoll.
    /// </summary>
    public void DisableRagdoll()
    {
        var animator = GetComponent<Animator>();
        if(animator)
        {
            animator.enabled = true;
        }
        var rigidbodies = GetComponentsInChildren<Rigidbody>();
        foreach(var rb in rigidbodies)
        {
            rb.isKinematic = true;
        }
    }

    void Start()
    {
        DisableRagdoll();
    }
}
