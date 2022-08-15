using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemThrow : MonoBehaviour
{   
    public float m_throwDelay = 0.5f;
    private float m_throwTimer = 0.0f;
    private Vector3 m_throwVelocity = Vector3.zero;
    private Transform m_throwTransform;
    private bool m_isThrown = false;

    public float m_startScale = 0.0f;
    private Vector3 m_realScale = Vector3.zero;

    // Start is called before the first frame update
    void Awake()
    {
        // disable rigidbody and collider
        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<Collider>().enabled = false;

        // set scale
        m_realScale = transform.localScale;
        transform.localScale = Vector3.one * m_startScale;
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, m_realScale, Time.deltaTime * 2.0f);

        // while the timer is greater than 0, lerp to transform position
        if (m_throwTimer > 0.0f)
        {
            m_throwTimer -= Time.deltaTime;
            transform.position = Vector3.Lerp(transform.position, m_throwTransform.position, Time.deltaTime * 10.0f);

            // lerp scale
            //transform.localScale = Vector3.Lerp(transform.localScale, m_realScale, 1 - (m_throwTimer / m_throwDelay));
        }
        else if (!m_isThrown) {
            m_isThrown = true;

            // enable rigidbody and collider
            GetComponent<Rigidbody>().isKinematic = false;
            GetComponent<Collider>().enabled = true;

            // set scale
            //transform.localScale = m_realScale;

            // throw the item
            GetComponent<Rigidbody>().velocity = m_throwVelocity;
        }
    }

    public void TossPrefab(Transform _throwTransform, Vector3 _velocity, GameObject _owner)
    {
        m_throwTimer = m_throwDelay;
        m_throwTransform = _throwTransform;
        m_throwVelocity = _velocity;
    }
}
