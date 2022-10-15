using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceFinalZone : MonoBehaviour
{
    private bool done = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other) {
        if (done) return;

        PlayerMovement pm = other.GetComponentInParent<PlayerMovement>() ?? other.GetComponentInChildren<PlayerMovement>();
        if (pm != null) {
            MissionManager mm = MissionManager.instance;
            if (mm == null)
            {
                Debug.LogError("ForceFinalZone: MissionManager not found");
                return;
            }

            // while this is not the final zone, go to the next one
            int maxTries = 100;
            while (mm.GetZoneIndex(mm.GetCurrentZone()) < mm.m_missionZones.Count - 1)
            {
                mm.GoToNextZone();

                maxTries--;
                if (maxTries <= 0)
                {
                    Debug.LogError("ForceFinalZone: Too many tries");
                    return;
                }
            }

            done = true;
        }
    }
}
