using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Rendering.HighDefinition;

/// <summary>
/// Class to manage rootwall collider behaviour
/// </summary>
public class Rootwall : MonoBehaviour
{
    bool solid = false;
    public float damage = 10f;
    public float duration = 2f;
    float timer = 0f;

    DecalProjector decalProjector;

    // Start is called before the first frame update
    void Start()
    {
        SetSemisolid();
        decalProjector = GetComponentInChildren<DecalProjector>();
        StartCoroutine(IndicatorDecal());
    }

    /// <summary>
    ///  Handles the indicator decal
    /// </summary>
    /// <returns></returns>
    IEnumerator IndicatorDecal()
    {
        decalProjector.enabled = true;
        float t = 0f;
        Vector3 endScale = new Vector3(45, 5, 1f);
        Vector3 startScale = new Vector3(45, 0, 1f);
        while (t < 1.0f)
        {
            t += Time.deltaTime;
            decalProjector.size = Vector3.Lerp(startScale, endScale, t);
            yield return null;
        }
        yield return new WaitForSeconds(1f);
        t = 0f;
        while (t < 0.5f)
        {
            t += Time.deltaTime;
            decalProjector.fadeFactor = Mathf.Lerp(1, 0, t * 2);
            yield return null;
        }
        decalProjector.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!solid) timer += Time.deltaTime;
        if (timer > duration)
        {
            SetSolid();
        }
    }

    /// <summary>
    /// Makes the rootwall colliders solid
    /// </summary>
    void SetSolid()
    {
        GetComponentsInChildren<Collider>().ToList().ForEach(c => c.isTrigger = false);
        solid = true;
    }

    /// <summary>
    /// Makes the rootwall colliders semisolid, pushing the player out of them
    /// </summary>
    public void SetSemisolid()
    {
        GetComponentsInChildren<Collider>().ToList().ForEach(c => c.isTrigger = true);
        solid = false;
        timer = 0f;
    }

    /// <summary>
    /// Damages the player when they move into the rootwall
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerEnter(Collider other)
    {
        //Push player outside of collider horizontally
        if (other.gameObject.tag == "Player")
        {
            Vector3 direction = other.transform.position - transform.position;
            direction.y = 0;
            direction.Normalize();
            //Damage player
            other.GetComponent<PlayerHealth>().TakeDamage(damage);

        }
    }

    /// <summary>
    ///  Pushed the player out of the rootwall
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            //Push player outside of collider horizontally
            Vector3 direction = other.transform.position - transform.position;
            direction.y = 0;
            direction.Normalize();
            bool isRight = Vector3.Dot(direction, transform.right) > 0;

            other.gameObject.GetComponent<CharacterController>().Move(isRight ? transform.right : -transform.right * 0.005f * Time.deltaTime);

        }
    }

}
