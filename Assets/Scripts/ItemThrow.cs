using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script to handle throwing of items by the player.
///</summary>
public class ItemThrow : MonoBehaviour
{
    public float m_throwDelay = 0.5f;
    private float m_throwTimer = 0.0f;
    public float m_throwForce = 10.0f;
    public bool m_randomTorque = true;
    private Vector3 m_throwVelocity = Vector3.zero;
    private Transform m_throwTransform;
    public Quaternion m_throwRotation;
    public bool m_forceUpright = false;
    private bool m_isThrown = false;

    public float m_startScale = 0.0f;
    private Vector3 m_realScale = Vector3.zero;

    GameObject m_owner;

    // Start is called before the first frame update
    void Awake()
    {
        // disable rigidbody and collider
        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<Collider>().enabled = false;

        // set scale
        m_realScale = transform.localScale;
        transform.localScale = m_realScale * m_startScale;
    }

    // Update is called once per frame
    void Update()
    {

        // while the timer is greater than 0, lerp to transform position
        if (m_throwTimer > 0.0f)
        {
            m_throwTimer -= Time.deltaTime;
            transform.position = m_throwTransform.position; //Vector3.Lerp(transform.position, m_throwTransform.position, Time.deltaTime * 20.0f);
            transform.rotation = Quaternion.Lerp(transform.rotation, m_throwTransform.rotation * m_throwRotation, Time.deltaTime * 20.0f);

            // lerp scale
            transform.localScale = Vector3.Lerp(transform.localScale, m_realScale, Time.deltaTime * 1.0f);
        }
        else if (!m_isThrown)
        {
            m_isThrown = true;

            // enable rigidbody and collider
            GetComponent<Rigidbody>().isKinematic = false;
            GetComponent<Collider>().enabled = true;

            // set scale
            //transform.localScale = m_realScale;

            // throw the item
            m_throwVelocity = m_owner.transform.forward.normalized * m_throwForce;
            GetComponent<Rigidbody>().velocity = m_throwVelocity;
            //random rotation
            if (m_randomTorque) GetComponent<Rigidbody>().angularVelocity = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)) * 20.0f;
        }

        if (m_isThrown)
        {
            // lerp quickly to real scale
            transform.localScale = Vector3.Lerp(transform.localScale, m_realScale, Time.deltaTime * 10.0f);

            if (m_forceUpright)
            {
                transform.rotation = Quaternion.identity;
            }
        }
    }

    /// <summary>
    /// Tells the item to start the throw sequence.
    /// </summary>
    /// <param name="_throwTransform"></param>
    /// <param name="_direction"></param>
    /// <param name="_owner"></param>
    public void TossPrefab(Transform _throwTransform, Vector3 _direction, GameObject _owner)
    {
        m_throwTimer = m_throwDelay;
        m_throwTransform = _throwTransform;
        m_throwVelocity = _direction.normalized * m_throwForce;
        m_owner = _owner;
    }
}
