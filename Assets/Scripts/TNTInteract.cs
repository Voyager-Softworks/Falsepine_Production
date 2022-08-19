using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Tnt interactable for the extermination mission.
/// </summary>
public class TNTInteract : Interactable
{
    public GameObject explosion = null;
    public GameObject enemies = null;

    // Start is called before the first frame update
    override public void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    override public void Update()
    {
        base.Update();
    }

    /// <summary>
    /// Alerts the enemies in scene.
    /// </summary>
    private void AlertEnemies()
    {
        // get player gameobject
        GameObject player = FindObjectOfType<PlayerHealth>()?.transform.root.gameObject;

        if (player != null)
        {
            // send somatic event to all enemies
            NodeAI.NodeAI_Senses[] senses = enemies.GetComponentsInChildren<NodeAI.NodeAI_Senses>();
            foreach (NodeAI.NodeAI_Senses sense in senses)
            {
                sense.RegisterSensoryEvent(player, sense.gameObject, 20.0f, NodeAI.SensoryEvent.SenseType.SOMATIC);
            }
        }
    }

    override public void DoInteract()
    {
        base.DoInteract();

        if (explosion != null)
        {
            // enable
            explosion.SetActive(true);
        }

        if (enemies != null)
        {
            // enable
            enemies.SetActive(true);

            // alert enemies after 0.5 seconds
            Invoke("AlertEnemies", 0.5f);
        }

        // set condition to be complete
        Manual_Condition mc = GetComponent<Manual_Condition>();
        if (mc != null)
        {
            mc.Complete();
        }
    }
}