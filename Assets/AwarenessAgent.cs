using System.Collections;
using System.Collections.Generic;
using NodeAI;
using UnityEngine;

/// <summary>
/// Translation layer between NodeAI senses and the Player Awareness Management system.
/// </summary>
public class AwarenessAgent : MonoBehaviour
{
    public string awarenessParameterName = "Aware";
    public string fromOtherEnemyParameterName = "FromOtherEnemy";

    bool isAware = false;
    // Start is called before the first frame update
    void Start()
    {
        FindObjectOfType<EnemyAwarenessManager>().onEnemyAware += OnEnemyAware;
        GetComponent<NodeAI_Senses>().OnSensoryEvent += SensoryInput;
    }

    void SensoryInput(SensoryEvent e)
    {
        if (isAware) return;
        if (e.source.gameObject.transform.root.gameObject.tag == "Player")
        {
            OnEnemyAware(new EnemyAwarenessManager.Context()
            {
                enemy = gameObject,
                fromOtherEnemy = false
            });
        }
    }

    void OnEnemyAware(EnemyAwarenessManager.Context context)
    {
        if (isAware) return;
        isAware = true;
        if (context.fromOtherEnemy)
        {
            GetComponent<NodeAI_Agent>().SetParameter(fromOtherEnemyParameterName, true);
            GetComponent<NodeAI_Agent>().SetParameter(awarenessParameterName, true);
        }
        else
        {
            GetComponent<NodeAI_Agent>().SetParameter(awarenessParameterName, true);
            GetComponent<NodeAI_Agent>().SetParameter(fromOtherEnemyParameterName, false);
            FindObjectOfType<EnemyAwarenessManager>().RegisterAwareness(gameObject);
        }

    }
}
