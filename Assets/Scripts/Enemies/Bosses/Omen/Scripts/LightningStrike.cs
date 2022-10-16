using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class LightningStrike : MonoBehaviour
{
    public GameObject effect;
    public float lightningDuration = 1.0f;
    public float lightningDelay = 0.5f;
    public float damageRadius = 5.0f;
    public float damage = 10.0f;
    float delayTimer = 0.0f;
    float durationTimer = 0.0f;
    public DecalProjector decalProjector;
    public DecalProjector scorchMarkProjector;
    public AudioSource boltSource;
    Vector3 decalSizeInitial;
    Vector3 decalSizeFinal;
    bool hasDamaged = false;
    bool struck = false;

    // Start is called before the first frame update
    void Start()
    {
        decalSizeInitial = new Vector3(0.0f, 0.0f, 6.0f);
        decalSizeFinal = new Vector3(damageRadius * 2, damageRadius * 2, 6.0f);
        decalProjector.size = decalSizeInitial;
        scorchMarkProjector.enabled = false;
        boltSource.pitch = Random.Range(0.5f, 1.1f);
        transform.localRotation = Quaternion.Euler(0.0f, Random.Range(0.0f, 360.0f), 0.0f); //Random Y rotation
    }

    // Update is called once per frame
    void Update()
    {
        if (delayTimer < lightningDelay)
        {
            delayTimer += Time.deltaTime;
            decalProjector.size = Vector3.Lerp(decalSizeInitial, decalSizeFinal, delayTimer / lightningDelay);
        }
        else
        {
            if (!struck)
            {
                boltSource.Play();
                struck = true;
            }
            decalProjector.enabled = false;
            scorchMarkProjector.enabled = true;
            if (durationTimer < lightningDuration)
            {
                durationTimer += Time.deltaTime;
                effect.SetActive(true);
                if (!hasDamaged)
                {
                    Collider[] hitColliders = Physics.OverlapSphere(transform.position, damageRadius);
                    foreach (Collider hitCollider in hitColliders)
                    {
                        if (hitCollider.gameObject.tag == "Player")
                        {
                            hitCollider.gameObject.GetComponent<PlayerHealth>().TakeDamage(damage);
                        }
                    }
                    hasDamaged = true;
                }
            }
            else
            {
                scorchMarkProjector.fadeFactor = Mathf.Max(0.0f, scorchMarkProjector.fadeFactor - (Time.deltaTime * 0.05f));
                Destroy(gameObject, 20f);

            }
        }
    }
}
