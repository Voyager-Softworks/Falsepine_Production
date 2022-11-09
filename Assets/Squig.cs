using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Achievements;

public class Squig : ClickableObject
{
    public GameObject[] explosions;

    public override void OnClick()
    {
        base.OnClick();

        AchievementsManager.instance.UnlockAchievement(AchievementsManager.Achievement.HuntSquib);

        // instantiate explosions
        foreach (GameObject explosion in explosions)
        {
            GameObject exp = Instantiate(explosion, transform.position, Quaternion.identity);
            Destroy(exp, 5.0f);
        }

        // destroy squig
        Destroy(gameObject);
    }
}
