using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NodeAI
{
    public class NodeAI_Senses : MonoBehaviour
    {
        Vector3 eyesForward;
        public Transform eyesBone;

        public enum BoneDirection
        {
            Forward,
            Right,
            Left,
            Up,
            Back,
            Down
        }

        public BoneDirection eyeDirection;

        public float sightDistance = 10f;
        public float sightAngle = 90f;


        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            
        }



        public void OnDrawGizmos()
        {
            switch (eyeDirection)
            {
                case BoneDirection.Forward:
                    eyesForward = eyesBone.forward;
                    break;
                case BoneDirection.Right:
                    eyesForward = eyesBone.right;
                    break;
                case BoneDirection.Up:
                    eyesForward = eyesBone.up;
                    break;
                case BoneDirection.Back:
                    eyesForward = -eyesBone.forward;
                    break;
            }
            Gizmos.color = Color.yellow;
            CustomGizmos.DrawCone(eyesBone, eyesForward, sightAngle, sightDistance);
            
            
        }
    }
}
