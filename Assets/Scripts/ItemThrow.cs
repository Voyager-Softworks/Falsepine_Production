using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script to handle throwing of items by the player.
///</summary>
public class ItemThrow : MonoBehaviour
{
    public bool m_randomTorque = true;
    private Transform m_throwTransform;
    public Quaternion m_throwRotation;
    public bool m_forceUpright = false;
    private bool m_isThrown = false;

    public float m_startScale = 0.0f;
    private Vector3 m_realScale = Vector3.zero;

    [Header("Throw Stats")]
    public float m_upVelocity = 2.0f;
    public float m_maxThrowDistance = 15.0f;

    GameObject m_owner;

    private PlayerInventoryInterface m_inventoryInterface;

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

    private void OnDisable() {
        if (m_inventoryInterface != null && m_inventoryInterface.m_currentlyThrowingItem == this)
        {
            m_inventoryInterface.m_currentlyThrowingItem = null;
        }
    }

    // Update is called once per frame
    void Update()
    {

        // while the timer is greater than 0, lerp to transform position
        if (!m_isThrown)
        {
            transform.position = m_throwTransform.position; //Vector3.Lerp(transform.position, m_throwTransform.position, Time.deltaTime * 20.0f);
            transform.rotation = Quaternion.Lerp(transform.rotation, m_throwTransform.rotation * m_throwRotation, Time.deltaTime * 20.0f);

            // lerp scale
            transform.localScale = Vector3.Lerp(transform.localScale, m_realScale, Time.deltaTime * 1.0f);

            // find and bind player inventory interface
            if (m_inventoryInterface == null)
            {
                m_inventoryInterface = m_owner.GetComponent<PlayerInventoryInterface>();
            }
            if (m_inventoryInterface != null)
            {
                m_inventoryInterface.m_currentlyThrowingItem = this;
            }
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

    public void DoThrow()
    {
        m_isThrown = true;

        // unbine from player inventory interface
        if (m_inventoryInterface != null)
        {
            m_inventoryInterface.m_currentlyThrowingItem = null;
        }

        // enable rigidbody and collider
        GetComponent<Rigidbody>().isKinematic = false;
        GetComponent<Collider>().enabled = true;

        // set scale
        //transform.localScale = m_realScale;

        // throw the item AT the mouse
        PlayerMovement pm = FindObjectOfType<PlayerMovement>();
        if (pm != null)
        {
            // get mouse pos
            Vector3 mousePos = pm.GetMouseAimPlanePoint();

            // get start position
            Vector3 startPos = m_throwTransform.position;

            // get horizontal distance (excluding y)
            Vector3 horizontalDistance = new Vector3(mousePos.x - startPos.x, 0, mousePos.z - startPos.z);
            // clamp
            horizontalDistance = Vector3.ClampMagnitude(horizontalDistance, m_maxThrowDistance);

            // get initial vertical velocity (scale by distance, 0-1)
            float initialVelocity = m_upVelocity * (horizontalDistance.magnitude / m_maxThrowDistance);

            // get acceleration due to gravity
            float gravity = -Physics.gravity.y;

            // calculate time to reach apex
            float timeToApex = initialVelocity / gravity;

            // height of apex
            float apexHeight = startPos.y + initialVelocity * timeToApex - 0.5f * gravity * timeToApex * timeToApex;

            // calc fall distance
            float fallDistance = apexHeight - mousePos.y;
            // ensure never below 0 for next equation
            fallDistance = Mathf.Max(0, fallDistance);

            // calculate time from apex to mouse height
            float timeToMouse = Mathf.Sqrt(2 * (fallDistance) / gravity);

            // calculate total time
            float totalTime = timeToApex + timeToMouse;

            // calculate horizontal velocity to reach mouse
            Vector3 horizontalVelocity = horizontalDistance / totalTime;

            // set velocity
            GetComponent<Rigidbody>().velocity = horizontalVelocity + Vector3.up * initialVelocity;
        }

        //random rotation
        if (m_randomTorque) GetComponent<Rigidbody>().angularVelocity = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)) * 20.0f;
    }

    /// <summary>
    /// Tells the item to start the throw sequence.
    /// </summary>
    /// <param name="_throwTransform"></param>
    /// <param name="_direction"></param>
    /// <param name="_owner"></param>
    public void StartThrow(Transform _throwTransform, Vector3 _direction, GameObject _owner)
    {
        m_throwTransform = _throwTransform;
        m_owner = _owner;

        // find and bind player inventory interface
        if (m_inventoryInterface == null)
        {
            m_inventoryInterface = m_owner.GetComponent<PlayerInventoryInterface>();
        }
        if (m_inventoryInterface != null)
        {
            m_inventoryInterface.m_currentlyThrowingItem = this;
        }
    }
}
