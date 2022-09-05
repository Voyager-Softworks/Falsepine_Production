using System.Collections;
using System.Collections.Generic;
using NodeAI;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Manages the spawning of groups of enemies.
/// </summary>
/// <remarks>
///  This script was made primarily for use in boss fights when the boss summons groups of enemies.
/// </remarks>
public class EnemyGroup : MonoBehaviour
{
    [System.Serializable]
    /// <summary>
    ///  A class representing one group of enemies.
    /// </summary>
    public struct GroupMember
    {
        public GameObject enemyPrefab; ///< The enemy prefab to spawn.
        public int count; ///< The number of enemies to spawn.
    }
    public List<GroupMember> groupMembers = new List<GroupMember>(); ///< The group members.

    public float spawnDelay = 1.0f; ///< The delay between each group member being spawned.

    public float spawnDelayJitter = 0.5f; ///< The amount of jitter to add to the spawn delay.

    public float radius = 10.0f; ///< The radius of the area in which to spawn the group.

    List<GameObject> spawnedEnemies = new List<GameObject>(); ///< The spawned enemies.

    /// <summary>
    ///  Spawns the group of enemies.
    /// </summary>
    /// <returns> The coroutine that is running the spawning of the group. </returns>
    IEnumerator SpawnEnemiesCoroutine()
    {
        foreach (GroupMember groupMember in groupMembers)
        {
            for (int i = 0; i < groupMember.count; i++)
            {
                yield return new WaitForSeconds(spawnDelay + Random.Range(-spawnDelayJitter, spawnDelayJitter));
                Vector3 spawnPosition = transform.position + Random.insideUnitSphere * radius;
                spawnPosition.y = transform.position.y;
                GameObject enemy = Instantiate(groupMember.enemyPrefab, spawnPosition, Quaternion.identity);
                spawnedEnemies.Add(enemy);
            }
        }
        FindObjectOfType<EnemyAwarenessManager>().RegisterAwareness(gameObject);
    }

    /// <summary>
    ///  Starts the spawning of the group.
    /// </summary>
    public void SpawnEnemies()
    {
        StartCoroutine(SpawnEnemiesCoroutine());
    }

    /// <summary>
    ///  Destroys the group of enemies.
    /// </summary>
    public void DestroyEnemies()
    {
        foreach (GameObject enemy in spawnedEnemies)
        {
            Destroy(enemy);
        }
        spawnedEnemies.Clear();
    }

    /// <summary>
    ///  Checks if the group of enemies is all dead.
    /// </summary>
    /// <returns> True if the group of enemies is all dead, false otherwise. </returns>
    public bool AreEnemiesAlive()
    {
        //Remove all dead enemies from the list
        spawnedEnemies.RemoveAll(enemy => enemy.GetComponent<EnemyHealth>().hasDied);
        return spawnedEnemies.Count > 0;
    }

    /// <summary>
    ///  Draws Gizmos for the group of enemies.
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
