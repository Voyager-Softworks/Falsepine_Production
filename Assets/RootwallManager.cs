using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class RootwallManager : MonoBehaviour
{
    public float spawnRadius = 10f;
    public float minSpawnAngle = 0f;
    public float maxSpawnAngle = 360f;

    public GameObject rootwallPrefab;

    public float duration = 10f;
    List<Vector3> gizmoSpawnPositions;
    List<float> gizmoSpawnAngles;
    Mesh rootwallMesh;

    public bool generateExampleRootwall = false;
    public int exampleRootwallCount = 10;

    List<GameObject> spawned;
    List<float> spawnedTimes;




    // Start is called before the first frame update
    void Start()
    {
        spawned = new List<GameObject>();
        spawnedTimes = new List<float>();
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < spawned.Count; i++)
        {
            if (Time.time - spawnedTimes[i] > duration)
            {
                Destroy(spawned[i], 10);
                spawned[i].GetComponentInChildren<Animator>().SetTrigger("Lower");
                spawned.RemoveAt(i);
                spawnedTimes.RemoveAt(i);
                break;
            }
        }
    }

    public void SpawnRootwall()
    {
        GameObject rootwall = Instantiate(rootwallPrefab, GetSpawnPosition(), Quaternion.Euler(0, Random.Range(0, 360), 0));
        spawned.Add(rootwall);
        spawnedTimes.Add(Time.time);
    }

    void OnDrawGizmosSelected()
    {

        // Draw angle lines
        Gizmos.color = Color.green;
        Vector3 old = transform.position;
        for (int i =  Mathf.FloorToInt(minSpawnAngle); i < Mathf.CeilToInt(maxSpawnAngle); i++)
        {
            float angle = i * Mathf.Deg2Rad;
            Vector3 pos = transform.position + new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * spawnRadius;
            Gizmos.DrawLine(old, pos);
            old = pos;
        }
        Gizmos.DrawLine(old, transform.position);
        if(rootwallMesh == null)
        {
            rootwallMesh = rootwallPrefab.GetComponentInChildren<SkinnedMeshRenderer>().sharedMesh;
        }
        
        if(generateExampleRootwall)
        {
            gizmoSpawnAngles = new List<float>();
            gizmoSpawnPositions = new List<Vector3>();
            for (int i = 0; i < exampleRootwallCount; i++)
            {
                gizmoSpawnAngles.Add(Random.Range(0.0f, 360.0f));
                gizmoSpawnPositions.Add(GetSpawnPosition());
            }
            generateExampleRootwall = false;
        }
        if(gizmoSpawnAngles != null)
        {
            Gizmos.color = new Color(8.0f/16.0f, 4.0f/16.0f, 1.0f/16.0f);
            for (int i = 0; i < gizmoSpawnAngles.Count; i++)
            {
                Vector3 pos = gizmoSpawnPositions[i];
                float angle = gizmoSpawnAngles[i];
                Vector3 offset = rootwallMesh.bounds.center;
                offset = Quaternion.Euler(0, angle, 0) * offset;
                Gizmos.DrawWireMesh(rootwallMesh, pos - offset, Quaternion.Euler(0, angle, 0), rootwallPrefab.transform.localScale);
            }
        }
        
        
        
        
    }

    public Vector3 GetSpawnPosition()
    {
        float angle = Random.Range(minSpawnAngle, maxSpawnAngle) * Mathf.Deg2Rad;
        return transform.position + new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * spawnRadius;
    }

}
