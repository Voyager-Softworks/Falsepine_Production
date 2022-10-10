using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrimaevalSpellManager : MonoBehaviour
{
    public float maxSpawnRadius = 10.0f;
    public float playerSpawnRadius = 2.0f;
    Transform player;
    Vector3 playerPos { get { return new Vector3(player.position.x, transform.position.y, player.position.z); } }
    public Transform boss;

    public List<GameObject> shadowEnemies;
    public int shadowSpawnCount = 4;
    public GameObject umbralSerpentsPrefab;
    public int umbralSerpentsSpawnCount = 2;
    public GameObject vileBarrierPrefab;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, maxSpawnRadius);
        if (player == null) player = GameObject.FindGameObjectWithTag("Player").transform;
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(playerPos, playerSpawnRadius);
    }

    public void SpawnShadowEnemies()
    {
        if (player == null) player = GameObject.FindGameObjectWithTag("Player").transform;
        for (int i = 0; i < shadowSpawnCount; i++)
        {
            Vector3 spawnPos = Random.insideUnitSphere * maxSpawnRadius;
            spawnPos.y = 0.0f;
            spawnPos += transform.position;
            if (Vector3.Distance(spawnPos, playerPos) < playerSpawnRadius)
            {
                i--;
                continue;
            }
            GameObject shadowEnemy = Instantiate(shadowEnemies[Random.Range(0, shadowEnemies.Count)], spawnPos, Quaternion.identity);
        }
    }

    public void SpawnUmbralSerpents()
    {
        if (player == null) player = GameObject.FindGameObjectWithTag("Player").transform;
        for (int i = 0; i < umbralSerpentsSpawnCount; i++)
        {
            Vector3 spawnPos = Random.insideUnitSphere * maxSpawnRadius;
            spawnPos.y = 0.0f;
            spawnPos += transform.position;
            if (Vector3.Distance(spawnPos, playerPos) < playerSpawnRadius)
            {
                i--;
                continue;
            }
            GameObject umbralSerpents = Instantiate(umbralSerpentsPrefab, spawnPos, Quaternion.identity);
        }
    }

    public void SpawnVileBarrier()
    {
        if (player == null) player = GameObject.FindGameObjectWithTag("Player").transform;
        Vector3 spawnDir = (playerPos - boss.position).normalized;
        Vector3 spawnPos = boss.position;
        GameObject vileBarrier = Instantiate(vileBarrierPrefab, spawnPos, Quaternion.LookRotation(spawnDir));
    }
}
