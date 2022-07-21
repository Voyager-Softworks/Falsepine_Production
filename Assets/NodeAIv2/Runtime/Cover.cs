/*
 * Bachelor of Software Engineering
 * Media Design School
 * Auckland
 * New Zealand
 * 
 * (c) 2022 Media Design School
 * 
 * File Name: Cover.cs
 * Description: 
 * Author: Nerys Thamm
 * Mail: nerysthamm@gmail.com
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is a script which is used as part of the demonstration of the NodeAIv2 system.
/// </summary>
/// <para>
/// It is used in the example scene to create objects which NodeAI_Agent's use as cover when engaging in the example behaviour.
/// </para>
public class Cover : MonoBehaviour
{
    public enum CoverType 
    {
        NONE,
        LOW,
        MEDIUM,
        HIGH
    }

    public CoverType coverType = CoverType.NONE; ///< The type of cover this object is.

    public Rect coverRect; ///< The rectange of the cover.

    List<CoverPoint> coverPoints = new List<CoverPoint>(); ///< The list of cover points in the cover.
    
    /// <summary>
    ///  A point surrounding the cover object where an agent may take cover.
    /// </summary>
    public struct CoverPoint 
    {
        public CoverPoint(Vector3 position, Vector3 direction)
        {
            this.position = position;
            this.direction = direction;
            this.taken = false;
        }
        public Vector3 position; ///< The position of the cover point.
        public bool taken; ///< Whether the cover point is taken or not.
        public Vector3 direction; ///< The direction of the cover point.
    }

    public float coverRadius = 1.0f; ///< The radius of the cover.

    int NumPointsPerSide{ get { return Mathf.RoundToInt(coverRect.width / (coverRadius * 2.0f)); } } ///< The number of points per side of the cover.

    /// <summary>
    ///  Gets all possible cover points for the cover.
    /// </summary>
    /// <param name="direction">Unit vector representing the direction from which the Agent wishes to take cover.</param>
    /// <returns>A list of all possible cover points.</returns>
    public List<Vector3> GetAvailableCoverPoints(Vector3 direction)
    {
        List<Vector3> points = new List<Vector3>();
        foreach (CoverPoint coverPoint in this.coverPoints)
        {
            if (!coverPoint.taken && Vector3.Dot(direction, coverPoint.direction) > 0.0f)
            {
                points.Add(coverPoint.position);
            }
        }
        return points;
    }

    //Get cover point positions
    void Awake()
    {
        coverPoints.Clear();

        Vector3 right = transform.right;
        Vector3 forward = transform.forward;

        for(int i = 0; i < NumPointsPerSide; i++)
        {

            Vector3 pos = transform.position - (right * coverRect.width / 2) + (right * (coverRect.width / NumPointsPerSide) * (i+0.5f));

            coverPoints.Add(new CoverPoint(pos + (forward * coverRadius), transform.forward));
            coverPoints.Add(new CoverPoint(pos - (forward * coverRadius), -transform.forward));


            
        }
    }

    void OnDrawGizmos()
    {


        Matrix4x4 rotationMatrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
        Gizmos.matrix = rotationMatrix;
        Gizmos.color = Color.red;
        Gizmos.DrawCube(Vector3.zero + (Vector3.up * coverRect.size.y * 0.5f), coverRect.size);
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(coverRect.width, 0, 3.0f));
        Gizmos.matrix = Matrix4x4.identity;
        
    }
}
