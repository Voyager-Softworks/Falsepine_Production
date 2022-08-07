using System.Collections;
using System.Collections.Generic;
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
    public struct GroupMember
    {
        public GameObject enemyPrefab;
        public int count;
    }
    public List<GroupMember> groupMembers = new List<GroupMember>();

    public float spawnDelay = 1.0f;

    public float spawnDelayJitter = 0.5f;

    public float radius = 10.0f;
    
    List<GameObject> spawnedEnemies = new List<GameObject>();

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
    }

    public void SpawnEnemies()
    {
        StartCoroutine(SpawnEnemiesCoroutine());
    }

    public void DestroyEnemies()
    {
        foreach (GameObject enemy in spawnedEnemies)
        {
            Destroy(enemy);
        }
        spawnedEnemies.Clear();
    }

    public bool AreEnemiesAlive()
    {
        //Remove all dead enemies from the list
        spawnedEnemies.RemoveAll(enemy => enemy.GetComponent<EnemyHealth>().hasDied);
        return spawnedEnemies.Count > 0;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
