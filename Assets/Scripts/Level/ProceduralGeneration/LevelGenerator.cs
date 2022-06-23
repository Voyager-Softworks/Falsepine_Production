using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class LevelGenerator : MonoBehaviour
{
    public Tile[] tiles;


    [System.Serializable]
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

    Tile[] ShuffleTiles()
    {
        return tiles.OrderBy(x => Random.value).ToArray();
    }

    Tile[] PickRandomTiles(int count)
    {
        List<Tile> pickedTiles = new List<Tile>();
        foreach (Tile tile in tiles)
        {
            if(tile.spawnChance == 1.0f)
            {
                pickedTiles.Add(tile);
            }
            else
            {
                float random = Random.Range(0f, 1f);
                if(random < tile.spawnChance)
                {
                    pickedTiles.Add(tile);
                }
            }
        }
        while(pickedTiles.Count < count)
        {
            foreach (Tile tile in tiles)
            {
                if(pickedTiles.Count == count)
                {
                    break;
                }
                if(tile.spawnChance < 1.0f)
                {
                    float random = Random.Range(0f, 1f);
                    if(random < tile.spawnChance)
                    {
                        pickedTiles.Add(tile);
                    }
                }
                else
                {
                    continue;
                }
            }
        }
        Debug.Log(pickedTiles.Count);
        return pickedTiles.ToArray();
    }

    IEnumerator GenerateLevel()
    {
        //List<NavMeshSurface> surfaces = new List<NavMeshSurface>();
        Tile[] pickedTiles = ShuffleTiles();
        int k = 0;
        for(int i = -1; i <= 1; i++)
        {
            for(int j = -1; j <= 1; j++)
            {
                Vector3 pos = new Vector3(i * (tileRadius * 2.0f), 0, j * (tileRadius * 2.0f));
                //Instantiate tiles with random rotation
                Instantiate(pickedTiles[k].tile, 
                pos, 
                pickedTiles[k].canRotate ? Quaternion.Euler(0,  Random.Range(0, 3) * 90, 0)  : Quaternion.identity,
                this.transform
                );
                k++;
                yield return new WaitForSeconds(.2f);
            }
        }
        
        yield return new WaitForEndOfFrame();

        GetComponent<NavMeshSurface>().BuildNavMesh();
        
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
