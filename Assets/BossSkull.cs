using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Enables/disables the tutorial skull if the next scene is the boss scene
/// </summary>
public class BossSkull : MonoBehaviour
{
    public GameObject skull;

    // Start is called before the first frame update
    void Start()
    {
        if (MissionManager.instance != null && MissionManager.instance.GetCurrentZone() != null){
            if (MissionManager.instance.GetCurrentZone().GetNextScenePath() == MissionManager.instance.GetCurrentZone().m_bossScene){
                skull.SetActive(true);
            }
            else{
                skull.SetActive(false);
            }
        }
        else {
            skull.SetActive(false);
        }
    }
}
