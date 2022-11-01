using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopCombatMusic : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (FindObjectOfType<AudioControllerPersistance>())
        {
            FindObjectOfType<AudioControllerPersistance>().StopBecauseOfBoss();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
