using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
