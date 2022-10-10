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

    public float awarenessRadius = 10.0f;

    bool isAware = false;
    // Start is called before the first frame update
    void Start()
    {
        FindObjectOfType<EnemyAwarenessManager>().onEnemyAware += OnEnemyAware;
        if (GetComponent<NodeAI_Senses>()) GetComponent<NodeAI_Senses>().OnSensoryEvent += SensoryInput;
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

        if (context.fromOtherEnemy)
        {
            if (Vector3.Distance(transform.position, context.enemy.transform.position) < awarenessRadius)
            {
                GetComponent<NodeAI_Agent>().SetParameter(fromOtherEnemyParameterName, true);
                GetComponent<NodeAI_Agent>().SetParameter(awarenessParameterName, true);
                isAware = true;
            }

        }
        else
        {
            isAware = true;
            GetComponent<NodeAI_Agent>().SetParameter(awarenessParameterName, true);
            GetComponent<NodeAI_Agent>().SetParameter(fromOtherEnemyParameterName, false);
            FindObjectOfType<EnemyAwarenessManager>().RegisterAwareness(gameObject);
        }

    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, awarenessRadius);
        Gizmos.color = Color.white;
    }
}
