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
    Vector3 decalSizeInitial;
    Vector3 decalSizeFinal;
    bool hasDamaged = false;

    // Start is called before the first frame update
    void Start()
    {
        decalSizeInitial = new Vector3(0.0f, 0.0f, 2.0f);
        decalSizeFinal = new Vector3(damageRadius * 2, damageRadius * 2, 2.0f);
        decalProjector.size = decalSizeInitial;
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
            decalProjector.enabled = false;
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
                Destroy(gameObject, 3f);
            }
        }
    }
}
