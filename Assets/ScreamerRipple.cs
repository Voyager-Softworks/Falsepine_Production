using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class ScreamerRipple : MonoBehaviour
{
    public float range = 7.5f;
    public float duration = 2f;
    public float damage = 10f;
    public float damageInterval = 0.5f;
    float damageTimer = 0f;
    DecalProjector decalProjector;
    public AnimationCurve opacityCurve;
    float timer = 0f;
    bool doDamage = true;


    // Start is called before the first frame update
    void Start()
    {
        decalProjector = GetComponentInChildren<DecalProjector>();
        decalProjector.size = new Vector3(range * 2, range * 2, 1f);
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > duration)
        {
            Destroy(gameObject, 2f);
            doDamage = false;
        }
        else
        {

            damageTimer += Time.deltaTime;
            if (damageTimer > damageInterval && doDamage)
            {
                damageTimer = 0f;
                Collider[] colliders = Physics.OverlapSphere(transform.position, range);
                foreach (Collider collider in colliders)
                {
                    if (collider.gameObject.tag == "Player")
                    {
                        collider.gameObject.GetComponent<PlayerHealth>().TakeDamage(damage);
                    }
                }
            }
        }
        float t = timer / duration;
        decalProjector.fadeFactor = opacityCurve.Evaluate(t);
    }
}
