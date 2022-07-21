using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///  Script that handles dealing damage to the player through melee, ranged, and AOE attacks.
/// </summary>
public class DamageDealer : MonoBehaviour
{

    public List<Collider> HurtBoxes = new List<Collider>(); ///< Colliders used to detect when the player is hit by an attack

    public GameObject hurtPlayerEffect; ///< Particle effect spawned when the player is hurt

    public float damage = 10f; ///< Damage done by the attack
    public int attkNum = 1; ///< Number of Attacks
    int currAttkNum = 0;

    /// <summary>
    ///  Coroutine to execute a melee attack on the player.
    /// </summary>
    /// <param name="dmg">The damage of the attack.</param>
    /// <param name="delay">The amount of time before the damage is dealt.</param>
    /// <param name="duration">The length of time during which the hurtboxes are checked for collision with the player.</param>
    /// <param name="stunDuration">The length of time to stun the player for if they are hit.</param>
    /// <returns></returns>
    IEnumerator MeleeAttackCoroutine(float dmg, float delay, float duration, float stunDuration)
    {
        yield return new WaitForSeconds(delay);
        foreach (Collider hurtBox in HurtBoxes)
        {
            hurtBox.enabled = true;
        }
        float timer = 0f;
        bool playerHit = false;
        while(timer < duration && !playerHit)
        {
            timer += Time.deltaTime;
            foreach (Collider hurtBox in HurtBoxes)
            {
                if (hurtBox.enabled)
                {
                    RaycastHit[] hits = Physics.BoxCastAll(hurtBox.bounds.center, hurtBox.bounds.extents, hurtBox.transform.forward, hurtBox.transform.rotation, 0.5f);
                    foreach (RaycastHit hit in hits)
                    {
                        if (hit.collider.CompareTag("Player"))
                        {
                            hit.collider.GetComponent<PlayerHealth>().TakeDamage(dmg);
                            hit.collider.GetComponent<PlayerHealth>().Stun(stunDuration);
                            Instantiate(hurtPlayerEffect, hit.point, Quaternion.identity);
                            playerHit = true;
                            break;
                        }
                    }
                }
            }
            yield return null;
        }
        foreach (Collider hurtBox in HurtBoxes)
        {
            hurtBox.enabled = false;
        }
    }

