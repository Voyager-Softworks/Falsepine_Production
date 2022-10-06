using System.Collections;
using System.Collections.Generic;
using NodeAI;
using UnityEngine;
using UnityEngine.Events;

public class BossTriggerVolume : MonoBehaviour
{
    public NodeAI_Agent agent;
    public string parameterName;
    public UnityEvent onTrigger;
    AudioControllerPersistance audioControllerPersistance;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            audioControllerPersistance = FindObjectOfType<AudioControllerPersistance>();
            if (audioControllerPersistance != null)
            {
                audioControllerPersistance.StopBecauseOfBoss();
            }

            agent.SetParameter<bool>(parameterName, true);
            GameObject.FindObjectOfType<UIScript>().bossUI.SetActive(true);
            onTrigger.Invoke();
            Destroy(gameObject);
        }
    }
}
