using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.IMGUI.Controls;
#endif

public class EnemySpanwer : MonoBehaviour
{
    // a box zone where the enemy will spawn
    public Bounds bounds { get { return m_Bounds; } set { m_Bounds = value; } }
    [SerializeField]
    private Bounds m_Bounds = new Bounds(Vector3.zero, Vector3.one);

    [System.Serializable]
    public class EnemySpawn
    {
        [ReadOnly] public string name = "Enemy";
        public GameObject prefab;
        public float weight = 1;
        [ReadOnly] public float chance = 1;
    }
    public List<EnemySpawn> possibleSpawns = new List<EnemySpawn>();
    private List<GameObject> spawnedEnemies = new List<GameObject>();

    // 2 value slider to control the spawn amount
    [HideInInspector] public int minSpawn = 1;
    [HideInInspector] public int maxSpawn = 1;

    // Start is called before the first frame update
    void Start()
    {
        SpawnAllEnemies();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Vector3 GetRandomPositionOnNavmesh()
    {
        int maxTries = 100;
        Vector3 randomPos = Vector3.zero;

        int i = 0;
        for (i = 0; i < maxTries; i++)
        {
            randomPos = new Vector3(
                Random.Range(bounds.min.x, bounds.max.x),
                bounds.max.y,
                Random.Range(bounds.min.z, bounds.max.z)
            );

            // get random position on navmesh by raycasting down
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPos, out hit, bounds.max.y - bounds.min.y, NavMesh.AllAreas))
            {
                randomPos = hit.position;
            }

            bool isValid = true;

            // check if position is in bounds
            if (!bounds.Contains(randomPos))
            {
                isValid = false;
            }

            // check if can navmesh path to player
            NavMeshPath path = new NavMeshPath();
            NavMesh.CalculatePath(randomPos, FindObjectOfType<PlayerMovement>().transform.position, NavMesh.AllAreas, path);
            if (path.status != NavMeshPathStatus.PathComplete)
            {
                isValid = false;
            }

            if (isValid)
            {
                break;
            }
            else{
                if (i == maxTries - 1)
                {
                    // log error and link the gameobject
                    Debug.LogError("Could not find a valid position for enemy spawn\nPlease check bounds intersect with valid navmesh positions", this);

                    return Vector3.zero;
                }
            }
        }

        return randomPos;
    }

    public void TrySpawnEnemy()
    {
        // get random position on navmesh
        Vector3 spawnPos = GetRandomPositionOnNavmesh();

        // get random enemy to spawn
        EnemySpawn enemySpawn = GetRandomEnemySpawn();

        // spawn enemy
        GameObject enemy = Instantiate(enemySpawn.prefab, spawnPos, Quaternion.identity);

        // random rotation
        enemy.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);

        // add to spawned enemies list
        spawnedEnemies.Add(enemy);
    }

    private EnemySpawn GetRandomEnemySpawn()
    {
        // calculate total weight
        float totalWeight = 0;
        foreach (EnemySpawn enemySpawn in possibleSpawns)
        {
            totalWeight += enemySpawn.weight;
        }

        // calculate chance for each enemy
        float currentWeight = 0;
        foreach (EnemySpawn enemySpawn in possibleSpawns)
        {
            currentWeight += enemySpawn.weight;
            enemySpawn.chance = currentWeight / totalWeight;
        }

        // get random value
        float randomValue = Random.value;

        // get random enemy
        EnemySpawn randomEnemySpawn = possibleSpawns[0];
        foreach (EnemySpawn enemySpawn in possibleSpawns)
        {
            if (randomValue <= enemySpawn.chance)
            {
                randomEnemySpawn = enemySpawn;
                break;
            }
        }

        return randomEnemySpawn;
    }

    public void SpawnAllEnemies()
    {
        // get random amount of enemies to spawn
        int amountToSpawn = Random.Range(minSpawn, maxSpawn + 1);

        // spawn enemies
        for (int i = 0; i < amountToSpawn; i++)
        {
            TrySpawnEnemy();
        }
    }

    // custom editor
#if UNITY_EDITOR
    private void OnValidate() {
        // update chances
        float totalWeight = 0;
        foreach (EnemySpawn enemy in possibleSpawns)
        {
            totalWeight += enemy.weight;
        }

        foreach (EnemySpawn enemy in possibleSpawns)
        {
            enemy.chance = enemy.weight / totalWeight;
        }

        // update names
        foreach (EnemySpawn enemy in possibleSpawns)
        {
            if (enemy.prefab != null)
            {
                enemy.name = enemy.prefab.name;
            }
        }
    }

    [CustomEditor(typeof(EnemySpanwer))]
    public class EnemySpanwerEditor : Editor
    {
        private BoxBoundsHandle m_BoundsHandle = new BoxBoundsHandle();

        protected virtual void OnSceneGUI()
        {
            EnemySpanwer boundsExample = (EnemySpanwer)target;

            // copy the target object's data to the handle
            m_BoundsHandle.center = boundsExample.bounds.center;
            m_BoundsHandle.size = boundsExample.bounds.size;

            // draw the handle
            EditorGUI.BeginChangeCheck();
            m_BoundsHandle.DrawHandle();
            if (EditorGUI.EndChangeCheck() || m_BoundsHandle.center != boundsExample.transform.position)
            {
                // record the target object before setting new values so changes can be undone/redone
                Undo.RecordObject(boundsExample, "Change Bounds");

                // copy the handle's updated data back to the target object
                Bounds newBounds = new Bounds();
                newBounds.center = boundsExample.transform.position;
                newBounds.size = m_BoundsHandle.size;
                boundsExample.bounds = newBounds;
            }
        }

        // custom inspector
        public override void OnInspectorGUI()
        {
            EnemySpanwer boundsExample = (EnemySpanwer)target;

            // draw the default inspector
            DrawDefaultInspector();

            // horiz
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Spawn amount (min, max)");
            boundsExample.minSpawn = EditorGUILayout.IntField(boundsExample.minSpawn);
            boundsExample.maxSpawn = EditorGUILayout.IntField(boundsExample.maxSpawn);
            EditorGUILayout.EndHorizontal();

            // spawn test
            if (GUILayout.Button("Spawn Test"))
            {
                boundsExample.SpawnAllEnemies();
            }

            // delete all spawned enemies
            if (GUILayout.Button("Delete All Spawned Enemies"))
            {
                foreach (GameObject enemy in boundsExample.spawnedEnemies)
                {
                    DestroyImmediate(enemy);
                }
                boundsExample.spawnedEnemies.Clear();
            }

            // on change
            if (GUI.changed)
            {
                // validate min and max
                if (boundsExample.minSpawn < 0)
                {
                    boundsExample.minSpawn = 0;
                }
                if (boundsExample.maxSpawn < boundsExample.minSpawn)
                {
                    boundsExample.maxSpawn = boundsExample.minSpawn;
                }

                // update the inspector
                EditorUtility.SetDirty(boundsExample);
            }
        }
    }
#endif
}
