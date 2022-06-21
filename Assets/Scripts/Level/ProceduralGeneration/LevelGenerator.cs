using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LevelGenerator : MonoBehaviour
{
    public Tile[] tiles;

    public struct Tile
    {
        public GameObject tile;
        public float spawnChance;
        public bool canRotate;
    }
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

    Tile[] PickRandomTiles(int count)
    {
        List<Tile> pickedTiles = new List<Tile>();
        for (int i = 0; i < count; i++)
        {
            float totalChance = 0;
            foreach (Tile tile in tiles)
            {
                totalChance += tile.spawnChance;
            }
            float randomPoint = Random.value * totalChance;
            float currentChance = 0;
            foreach (Tile tile in tiles)
            {
                currentChance += tile.spawnChance;
                if (randomPoint <= currentChance)
                {
                    pickedTiles.Add(tile);
                    break;
                }
            }
        }
        return pickedTiles.ToArray();
    }

    IEnumerator GenerateLevel()
    {
        List<NavMeshSurface> surfaces = new List<NavMeshSurface>();
        Tile[] pickedTiles = PickRandomTiles(9);
        int k = 0;
        for(int i = -1; i <= 1; i++)
        {
            for(int j = -1; j <= 1; j++)
            {
                Vector3 pos = new Vector3(i * (tileRadius * 2.0f), 0, j * (tileRadius * 2.0f));
                //Instantiate tiles with random rotation
                surfaces.Add(Instantiate(tiles[k].tile, pos, tiles[k].canRotate ? Quaternion.Euler(0,  Random.Range(0, 3) * 90, 0)  : Quaternion.identity).GetComponent<NavMeshSurface>());
                k++;
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
