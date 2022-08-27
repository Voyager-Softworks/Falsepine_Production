using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script to allow characters to dynamically vault over obstacles in the scene.
/// </summary>
public class DynamicVaulting : MonoBehaviour
{
    public int sphereCastCount = 10; ///< The number of spheres to cast to check for obstacles.
    public float sphereCastRadius = 0.5f; ///< The radius of the spheres to cast to check for obstacles.
    public float sphereCastDistance = 1f; ///< The distance of the spheres to cast to check for obstacles.
    public LayerMask layerMask; ///< The layer mask to use to check for obstacles.
    public float maxVaultingHeight = 1f; ///< The maximum height to vault over.
    public float maxVaultingDepth = 1f; ///< The maximum depth to vault over.
    public float maxVaultingAngle = 45f; ///< The maximum angle to vault over.
    //public float maxVaultingDistance = 1f; ///< The maximum distance to vault over.
    public float verticalOffset = 0.5f; ///< The vertical offset to apply to the vaulting position.

    private Vector3[] sphereCastPositions; ///< The positions of the spheres to cast to check for obstacles.
    private RaycastHit[] sphereCastHits; ///< The hits of the spheres to cast to check for obstacles.
    private Vector3 horizontalHit; ///< The hit of the horizontal raycast to check for obstacles.
    private Vector3 verticalHit; ///< The hit of the vertical raycast to check for obstacles.
    public Vector3 vaultingHit; ///< The hit of the vaulting raycast to check for obstacles.

    public bool canVault = false; ///< Whether or not the player can vault.

    public AnimationCurve vaultingCurve, vaultingHeightCurve; ///< The curve to use to calculate the vaulting movement.

    /// <summary>
    ///  Gets the height of the vaulting position.
    /// </summary>
    /// <returns> The height of the vaulting position. </returns>
    public float GetVaultingHeight()
    {
        return vaultingHit.y;
    }
    /// <summary>
    ///  Gets the Direction to vault over.
    /// </summary>
    /// <returns> The Direction to vault over. </returns>
    public Vector3 GetVaultingDirection()
    {
        Vector3 dir = horizontalHit - transform.position;
        dir.y = 0;
        return dir;
    }

    // Start is called before the first frame update
    void Start()
    {
        sphereCastHits = new RaycastHit[sphereCastCount];
    }

    // Update is called once per frame
    void Update()
    {
        canVault = CalculateVaultData();
    }

    /// <summary>
    ///  Calculates data related to vaulting.
    /// </summary>
    /// <returns>True if the character can vault, false otherwise.</returns>
    /// <remarks>
    ///  This method detects whether there is an object in front of the player which can be vaulted over, and finds the point on the object
    ///  which can be grabbed by the character.
    /// </remarks>
    public bool CalculateVaultData()
    {
        sphereCastPositions = new Vector3[sphereCastCount];
        sphereCastHits = new RaycastHit[sphereCastCount];
        for (int i = 0; i < sphereCastCount; i++)
        {
            sphereCastPositions[i] = transform.position + (transform.up * (maxVaultingHeight / sphereCastCount) * i) + (transform.up * verticalOffset);
            Physics.SphereCast(sphereCastPositions[i], sphereCastRadius, transform.forward, out sphereCastHits[i], sphereCastDistance, layerMask);
        }
        if (sphereCastHits[sphereCastCount - 1].collider != null)
        {
            horizontalHit = Vector3.zero;
            verticalHit = Vector3.zero;
            vaultingHit = Vector3.zero;
            return false;
        }
        Vector3 hitNormal = Vector3.zero;
        for (int i = sphereCastCount - 2; i >= 0; i--)
        {
            horizontalHit = Vector3.zero;
            if (sphereCastHits[i].collider != null)
            {
                horizontalHit = sphereCastHits[i].point;
                hitNormal = sphereCastHits[i].normal;
                break;
            }
        }
        if (horizontalHit == Vector3.zero)
        {
            vaultingHit = Vector3.zero;
            return false;
        }
        //Check if the normal is opposite to the forward vector within the max angle
        if (Vector3.Angle(new Vector3(hitNormal.x, 0.0f, hitNormal.z), -transform.forward) > maxVaultingAngle)
        {
            vaultingHit = Vector3.zero;
            horizontalHit = Vector3.zero;
            return false;
        }


        RaycastHit verticalHitInfo;
        Physics.Raycast(new Vector3(horizontalHit.x, transform.position.y + maxVaultingHeight + verticalOffset, horizontalHit.z), -transform.up, out verticalHitInfo, maxVaultingHeight, layerMask);
        if (verticalHitInfo.collider == null)
        {
            vaultingHit = Vector3.zero;
            horizontalHit = Vector3.zero;
            return false;
        }
        verticalHit = verticalHitInfo.point;
        vaultingHit.x = horizontalHit.x;
        vaultingHit.y = verticalHit.y;
        vaultingHit.z = horizontalHit.z;
        if (Physics.OverlapSphere(vaultingHit + (transform.forward * (maxVaultingDepth + sphereCastRadius)), sphereCastRadius).Length > 0)
        {
            vaultingHit = Vector3.zero;
            horizontalHit = Vector3.zero;
            verticalHit = Vector3.zero;
            return false;
        }
        return true;

    }

    /// <summary>
    ///  Draws the debug lines for the vaulting system.
    /// </summary>
    private void OnDrawGizmos()
    {
        if (sphereCastHits == null)
        {
            sphereCastHits = new RaycastHit[sphereCastCount];
        }
        if (!Application.isPlaying) canVault = CalculateVaultData();
        for (int i = 0; i < sphereCastCount; i++)
        {
            Gizmos.color = canVault ? Color.blue : Color.red;
            Gizmos.DrawSphere(sphereCastPositions[i], sphereCastRadius);
        }
        for (int i = 0; i < sphereCastCount; i++)
        {
            if (sphereCastHits[i].collider != null)
            {
                Gizmos.DrawLine(sphereCastPositions[i], sphereCastHits[i].point);
                Gizmos.DrawSphere(sphereCastHits[i].point, sphereCastRadius);
            }
            else
            {
                Gizmos.DrawLine(sphereCastPositions[i], sphereCastPositions[i] + (transform.forward * sphereCastDistance));
            }
        }
        if (horizontalHit != Vector3.zero)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, horizontalHit);
            Gizmos.DrawSphere(horizontalHit, sphereCastRadius);
            if (verticalHit != Vector3.zero)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(new Vector3(horizontalHit.x, transform.position.y + maxVaultingHeight + verticalOffset, horizontalHit.z), verticalHit);
                Gizmos.DrawSphere(verticalHit, sphereCastRadius);
                if (vaultingHit != Vector3.zero)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawLine(horizontalHit, vaultingHit);
                    Gizmos.DrawLine(verticalHit, vaultingHit);
                    Gizmos.DrawSphere(vaultingHit, sphereCastRadius);
                    Gizmos.DrawSphere(vaultingHit + (transform.forward * (maxVaultingDepth + sphereCastRadius)), sphereCastRadius);
                }
            }
        }

        Gizmos.color = Color.white;
    }
}
