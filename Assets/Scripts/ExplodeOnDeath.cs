using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodeOnDeath : MonoBehaviour
{
    public GameObject m_explosion;
    public float m_damage = 10f;
    public float m_radius = 5f;

    public float m_fuseTime = 0f;

    [Header("Screenshake: Explosion")]
    public float m_screenshakeDuration = 0.5f;
    public Vector3 m_screenshakeAmplitude = Vector3.one;
    public float m_screenshakeFrequency = 1f;

    [Header("Stats")]
    [SerializeField] public StatsProfile m_statsProfile;


    // Start is called before the first frame update
    void Start()
    {
        if (GetComponent<Health_Base>()) GetComponent<Health_Base>().Death += (ctx) => { Explode(ctx); };

    }

    void Explode(Health_Base.DeathContext context)
    {
        var explosion = Instantiate(m_explosion, transform.position, Quaternion.identity);
        Destroy(explosion, 5f);
        Collider[] colliders = Physics.OverlapSphere(transform.position, m_radius);
        foreach (Collider collider in colliders)
        {
            // calc damage
            float calcDmg = StatsManager.CalculateDamage(m_statsProfile, m_damage);

            // Scale damage by distance
            calcDmg *= (1f - (Vector3.Distance(collider.transform.position, transform.position) / m_radius));

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
        FindObjectOfType<ScreenshakeManager>().AddShakeImpulse(m_screenshakeDuration, m_screenshakeAmplitude, m_screenshakeFrequency);

    }

    IEnumerator Fuse(Health_Base.DeathContext context)
    {
        yield return new WaitForSeconds(m_fuseTime);
        Explode(context);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
