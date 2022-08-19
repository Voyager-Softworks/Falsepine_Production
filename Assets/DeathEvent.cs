using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
///  Bandaid solution for making the animator play the death of the thing.
/// </summary>
/// @todo Get rid of this at one point because its super stinky
public class DeathEvent : MonoBehaviour
{
    public UnityEvent onDeath;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Health_Base>().Death += (ctx) => onDeath.Invoke();
    }

    // Update is called once per frame
    void Update()
    {

    }


}
