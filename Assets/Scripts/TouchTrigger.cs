using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TouchTrigger : MonoBehaviour  /// @todo Comment
{
    public string triggerName;
    public UnityEvent onTrigger;
    bool triggered = false;

    public bool ignorePlayer = false;
    public bool alwaysCheck = false;
    public string targetTag = "";

    Collider[] hits = new Collider[10];

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(alwaysCheck && !triggered)
        {
            if(Physics.OverlapSphereNonAlloc(transform.position, 1f, hits) > 0)
            {
                if(hits.Length == 0 || hits[0] == null) return;
                for(int i = 0; i < hits.Length; i++)
                {
                    if(hits[i].gameObject.tag == targetTag)
                    {
                        if(!triggered)
                        {
                            triggered = true;
                            onTrigger.Invoke();
                        }
                    }
                }
            }
        }
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
        onTrigger.Invoke();
        
    }
}
