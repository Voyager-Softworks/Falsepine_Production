using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageDealer : MonoBehaviour
{

    public List<Collider> HurtBoxes = new List<Collider>();

    public GameObject hurtPlayerEffect;

    public float damage = 10f;
    public int attkNum = 1;
    int currAttkNum = 0;

    IEnumerator MeleeAttackCoroutine(float dmg, float delay, float duration)
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

    IEnumerator AOEAttackCoroutine(float dmg, float delay, float radius, GameObject effect)
    {
        yield return new WaitForSeconds(delay);
        Destroy(Instantiate(effect, transform.position, Quaternion.identity), 20.0f);
        RaycastHit[] hits = Physics.SphereCastAll(transform.position, radius, transform.forward, 0.5f);
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.CompareTag("Player"))
            {
                hit.collider.GetComponent<PlayerHealth>().TakeDamage(dmg);
                Instantiate(hurtPlayerEffect, hit.point, Quaternion.identity);
            }
        }
    }

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

    IEnumerator RangedAttackCoroutine(GameObject projectile, float delay, float speed, Transform spawnPoint, float duration, float waitBetweenSpawns)
    {
        yield return new WaitForSeconds(delay);
        for (int i = 0; i < duration / waitBetweenSpawns; i++)
        {
            GameObject proj = Instantiate(projectile, spawnPoint.position, spawnPoint.rotation);
            proj.GetComponent<Rigidbody>().velocity = proj.transform.forward * speed;
            Destroy(proj, 20.0f);
            yield return new WaitForSeconds(waitBetweenSpawns);
        }
    }

    public void MeleeAttack(float dmg, float delay, float duration)
    {
        StartCoroutine(MeleeAttackCoroutine(dmg, delay, duration));
    }

    public void AOEAttack(float dmg, float delay, float radius, GameObject effect)
    {
        StartCoroutine(AOEAttackCoroutine(dmg, delay, radius, effect));
    }

    public void RangedAttack(GameObject projectile, float delay, float speed, Transform spawnpoint, bool aimAtPlayer = false)
    {
        StartCoroutine(RangedAttackCoroutine(projectile, delay, speed, spawnpoint, aimAtPlayer));
    }

    public void RangedAttack(GameObject projectile, float delay, float speed, Transform spawnPoint, float duration, float waitBetweenSpawns)
    {
        StartCoroutine(RangedAttackCoroutine(projectile, delay, speed, spawnPoint, duration, waitBetweenSpawns));
    }

    public void Update()
    {
        if(currAttkNum > 0)
        {
            
        }
    }
    
    

    


}
