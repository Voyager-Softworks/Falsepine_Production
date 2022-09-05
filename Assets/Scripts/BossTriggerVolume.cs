using System.Collections;
using System.Collections.Generic;
using NodeAI;
using UnityEngine;

public class BossTriggerVolume : MonoBehaviour
{
    public NodeAI_Agent agent;
    public string parameterName;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            agent.SetParameter<bool>(parameterName, true);
            GameObject.FindObjectOfType<UIScript>().bossUI.SetActive(true);
            Destroy(gameObject);
        }
    }
}