    /// <summary>
    ///  Coroutine to execute an AOE attack on the player.
    /// </summary>
    /// <param name="dmg">The damage of the attack.</param>
    /// <param name="delay">The amount of time before the damage is dealt.</param>
    /// <param name="radius">The radius of the AOE attack.</param>
    /// <param name="effect">The effect to spawn when the player is hit.</param>
    /// <param name="stunDuration">The length of time to stun the player for if they are hit.</param>
    /// <returns></returns>
    IEnumerator AOEAttackCoroutine(float dmg, float delay, float radius, GameObject effect, float stunDuration)
    {
        yield return new WaitForSeconds(delay);
        Destroy(Instantiate(effect, transform.position, Quaternion.identity), 20.0f);
        RaycastHit[] hits = Physics.SphereCastAll(transform.position, radius, transform.forward, 0.5f);
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.CompareTag("Player"))
            {
                hit.collider.GetComponent<PlayerHealth>().TakeDamage(dmg);
                hit.collider.GetComponent<PlayerHealth>().Stun(stunDuration);
                Instantiate(hurtPlayerEffect, hit.point, Quaternion.identity);
                break;
            }
        }
    }

    /// <summary>
    ///  Coroutine to execute a ranged attack on the player.
    /// </summary>
    /// <param name="projectile">The projectile to spawn.</param>
    /// <param name="delay">The amount of time before the damage is dealt.</param>
    /// <param name="speed">The speed of the projectile.</param>
    /// <param name="spawnpoint">The spawnpoint of the projectile.</param>
    /// <param name="aimAtPlayer">Whether or not the projectile should aim at the player.</param>
    /// <returns></returns>
    IEnumerator RangedAttackCoroutine(GameObject projectile, float delay, float speed, Transform spawnpoint, bool aimAtPlayer = false)
    {
        yield return new WaitForSeconds(delay);
        GameObject proj = Instantiate(projectile, spawnpoint.position, spawnpoint.rotation);
        if(!aimAtPlayer)
            proj.GetComponent<Rigidbody>().velocity = proj.transform.forward * speed;
        else
        {
            Vector3 dir = (GameObject.FindGameObjectWithTag("Player").transform.position - proj.transform.position).normalized;
            proj.GetComponent<Rigidbody>().velocity = dir * speed;
        }
        Destroy(proj, 20.0f);
    }

    /// <summary>
    ///  Coroutine to execute a ranged attack on the player.
    /// </summary>
    /// <param name="projectile">The projectile to spawn.</param>
    /// <param name="delay">The amount of time before the damage is dealt.</param>
    /// <param name="speed">The speed of the projectile.</param>
    /// <param name="spawnPoint">The spawnpoint of the projectile.</param>
    /// <param name="duration">The duration for which projectiles should be spawned.</param>
    /// <param name="waitBetweenSpawns">The amount of time between each projectile spawn.</param>
    /// <returns></returns>
    IEnumerator RangedAttackCoroutine(GameObject projectile, float delay, float speed, Transform spawnPoint, float duration, float waitBetweenSpawns)
    {
        yield return new WaitForSeconds(delay);
        for (int i = 0; i < duration / waitBetweenSpawns; i++)
        {
            GameObject proj = Instantiate(projectile, spawnPoint.position, spawnPoint.rotation);
            proj.GetComponent<Rigidbody>().velocity = proj.transform.forward * -speed;
            Destroy(proj, 20.0f);
            yield return new WaitForSeconds(waitBetweenSpawns);
        }
    }

    /// <summary>
    ///  Do a melee attack on the player.
    /// </summary>
    /// <param name="dmg">The Damage</param>
    /// <param name="delay">The amount of time before the damage is dealt.</param>
    /// <param name="duration">The length of time during which the hurtboxes are checked for collision with the player.</param>
    /// <param name="stunDuration">The length of time to stun the player for if they are hit.</param>
    public void MeleeAttack(float dmg, float delay, float duration, float stunDuration)
    {
        StartCoroutine(MeleeAttackCoroutine(dmg, delay, duration, stunDuration));
    }

    /// <summary>
    ///  Do an AOE attack on the player.
    /// </summary>
    /// <param name="dmg">The Damage</param>
    /// <param name="delay">The amount of time before the damage is dealt.</param>
    /// <param name="radius">The radius of the AOE attack.</param>
    /// <param name="effect">The effect to spawn when the player is hit.</param>
    /// <param name="stunDuration">The length of time to stun the player for if they are hit.</param>
    public void AOEAttack(float dmg, float delay, float radius, GameObject effect, float stunDuration)
    {
        StartCoroutine(AOEAttackCoroutine(dmg, delay, radius, effect, stunDuration));
    }

    /// <summary>
    ///  Do a ranged attack on the player.
    /// </summary>
    /// <param name="projectile">The projectile to spawn.</param>
    /// <param name="delay">The amount of time before the damage is dealt.</param>
    /// <param name="speed">The speed of the projectile.</param>
    /// <param name="spawnpoint">The spawnpoint of the projectile.</param>
    /// <param name="aimAtPlayer">Whether or not the projectile should aim at the player.</param>
    public void RangedAttack(GameObject projectile, float delay, float speed, Transform spawnpoint, bool aimAtPlayer = false)
    {
        StartCoroutine(RangedAttackCoroutine(projectile, delay, speed, spawnpoint, aimAtPlayer));
    }

    /// <summary>
    ///  Do a ranged attack on the player.
    /// </summary>
    /// <param name="projectile">The projectile to spawn.</param>
    /// <param name="delay">The amount of time before the damage is dealt.</param>
    /// <param name="speed">The speed of the projectile.</param>
    /// <param name="spawnPoint">The spawnpoint of the projectile.</param>
    /// <param name="duration">The duration for which projectiles should be spawned.</param>
    /// <param name="waitBetweenSpawns">The amount of time between each projectile spawn.</param>
    public void RangedAttack(GameObject projectile, float delay, float speed, Transform spawnPoint, float duration, float waitBetweenSpawns)
    {
        StartCoroutine(RangedAttackCoroutine(projectile, delay, speed, spawnPoint, duration, waitBetweenSpawns));
    }

    /// <summary>
    /// 
    /// </summary>
    public void Update()
    {
        if(currAttkNum > 0)
        {
            
        }
    }
    
    

    


}
