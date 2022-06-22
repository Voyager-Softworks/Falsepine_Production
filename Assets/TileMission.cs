using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileMission : MonoBehaviour
{
    public GameObject missionGroup;
    public GameObject missionObject;

    public Mission linkedMission;

    // Start is called before the first frame update
    void Start()
    {
        if (MissionManager.instance){
            if (MissionManager.instance.currentMission.Equals(linkedMission)){
                EnableMisison();
            }
            else {
                DisableMission();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EnableMisison(){
        missionGroup.SetActive(true);
    }

    public void DisableMission(){
        missionGroup.SetActive(false);
    }
}
