using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBaitScript : MonoBehaviour
{
    static bool hasPlacedBait = false;
    public bool isCorrect = false;

    // Start is called before the first frame update
    void Start()
    {
        if (SpawnBaitScript.hasPlacedBait)
        {
            return;
        }
        else{
            SpawnBaitScript.hasPlacedBait = true;
        }

        BossArenaController bossArenaController = FindObjectOfType<BossArenaController>();

        if (bossArenaController != null)
        {
            if (isCorrect)
            {
                bossArenaController.UseCorrectBait();
            }
            else
            {
                bossArenaController.UseWrongBait();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
