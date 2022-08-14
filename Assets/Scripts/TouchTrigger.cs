using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TouchTrigger : MonoBehaviour  /// @todo Comment
{
    public string triggerName;
    bool triggered = false;
    public bool ignorePlayer = false;
    public System.Action Triggered;

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
        if (ignorePlayer && (other.GetComponentInChildren<PlayerMovement>() != null || other.GetComponentInParent<PlayerMovement>() != null))
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
        Triggered.Invoke();
        
    }
}
