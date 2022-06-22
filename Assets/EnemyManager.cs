using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public GameObject enemiesObject;
    public Rect extents;

    bool enemiesActive = false;

    public void EnableEnemies()
    {
        enemiesObject.SetActive(true);
        enemiesActive = true;
    }

    public void DisableEnemies()
    {
        enemiesObject.SetActive(false);
        enemiesActive = false;
    }

    bool getPlayerInBounds()
    {
        return Physics.BoxCast(transform.position, new Vector3(extents.width, extents.height, extents.width), transform.forward, Quaternion.identity, extents.width, LayerMask.GetMask("Player"));
    }

    void Update()
    {
        if (!enemiesActive)
        {
            if (getPlayerInBounds())
            {
                EnableEnemies();
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(extents.width, extents.height, extents.width));
    }
}
