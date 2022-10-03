using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaDamage : MonoBehaviour
{
    public float m_radius = 5.0f;
    public float m_damage = 1.0f;
    public float m_damageDelay = 0.1f;
    private float m_damageTimer = 0.0f;
    public float m_time = 5.0f;
    private float m_timer = 0.0f;
    public StatsProfile m_statsProfile;

    // Start is called before the first frame update
    void Start()
    {
        m_timer = m_time;
    }

    // Update is called once per frame
    void Update()
    {
        // if timer is greater than 0, count down
        if (m_timer > 0.0f)
        {
            m_timer -= Time.deltaTime;
        }
        else
        {
            // destroy
            Destroy(gameObject, 10.0f);

            // stop particle system
            ParticleSystem[] ps = GetComponentsInChildren<ParticleSystem>();
            foreach (ParticleSystem p in ps)
            {
                p.Stop();
            }
        }

        // if damage timer is greater than 0, count down
        if (m_damageTimer > 0.0f)
        {
            m_damageTimer -= Time.deltaTime;
        }
        else if (m_timer > 0.0f)
        {
            // reset damage timer
            m_damageTimer = m_damageDelay;

            // get all colliders in radius
            Collider[] colliders = Physics.OverlapSphere(transform.position, m_radius);

            List<Health_Base> hitObjects = new List<Health_Base>();
            // loop through colliders
            foreach (Collider collider in colliders)
            {
                // get health component in parents and children
                Health_Base health = collider.GetComponentInParent<Health_Base>() ?? collider.GetComponentInChildren<Health_Base>();

                // if health is not null and not already hit
                if (health != null && !hitObjects.Contains(health))
                {
                    // add to hit list
                    hitObjects.Add(health);

                    // calculate damage
                    float calcDmg = StatsManager.CalculateDamage(m_statsProfile, m_damage);

                    // take damage
                    health.TakeDamage(new Health_Base.DamageStat(calcDmg, gameObject, collider.transform.position, transform.position, m_statsProfile));
                }
            }
        }
    }
}
