using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Boss.Primaeval
{
    /// <summary>
    ///  Manages spawning spell effects for the Primaeval boss, where they cannot be handled by ranged attack nodes.
    /// </summary>  
    /// <remarks>
    /// This is a separate class because the Primaeval boss has a lot of spells that are not handled by ranged attacks, so they must be handled by a separate class.
    /// The spells that are managed by this class include spawning shadow enemies, spawning umbral serpents, and spawning a vile barrier.
    /// </remarks>
    public class PrimaevalSpellManager : MonoBehaviour
    {
        public float maxSpawnRadius = 10.0f; //The maximum distance from the boss that enemies can spawn
        public float playerSpawnRadius = 2.0f; //The minimum distance from the player that enemies can spawn
        Transform player; //The player's transform
        Vector3 playerPos { get { return new Vector3(player.position.x, transform.position.y, player.position.z); } } //The player's position, but on the same y level as the boss
        public Transform boss; //The boss's transform

        public List<GameObject> shadowEnemies; //The list of shadow enemies that can be spawned
        public int shadowSpawnCount = 4; //The number of shadow enemies to spawn
        public GameObject umbralSerpentsPrefab; //The prefab for the umbral serpents
        public float umbralSerpentSpawnDelay = 0.5f; //The delay between spawning each umbral serpent
        public float umbralSerpentsSpawnDuration = 3.5f; //The duration of the umbral serpents' spawn
        public GameObject vileBarrierPrefab; //The prefab for the vile barrier

        public float vileBarrierDuration = 3.0f;


        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        /// <summary>
        ///  Draws the maximum spawn radius and the minimum spawn radius in the editor.
        /// </summary>
        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, maxSpawnRadius);
            if (player == null) player = GameObject.FindGameObjectWithTag("Player").transform;
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(playerPos, playerSpawnRadius);
        }

        /// <summary>
        ///  Spawns shadow enemies.
        /// </summary>
        public void SpawnShadowEnemies()
        {
            if (player == null) player = GameObject.FindGameObjectWithTag("Player").transform; //Get the player's transform if it hasn't been gotten yet
            for (int i = 0; i < shadowSpawnCount; i++)
            {
                Vector3 spawnPos = Random.insideUnitSphere * maxSpawnRadius; //Get a random position within the maximum spawn radius
                spawnPos.y = 0.0f; //Set the y position to 0
                spawnPos += transform.position; //Add the boss's position to the spawn position
                if (Vector3.Distance(spawnPos, playerPos) < playerSpawnRadius) //If the spawn position is too close to the player
                {
                    spawnPos = (spawnPos - playerPos).normalized * playerSpawnRadius; //Move the spawn position away from the player
                }
                GameObject shadowEnemy = Instantiate(shadowEnemies[Random.Range(0, shadowEnemies.Count)], spawnPos, Quaternion.identity);
            }
        }

        /// <summary>
        ///  Spawns umbral serpents.
        /// </summary>
        public void SpawnUmbralSerpents()
        {
            if (player == null) player = GameObject.FindGameObjectWithTag("Player").transform; //Get the player's transform if it hasn't been gotten yet
            StartCoroutine(SpawnUmbralSerpentsCoroutine());
        }

        IEnumerator SpawnUmbralSerpentsCoroutine()
        {
            float time = 0.0f;
            while (time < umbralSerpentsSpawnDuration)
            {
                Vector3 spawnPos = Random.insideUnitSphere * maxSpawnRadius; //Get a random position within the maximum spawn radius
                spawnPos.y = 0.0f; //Set the y position to 0
                spawnPos += transform.position; //Add the boss's position to the spawn position
                GameObject umbralSerpent = Instantiate(umbralSerpentsPrefab, spawnPos, Quaternion.identity);
                time += umbralSerpentSpawnDelay;
                Destroy(umbralSerpent, 8f);
                yield return new WaitForSeconds(umbralSerpentSpawnDelay);
            }
        }

        /// <summary>
        ///  Spawns a vile barrier.
        /// </summary>
        public void SpawnVileBarrier()
        {
            if (player == null) player = GameObject.FindGameObjectWithTag("Player").transform; //Get the player's transform if it hasn't been gotten yet
            Vector3 spawnDir = (playerPos - boss.position).normalized; //Get the direction from the boss to the player
            Vector3 spawnPos = boss.position; //Set the spawn position to the boss's position
            GameObject vileBarrier = Instantiate(vileBarrierPrefab, spawnPos, Quaternion.LookRotation(spawnDir)); //Spawn the vile barrier
            StartCoroutine(VileBarrierCoroutine(vileBarrier));
        }

        IEnumerator VileBarrierCoroutine(GameObject barrier)
        {
            // Start a timer for the duration
            float timer = 0.0f;
            while (timer < vileBarrierDuration)
            {
                timer += Time.deltaTime;
                yield return null;
            }
            // Get the materials
            Material[] mats = barrier.GetComponentInChildren<SkinnedMeshRenderer>().materials;
            // Start a timer for the fade
            float t = 0.6f; // The time variable.
            while (t > 0.0f)
            {
                t -= Time.deltaTime * 0.2f; // Decrement the time variable.
                foreach (var mat in mats)
                {
                    mat.SetFloat("_Threshhold", t); // Set the dissolve threshold of the material.
                    mat.SetFloat("_Fade", t / 0.6f); // Set the fade of the material.
                }
                yield return null;
            }
            foreach (var mat in mats)
            {
                mat.SetFloat("_Threshhold", 0.0f); // Set the dissolve threshold of the material.
                mat.SetFloat("_Fade", 0.0f); // Set the fade of the material.
            }
            Destroy(barrier);
        }
    }
}
