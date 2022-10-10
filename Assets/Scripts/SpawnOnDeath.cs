using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///  Spawns a prefab apon death of an agent, as triggered by a <see cref="Health_Base"/> component.
/// </summary>  
public class SpawnOnDeath : MonoBehaviour
{
    public GameObject spawnPrefab;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<EnemyHealth>().Death += (ctx) =>
        {
            Instantiate(spawnPrefab, transform.position, Quaternion.identity);
        };
    }


}
