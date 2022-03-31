using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSecondState : MonoBehaviour
{
    public void OnSecondStateEnter()
    {
        GetComponent<NodeAI.NodeAI_Agent>().SetBool("SecondPhase", true);
        Debug.Log("Second Phase");
    }
}
