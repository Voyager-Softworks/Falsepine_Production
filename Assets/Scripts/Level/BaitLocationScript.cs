using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaitLocationScript : MonoBehaviour  /// @todo comment
{
    public float placeDistance = 3.0f;
    public bool hasPlacedBait = false;
    public GameObject correctBaitModel;
    public GameObject wrongBaitModel;
    public GameObject canvas;
    private bool hasSpawnedBoss = false;
    private bool isCorrectBait = false;

    private BossArenaController bossArenaController;

    // Start is called before the first frame update
    void Start()
    {
        bossArenaController = FindObjectOfType<BossArenaController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (hasPlacedBait)
        {
            return;
        }
    }

    public void PlaceBait(SpawnBaitScript _bait)
    {
        if (hasPlacedBait || hasSpawnedBoss || bossArenaController == null) return;

        hasPlacedBait = true;
        hasSpawnedBoss = true;
        isCorrectBait = _bait.isCorrectBait;

        if (isCorrectBait)
        {
            bossArenaController.UseCorrectBait();
            correctBaitModel.SetActive(true);
        }
        else
        {
            bossArenaController.UseWrongBait();
            wrongBaitModel.SetActive(true);
        }

        Destroy(_bait.gameObject);
        canvas.SetActive(false);
    }
}
