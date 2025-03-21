using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

/// <summary>
///  AOE effect spawned by Corpse Puppet Screamer enemy. 
/// </summary>  
public class ScreamerRipple : MonoBehaviour
{
    public float range = 7.5f; //The range of the ripple
    public float duration = 2f; //The duration of the ripple
    public float damage = 10f; //The damage of the ripple
    public float damageInterval = 0.5f;  //The interval between damage ticks
    float damageTimer = 0f; //The timer for damage
    DecalProjector decalProjector; //The decal projector component
    public AnimationCurve opacityCurve; //The opacity curve of the ripple
    float timer = 0f; //The timer for the ripple
    bool doDamage = true; //Whether or not to do damage

    [Header("AOE persist settings")]
    public float persistTime = 5.0f; //The time to persist after creature is dead
    private float persistTimer = 0.0f; //The timer for persisting
    private bool hasStopped = false; //Whether or not the ripple has stopped
    private EnemyHealth enemyHealth;



    // Start is called before the first frame update
    void Start()
    {
        decalProjector = GetComponentInChildren<DecalProjector>(); //Get the decal projector component
        if (decalProjector != null)
        {
            decalProjector.size = new Vector3(range * 3, range * 3, 1f); //Set the size of the decal projector
        }

        enemyHealth = GetComponentInParent<EnemyHealth>(); //Get the enemy health component
        persistTimer = persistTime; //Set the persist timer
    }

    // Update is called once per frame
    void Update()
    {
        if (hasStopped) return;

        timer += Time.deltaTime; //Increment the timer
        if (timer > duration)
        {
            Destroy(gameObject, 2f); //Destroy the ripple after the duration
            doDamage = false;
        }
        else
        {

            damageTimer += Time.deltaTime; //Increment the damage timer
            if (damageTimer > damageInterval && doDamage)
            {
                damageTimer = 0f; //Reset the damage timer
                Collider[] colliders = Physics.OverlapSphere(transform.position, range); //Get all colliders in the range of the ripple
                foreach (Collider collider in colliders)
                {
                    if (collider.gameObject.tag == "Player")
                    {
                        collider.gameObject.GetComponent<PlayerHealth>().TakeDamage(damage); //Damage the player
                    }
                }
            }
        }
        float t = timer / duration; //Get the time as a percentage
        if (decalProjector != null && opacityCurve != null)
        {
            decalProjector.fadeFactor = opacityCurve.Evaluate(t); //Set the fade factor of the decal projector
        }


        // death check
        if (enemyHealth != null && enemyHealth.hasDied)
        {
            persistTimer -= Time.deltaTime;
            if (persistTimer <= 0.0f)
            {
                // stop all particles
                ParticleSystem[] particleSystems = GetComponentsInChildren<ParticleSystem>();
                foreach (ParticleSystem particleSystem in particleSystems)
                {
                    particleSystem.Stop();
                }

                hasStopped = true;
            }
        }
    }
}
