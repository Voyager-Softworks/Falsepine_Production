using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TouchTrigger : MonoBehaviour
{
    public string triggerName;
    public UnityEvent onTrigger;
    bool triggered = false;
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
        if(triggered)
        {
            return;
        }
        if(other.gameObject.GetComponent<NodeAI.NodeAI_Agent>() != null)
        {
            other.gameObject.GetComponent<NodeAI.NodeAI_Agent>().SetParameter<bool>(triggerName, true);
            
            
        }
        triggered = true;
        onTrigger.Invoke();
        
    }
}
