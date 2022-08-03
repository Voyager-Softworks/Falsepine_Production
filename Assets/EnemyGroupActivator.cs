using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGroupActivator : MonoBehaviour
{
    public void SpawnEnemies()
    {
        GameObject.FindObjectOfType<EnemyGroup>().SpawnEnemies();
    }
}
