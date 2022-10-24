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
        Collider[] colliders = Physics.OverlapSphere(transform.position, m_radius); // Get all colliders within the explosion radius.
        foreach (Collider collider in colliders)
        {
            // calc damage
            float calcDmg = StatsManager.CalculateDamage(m_statsProfile, m_damage);

            // Scale damage by distance
            calcDmg *= (1f - Mathf.Pow((Vector3.Distance(collider.transform.position, transform.position) / m_radius), 3f));

            if (collider.gameObject.GetComponentInChildren<Health_Base>() != null)
            {
                collider.gameObject.GetComponentInChildren<Health_Base>().TakeDamage(new Health_Base.DamageStat(damage: calcDmg, sourceObject: gameObject, origin: transform.position, hitPoint: collider.transform.position, m_statsProfile));
            }
            else if (collider.gameObject.GetComponentInParent<Health_Base>() != null)
            {
                collider.gameObject.GetComponentInParent<Health_Base>().TakeDamage(new Health_Base.DamageStat(damage: calcDmg, sourceObject: gameObject, origin: transform.position, hitPoint: collider.transform.position, m_statsProfile));
            }
            else if (collider.gameObject.GetComponentInChildren<PlayerHealth>() != null)
            {
                collider.gameObject.GetComponentInChildren<PlayerHealth>().TakeDamage(calcDmg);
            }
            else if (collider.gameObject.GetComponentInParent<PlayerHealth>() != null)
            {
                collider.gameObject.GetComponentInParent<PlayerHealth>().TakeDamage(calcDmg);
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
