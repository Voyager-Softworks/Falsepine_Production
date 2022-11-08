using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Achievements;

public class MyRoots : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<EnemyHealth>().Death += (ctx) => { GiveRootsAchievement(); };
    }

    // Update is called once per frame
    void Update()
    {

    }

    void GiveRootsAchievement()
    {
        if (FindObjectOfType<AchievementsManager>() is AchievementsManager am)
        {
            am.UnlockAchievement(AchievementsManager.Achievement.DefeatPossessedDummy);
        }
    }
}
