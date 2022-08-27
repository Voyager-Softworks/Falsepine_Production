using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class RollingBoulder : MonoBehaviour
{
    [SerializeField] Vector3 rollDirection;
    [SerializeField] float rollSpeed;
    [SerializeField] float rotationMultiplier;

    [SerializeField] GameObject vfxObject;
    [SerializeField] GameObject destroyParticle;
    [SerializeField] float damage = 10f;

    public AnimationCurve initialPositionCurve = new AnimationCurve();

    bool isRolling = false;

    [SerializeField] Mesh[] boulderMeshes;

    IEnumerator PopUpCoroutine()
    {
        Vector3 finalPosition = transform.position;
        Vector3 initialPosition = finalPosition - Vector3.up;
        float t = 0;
        while (t < 0.5)
        {
            t += Time.deltaTime;
            transform.position = Vector3.Lerp(initialPosition, finalPosition, initialPositionCurve.Evaluate(t * 2));
            yield return null;
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<MeshFilter>().mesh = boulderMeshes[Random.Range(0, boulderMeshes.Length)];

        //Set random rotation on all axes
        transform.rotation = Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));


        StartCoroutine(PopUpCoroutine());
    }

    public void Roll(Vector3 direction)
    {
        rollDirection = direction.normalized;
        rollDirection.y = 0;
        rollDirection *= 10.0f;
        isRolling = true;
    }

    // Update is called once per frame
    void Update()
    {
        vfxObject.transform.rotation = Quaternion.LookRotation(rollDirection, Vector3.up);
        if (!isRolling)
        {
            return;
        }
        Vector3 rollEuler = new Vector3(rollDirection.z * rollSpeed, rollDirection.y * rollSpeed, -rollDirection.x * rollSpeed);
        // Rotate the boulder
        transform.Rotate(rollEuler * rollSpeed * rotationMultiplier * Time.deltaTime, Space.World);


        // Move the boulder
        transform.Translate(rollDirection * rollSpeed * Time.deltaTime, Space.World);




    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("IgnoreRaycast") || !isRolling) return;
        if (other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponent<PlayerHealth>().TakeDamage(damage);
        }
        if ((other.ClosestPoint(transform.position) - transform.position).y >= 0.0f)
        {
            if (destroyParticle != null)
            {
                Instantiate(destroyParticle, transform.position, Quaternion.LookRotation(other.transform.position - transform.position));
            }
            gameObject.GetComponent<MeshRenderer>().enabled = false;
            gameObject.GetComponent<Collider>().enabled = false;
            gameObject.GetComponentInChildren<VisualEffect>().Stop();
            isRolling = false;
            Destroy(gameObject, 10.0f);
        }

    }
}
