using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LevelGenerator : MonoBehaviour
{
    public GameObject[] tiles;
    public float tileRadius = 1f;
    
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GenerateLevel());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator GenerateLevel()
    {
        List<NavMeshSurface> surfaces = new List<NavMeshSurface>();
        for(int i = -1; i <= 1; i++)
        {
            for(int j = -1; j <= 1; j++)
            {
                Vector3 pos = new Vector3(i * (tileRadius * 2.0f), 0, j * (tileRadius * 2.0f));
                //Instantiate tiles with random rotation
                surfaces.Add(Instantiate(tiles[Random.Range(0, tiles.Length)], pos, Quaternion.Euler(0, Random.Range(0, 3) * 90, 0)).GetComponent<NavMeshSurface>());
                yield return new WaitForEndOfFrame();
            }
        }
        foreach(NavMeshSurface surface in surfaces)
        {
            surface.BuildNavMesh();
            yield return new WaitForEndOfFrame();
        }
        yield return null;
    }


    void OnDrawGizmos()
    {
        for(int i = -1; i <= 1; i++)
        {
            for(int j = -1; j <= 1; j++)
            {
                Vector3 pos = new Vector3(i * (tileRadius * 2.0f), 0, j * (tileRadius * 2.0f));
                Vector3 cornerA = pos + new Vector3(-tileRadius, 0, -tileRadius);
                Vector3 cornerB = pos + new Vector3(tileRadius, 0, -tileRadius);
                Vector3 cornerC = pos + new Vector3(tileRadius, 0, tileRadius);
                Vector3 cornerD = pos + new Vector3(-tileRadius, 0, tileRadius);
                Gizmos.DrawLine(cornerA, cornerB);
                Gizmos.DrawLine(cornerB, cornerC);
                Gizmos.DrawLine(cornerC, cornerD);
                Gizmos.DrawLine(cornerD, cornerA);
            }
        }
    }
}
