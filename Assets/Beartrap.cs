using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Beartrap : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<TouchTrigger>().Triggered += CloseBeartrap;
    }

    void CloseBeartrap()
    {
        gameObject.transform.root.gameObject.GetComponent<Animator>().SetTrigger("Close");
        gameObject.transform.root.gameObject.GetComponentsInChildren<Collider>().All(collider => collider.enabled = false);
    }

    
}
