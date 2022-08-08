using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootwallManager : MonoBehaviour
{
    public float spawnRadius = 10f;
    public float minSpawnAngle = 0f;
    public float maxSpawnAngle = 360f;

    public GameObject rootwallPrefab;
    List<Vector3> gizmoSpawnPositions;
    List<float> gizmoSpawnAngles;
    Mesh rootwallMesh;

    public bool generateExampleRootwall = false;
    public int exampleRootwallCount = 10;




    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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

    public Vector3 GetSpawnPosition()
    {
        float angle = Random.Range(minSpawnAngle, maxSpawnAngle) * Mathf.Deg2Rad;
        return transform.position + new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * spawnRadius;
    }

}
