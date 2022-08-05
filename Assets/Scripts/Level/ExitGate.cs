using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitGate : MonoBehaviour
{
    public enum GateDestination
    {
        Next,
        //Previous,
        Town,
        //Boss,
    }

    public GateDestination destination;

    public List<LevelCondition> conditions;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool AreConditionsComplete()
    {
        foreach (LevelCondition condition in conditions)
        {
            // skip entries that are not enabled
            if (!condition.enabled || condition.gameObject.activeSelf == false){
                continue;
            }

            if (!condition.isComplete)
            {
                return false;
            }
        }
        return true;
    }

    public void TryGoToDestination()
    {
        if (!AreConditionsComplete()){
            return;
        }

        switch (destination)
        {
            case GateDestination.Next:
                MissionManager.instance?.LoadNextLesserScene();
                break;
            case GateDestination.Town:
                LevelController.LoadTown();
                break;
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.transform.root.tag == "Player") {
            TryGoToDestination();
        }
    }
}
