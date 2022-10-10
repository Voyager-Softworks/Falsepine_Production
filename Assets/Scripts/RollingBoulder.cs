using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

/// <summary>
///  Manages the rolling boulders spawned by <see cref="SummonBoulders"/>, after they are rolled by the boss using <see cref="ThrowBoulder"/>
/// </summary> 
public class RollingBoulder : MonoBehaviour
{
    [SerializeField] Vector3 rollDirection; // The direction the boulder will roll in
    [SerializeField] float rollSpeed; // The speed at which the boulder will roll
    [SerializeField] float rotationMultiplier; // The multiplier for the rotation of the boulder

    [SerializeField] GameObject vfxObject; // The visual effect object that will be spawned when the boulder hits the ground
    [SerializeField] GameObject destroyParticle; // The particle effect that will be spawned when the boulder is destroyed
    [SerializeField] float damage = 10f; // The amount of damage the boulder will deal to the player

    [Header("Screenshake: Rolling")]
    [SerializeField] float screenshakeDuration = 0.5f; // The duration of the screenshake
    [SerializeField] Vector3 screenshakeAmplitude = Vector3.one; // The amplitude of the screenshake
    [SerializeField] float screenshakeFrequency = 1f; // The frequency of the screenshake

    [Header("Screenshake: Impact")]
    [SerializeField] float impactScreenshakeDuration = 0.5f; // The duration of the screenshake
    [SerializeField] Vector3 impactScreenshakeAmplitude = Vector3.one; // The amplitude of the screenshake
    [SerializeField] float impactScreenshakeFrequency = 1f; // The frequency of the screenshake

    ScreenshakeManager screenshakeManager; // The screenshake manager


    public AnimationCurve initialPositionCurve = new AnimationCurve(); // The curve that will be used to determine the initial position of the boulder

    bool isRolling = false; // Whether the boulder is rolling or not

    [SerializeField] Mesh[] boulderMeshes; // The meshes that will be used for the boulder

    /// <summary>
    ///  Called when the boulder is spawned
    /// </summary>
    /// <returns></returns>
    IEnumerator PopUpCoroutine()
    {
        Vector3 finalPosition = transform.position + (Vector3.up * 0.5f);

        Vector3 initialPosition = finalPosition - (Vector3.up * 4f);
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
        screenshakeManager = FindObjectOfType<ScreenshakeManager>();

        //Set random rotation on all axes
        transform.rotation = Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));


        StartCoroutine(PopUpCoroutine());
    }

    /// <summary>
    ///  Method to roll the boulder in a given direction
    /// </summary>
    /// <param name="direction"></param>
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
        screenshakeManager.AddShakeImpulse(screenshakeDuration, screenshakeAmplitude, screenshakeFrequency);
        Vector3 rollEuler = new Vector3(rollDirection.z * rollSpeed, rollDirection.y * rollSpeed, -rollDirection.x * rollSpeed);
        // Rotate the boulder
        transform.Rotate(rollEuler * rollSpeed * rotationMultiplier * Time.deltaTime, Space.World);


        // Move the boulder
        transform.Translate(rollDirection * rollSpeed * Time.deltaTime, Space.World);




    }

    /// <summary>
    ///  Called when the boulder hits something
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerEnter(Collider other)
    {
        // Check if the boulder hit the ground
        if (other.gameObject.layer == LayerMask.NameToLayer("IgnoreRaycast") || !isRolling || other.gameObject.layer == LayerMask.NameToLayer("Floor")) return;
        // Check if the boulder hit the player
        if (other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponent<PlayerHealth>().TakeDamage(damage); // Deal damage to the player
        }
        // Check if the boulder hit a surface which is a wall as found by its surface normal
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
            screenshakeManager.AddShakeImpulse(impactScreenshakeDuration, impactScreenshakeAmplitude, impactScreenshakeFrequency);
            Destroy(gameObject, 10.0f);
        }

    }
}
