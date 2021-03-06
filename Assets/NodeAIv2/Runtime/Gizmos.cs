/*
 * Bachelor of Software Engineering
 * Media Design School
 * Auckland
 * New Zealand
 * 
 * (c) 2022 Media Design School
 * 
 * File Name: Gizmos.cs
 * Description: 
 * Author: Nerys Thamm
 * Mail: nerysthamm@gmail.com
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NodeAI
{
    /// <summary>
    /// A class used to draw Custom Gizmos used by NodeAI.
    /// </summary>
    public class CustomGizmos
    {
        /// <summary>
        ///  Draw a Ray Gizmo.
        /// </summary>
        /// <param name="start">The startpoint of the ray.</param>
        /// <param name="dir">A unit vector representing the direction of the ray.</param>
        /// <param name="length">The length of the ray.</param>
        public static void DrawRay(Vector3 start, Vector3 dir, float length = 1f)
        {
            Debug.DrawRay(start, dir * length, Color.yellow);
        }

        /// <summary>
        ///  Draw a Cone Gizmo.
        /// </summary>
        /// <param name="start">The start point of the cone.</param>
        /// <param name="dir">A unit vector representing the direction of the cone.</param>
        /// <param name="angle">The interior angle of the cone's point.</param>
        /// <param name="length">The length of the cone.</param>
        /// <remarks>
        /// This version of the method uses a Vector3 to determine its origin.
        /// </remarks>
        public static void DrawCone(Vector3 start, Vector3 dir, float angle, float length = 1f)
        {
            Gizmos.color = Color.yellow;
            Vector3 coneRay1 = Quaternion.AngleAxis(angle/2, Vector3.up) * dir;
            Vector3 coneRay2 = Quaternion.AngleAxis(angle/3, Vector3.up) * dir;
            Vector3 coneRay3 = Quaternion.AngleAxis(angle/4, Vector3.up) * dir;
            Vector3 coneRay4 = Quaternion.AngleAxis(angle/5, Vector3.up) * dir;
            Vector3 coneRay5 = Quaternion.AngleAxis(angle/6, Vector3.up) * dir;
            Vector3 forwardRay = start + (dir.normalized * length);
            for (int i = 0; i < 20; i++)
            {
                Gizmos.DrawRay(start, coneRay1.normalized * length);
                Gizmos.DrawLine((coneRay1.normalized * length) + start, (coneRay2.normalized * length) + start);
                Gizmos.DrawLine((coneRay2.normalized * length) + start, (coneRay3.normalized * length) + start);
                Gizmos.DrawLine((coneRay3.normalized * length) + start, (coneRay4.normalized * length) + start);
                Gizmos.DrawLine((coneRay4.normalized * length) + start, (coneRay5.normalized * length) + start);
                Gizmos.DrawLine((coneRay5.normalized * length) + start, forwardRay);
                Vector3 temp = coneRay1;
                coneRay1 = Quaternion.AngleAxis(18.0f, dir) * coneRay1;
                coneRay2 = Quaternion.AngleAxis(18.0f, dir) * coneRay2;
                coneRay3 = Quaternion.AngleAxis(18.0f, dir) * coneRay3;
                coneRay4 = Quaternion.AngleAxis(18.0f, dir) * coneRay4;
                coneRay5 = Quaternion.AngleAxis(18.0f, dir) * coneRay5;
                Gizmos.DrawLine((temp.normalized * length) + start, (coneRay1.normalized * length) + start);
            }
        }

        /// <summary>
        ///  Draw a Cone Gizmo.
        /// </summary>
        /// <param name="start">The start point of the cone.</param>
        /// <param name="dir">A unit vector representing the direction of the cone.</param>
        /// <param name="angle">The interior angle of the cone's point.</param>
        /// <param name="length">The length of the cone.</param>
        /// <remarks>
        /// This version of the method uses a Transform to determine its origin.
        /// </remarks>
        public static void DrawCone(Transform startTransform, Vector3 dir, float angle, float length = 1f)
        {
            Gizmos.color = Color.yellow;
            Vector3 start = startTransform.position;
            Vector3 coneRay1 = Quaternion.AngleAxis(angle/2, Vector3.Cross(startTransform.right, dir)) * dir;
            Vector3 coneRay2 = Quaternion.AngleAxis(angle/3, Vector3.Cross(startTransform.right, dir)) * dir;
            Vector3 coneRay3 = Quaternion.AngleAxis(angle/4, Vector3.Cross(startTransform.right, dir)) * dir;
            Vector3 coneRay4 = Quaternion.AngleAxis(angle/5, Vector3.Cross(startTransform.right, dir)) * dir;
            Vector3 coneRay5 = Quaternion.AngleAxis(angle/6, Vector3.Cross(startTransform.right, dir)) * dir;
            Vector3 forwardRay = start + (dir.normalized * length);
            for (int i = 0; i < 20; i++)
            {
                Gizmos.DrawRay(start, coneRay1.normalized * length);
                Gizmos.DrawLine((coneRay1.normalized * length) + start, (coneRay2.normalized * length) + start);
                Gizmos.DrawLine((coneRay2.normalized * length) + start, (coneRay3.normalized * length) + start);
                Gizmos.DrawLine((coneRay3.normalized * length) + start, (coneRay4.normalized * length) + start);
                Gizmos.DrawLine((coneRay4.normalized * length) + start, (coneRay5.normalized * length) + start);
                Gizmos.DrawLine((coneRay5.normalized * length) + start, forwardRay);
                Vector3 temp = coneRay1;
                coneRay1 = Quaternion.AngleAxis(18.0f, dir) * coneRay1;
                coneRay2 = Quaternion.AngleAxis(18.0f, dir) * coneRay2;
                coneRay3 = Quaternion.AngleAxis(18.0f, dir) * coneRay3;
                coneRay4 = Quaternion.AngleAxis(18.0f, dir) * coneRay4;
                coneRay5 = Quaternion.AngleAxis(18.0f, dir) * coneRay5;
                Gizmos.DrawLine((temp.normalized * length) + start, (coneRay1.normalized * length) + start);
            }
        }
    }
}


