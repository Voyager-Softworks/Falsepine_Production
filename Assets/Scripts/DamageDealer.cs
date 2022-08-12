using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using System;

/// <summary>
///  Script that handles dealing damage to the player through melee, ranged, and AOE attacks.
/// </summary>
public class DamageDealer : MonoBehaviour
{

    public List<Collider> HurtBoxes = new List<Collider>(); ///< Colliders used to detect when the player is hit by an attack

    public GameObject hurtPlayerEffect; ///< Particle effect spawned when the player is hurt
    
    public GameObject indicatorPrefab; ///< Indicator that shows where the attack will hit

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
    IEnumerator AOEAttackCoroutine(float dmg, float delay, float radius, GameObject effect, Vector2 offset, float stunDuration)
    {
        yield return new WaitForSeconds(delay);
        Vector3 offsetVector = transform.forward * offset.y + transform.right * offset.x;
        Destroy(Instantiate(effect, transform.position + offsetVector, Quaternion.identity), 20.0f);
        RaycastHit[] hits = Physics.SphereCastAll(transform.position + offsetVector, radius, transform.forward, 0.5f);
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
    ///  Coroutine to display an attack indicator for AOE attacks.
    /// </summary>
    /// <param name="delay"></param>
    /// <param name="radius"></param>
    /// <param name="translationSpeed"></param>
    /// <param name="translationDuration"></param>
    /// <param name="translationDirection"></param>
    /// <returns></returns>
    IEnumerator IndicatorCoroutine(float delay, float radius, Vector2 offset, Func<Vector3> playerDirectionFunction,float translationSpeed, float translationDuration, Color attackColor, float indicatorDuration)
    {
        yield return new WaitForSeconds(delay-indicatorDuration);
        Vector3 offsetVector = transform.forward * offset.y + transform.right * offset.x;
        GameObject indicator = Instantiate(indicatorPrefab, transform.position + offsetVector + (playerDirectionFunction() * (translationSpeed * translationDuration)) - Vector3.up, Quaternion.Euler(90, 0, 0));
        float t = 0.0f;
        DecalProjector decalProjector = indicator.GetComponent<DecalProjector>();
        decalProjector.material.SetColor("_BaseColor", attackColor);
        
        decalProjector.material.SetColor("_EmissiveColor", attackColor);
        decalProjector.size = Vector3.zero;
        Vector3 startSize = new Vector3(0.0f, 0.0f, 2.0f);
        Vector3 endSize = new Vector3(radius*4.0f, radius*4.0f, 2.0f);
        while (t < indicatorDuration)
        {
            Vector3 groundPos = transform.position;
            groundPos.y = 0.0f;
            offsetVector = transform.forward * offset.y + transform.right * offset.x;
            indicator.transform.position = (groundPos+offsetVector) + (transform.forward * translationSpeed * translationDuration * (1-((t+(delay-indicatorDuration))/delay))) + (Vector3.up);
            
            t += Time.deltaTime;
            decalProjector.size = Vector3.Lerp(startSize, endSize, t / indicatorDuration);
            yield return null;
        }
        Destroy(indicator);
        
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
            proj.transform.rotation = Quaternion.LookRotation(dir);
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
    public void AOEAttack(float dmg, float delay, float radius, GameObject effect, Vector2 offset, float stunDuration)
    {
        StartCoroutine(AOEAttackCoroutine(dmg, delay, radius, effect, offset, stunDuration));
    }

    public void DisplayIndicator(float delay, float radius, Vector2 offset, Func<Vector3> playerDirectionFunction, float translationSpeed, float translationDuration, Color color, float indicatorDuration)
    {
        StartCoroutine(IndicatorCoroutine(delay, radius, offset, playerDirectionFunction, translationSpeed, translationDuration, color, indicatorDuration));
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

    public void CancelAttack()
    {
        StopAllCoroutines();
    }
    
    

    


}
