using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// A simple touch trigger that can be used to trigger events.
/// </summary>
public class TouchTrigger : MonoBehaviour  /// @todo Comment
{
    public string triggerName;
    bool triggered = false;
    public bool ignorePlayer = false;
    public bool mustHaveHealth = false;
    public System.Action Triggered;

    #if UNITY_EDITOR
    [ReadOnly]
    #endif
    public Collider hitCollider;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (triggered) return;

        bool hasHealth = false;
        if (other.gameObject.GetComponentInParent<Health_Base>() != null || other.gameObject.GetComponentInChildren<Health_Base>() != null)
        {
            hasHealth = true;
        }

        bool isPlayer = false;
        if (other.GetComponentInChildren<PlayerMovement>() != null || other.GetComponentInParent<PlayerMovement>() != null)
        {
            isPlayer = true;
        }

        if ((mustHaveHealth && !hasHealth && !isPlayer) ||
            (ignorePlayer && isPlayer))
        {
            return;
        }

        if(triggered)
        {
            return;
        }
        if(other.transform.root.GetComponentInChildren<NodeAI.NodeAI_Agent>() != null)
        {
            other.transform.root.GetComponentInChildren<NodeAI.NodeAI_Agent>().SetParameter<bool>(triggerName, true);
        }
        triggered = true;
        hitCollider = other;
        Triggered.Invoke();
    }
}
