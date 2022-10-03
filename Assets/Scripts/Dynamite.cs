using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dynamite : MonoBehaviour
{

    public float m_fuseTime = 3.0f;
    private float m_fuseTimer = 0.0f;

    public float m_damage = 50.0f;
    public float m_explosionRadius = 5.0f;

    public GameObject explosionPrefab;

    [Header("Screenshake: Explosion")]
    public float m_screenshakeDuration = 0.5f;
    public Vector3 m_screenshakeAmplitude = Vector3.one;
    public float m_screenshakeFrequency = 1f;

    [Header("Stats")]
    [SerializeField] public StatsProfile m_statsProfile;

    // Start is called before the first frame update
    void Awake()
    {
        // set fuse timer
        m_fuseTimer = m_fuseTime;
    }

    // Update is called once per frame
    void Update()
    {
        // while the timer is greater than 0, lerp to transform position
        if (m_fuseTimer > 0.0f)
        {
            m_fuseTimer -= Time.deltaTime;
        }
        else
        {
            // explode
            Explode();
        }
    }

    private void Explode()
    {
        // do damage check
        Collider[] colliders = Physics.OverlapSphere(transform.position, m_explosionRadius);
        List<Health_Base> hitObjects = new List<Health_Base>();
        foreach (Collider collider in colliders)
        {
            // get health from parent and children
            Health_Base health = collider.transform.GetComponentInParent<Health_Base>();
            if (health == null) health = collider.transform.GetComponentInChildren<Health_Base>();
            if (health != null && !hitObjects.Contains(health))
            {
                hitObjects.Add(health);
                health.TakeDamage(new Health_Base.DamageStat(m_damage, gameObject, transform.position, collider.transform.position, m_statsProfile));
            }
        }
        FindObjectOfType<ScreenshakeManager>().AddShakeImpulse(m_screenshakeDuration, m_screenshakeAmplitude, m_screenshakeFrequency);

        // instantiate explosion
        GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        // unparent explosion
        explosion.transform.parent = null;
        Destroy(explosion, 20.0f);
        Destroy(gameObject);
    }
}
