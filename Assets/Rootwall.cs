using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// Class to manage rootwall collider behaviour
/// </summary>
public class Rootwall : MonoBehaviour
{
    bool solid = false;
    public float damage = 10f;
    public float duration = 2f;
    float timer = 0f;
    // Start is called before the first frame update
    void Start()
    {
        SetSemisolid();
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
