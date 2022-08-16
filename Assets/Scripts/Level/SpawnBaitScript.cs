using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// @deprecated
/// Spawns the bait when player is close enough.
/// </summary>
public class SpawnBaitScript : MonoBehaviour
{
    public bool isCorrectBait = false;

    private BaitLocationScript placeBaitScript;

    // Start is called before the first frame update
    void Start()
    {
        placeBaitScript = GameObject.FindObjectOfType<BaitLocationScript>();
        if (placeBaitScript == null) return;

        if (Vector3.Distance(transform.position, placeBaitScript.transform.position) < placeBaitScript.placeDistance)
        {
            placeBaitScript.PlaceBait(this);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
