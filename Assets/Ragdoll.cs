using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ragdoll : MonoBehaviour
{
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
