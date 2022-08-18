using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///  Manages eneny awareness of the player.
/// </summary>
public class EnemyAwarenessManager : MonoBehaviour
{
    public struct Context
    {
        public GameObject enemy;
        public bool fromOtherEnemy;
    }

    public System.Action<Context> onEnemyAware;

    public void RegisterAwareness(GameObject self)
    {
        if (onEnemyAware != null)
        {
            Context context = new Context();
            context.enemy = self;
            context.fromOtherEnemy = true;
            onEnemyAware.Invoke(context);
        }
    }

}
