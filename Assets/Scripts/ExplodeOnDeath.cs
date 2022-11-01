using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///  Makes an enemy or object explode when it dies while using a component that inherits from <see cref="Health_Base"/>.
/// </summary>  @todo comment
public class ExplodeOnDeath : MonoBehaviour
{
    public GameObject m_explosion; ///< The explosion prefab to instantiate when the object dies.
    public float m_damage = 10f; ///< The damage to deal to nearby objects.
    public float m_radius = 5f; ///< The radius of the explosion.

    public float m_fuseTime = 0f; ///< The time to wait before exploding.

    [Header("Screenshake: Explosion")]
    public float m_screenshakeDuration = 0.5f; ///< The duration of the screenshake.
    public Vector3 m_screenshakeAmplitude = Vector3.one; ///< The amplitude of the screenshake.
    public float m_screenshakeFrequency = 1f; ///< The frequency of the screenshake.

    [Header("Stats")]
    [SerializeField] public StatsProfile m_statsProfile; ///< The stats profile to use for this object.


    // Start is called before the first frame update
    void Start()
    {
        if (GetComponent<Health_Base>()) GetComponent<Health_Base>().Death += (ctx) => { Explode(ctx); }; // If the object has a health component, subscribe to its death event.
        else Debug.LogWarning("ExplodeOnDeath: No health component found on " + gameObject.name + "."); // If the object doesn't have a health component, warn the user.

    }

    /// <summary>
    ///  Explodes the object.
    /// </summary>
    /// <param name="context"></param>
    void Explode(Health_Base.DeathContext context)
    {
        // Stop all playing audio.
        foreach (AudioSource audioSource in GetComponentsInChildren<AudioSource>())
        {
            audioSource.Stop();
        }
        var explosion = Instantiate(m_explosion, transform.position, Quaternion.identity); // Instantiate the explosion prefab.
        Destroy(explosion, 5f); // Destroy the explosion after 5 seconds.
        // do damage check
        Collider[] colliders = Physics.OverlapSphere(transform.position, m_radius);
        List<Health_Base> hitObjects = new List<Health_Base>();
        List<PlayerHealth> hitPlayers = new List<PlayerHealth>();
        foreach (Collider collider in colliders)
        {
            float calcedDamage = StatsManager.CalculateDamage(m_statsProfile, m_damage);

            // Scale damage by distance
            calcedDamage *= (1f - Mathf.Pow((Vector3.Distance(collider.transform.position, transform.position) / m_radius), 3f));

            // get health from parent and children
            Health_Base health = collider.transform.GetComponentInParent<Health_Base>();
            if (health == null) health = collider.transform.GetComponentInChildren<Health_Base>();
            if (health != null && !hitObjects.Contains(health))
            {
                hitObjects.Add(health);
                health.TakeDamage(new Health_Base.DamageStat(calcedDamage, gameObject, transform.position, collider.transform.position, m_statsProfile));
            }

            // get player health from parent and children
            PlayerHealth playerHealth = collider.transform.GetComponentInParent<PlayerHealth>();
            if (playerHealth == null) playerHealth = collider.transform.GetComponentInChildren<PlayerHealth>();
            if (playerHealth != null && !hitPlayers.Contains(playerHealth))
            {
                hitPlayers.Add(playerHealth);
                playerHealth.TakeDamage(calcedDamage);
            }
        }
        FindObjectOfType<ScreenshakeManager>().AddShakeImpulse(m_screenshakeDuration, m_screenshakeAmplitude, m_screenshakeFrequency); // Add a screenshake.

    }

    /// <summary>
    ///  Explodes the object after a delay.
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    IEnumerator Fuse(Health_Base.DeathContext context)
    {
        yield return new WaitForSeconds(m_fuseTime);
        Explode(context);
    }

}
