using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeAI;

namespace Boss.Omen
{
    public class Fly : NodeAI.ActionBase
    {
        RootMotionFlight flight; ///< The flight script.
        Animator animator; ///< The animator.

        bool init = false; ///< Whether or not the node has been initialized.

        public Fly()
        {
            AddProperty<Vector3>("Destination", Vector3.zero);
            AddProperty<float>("Speed", 1f);
            AddProperty<float>("AngularSpeed", 1f);
            AddProperty<float>("Acceleration", 1f);
            AddProperty<float>("StoppingDistance", 0.1f);
        }

        public override NodeData.State Eval(NodeAI_Agent agent, NodeTree.Leaf current)
        {
            if (flight == null)
            {
                flight = agent.GetComponent<RootMotionFlight>();
                if (flight == null)
                {
                    Debug.LogError("No RootMotionFlight script found on agent");
                    state = NodeData.State.Failure;
                    return state;
                }
            }
            if (animator == null)
            {
                animator = agent.GetComponentInChildren<Animator>();
                if (animator == null)
                {
                    Debug.LogError("No Animator found on agent");
                    state = NodeData.State.Failure;
                    return state;
                }
            }
            if (!init)
            {
                animator.SetTrigger("Fly");
                init = true;
            }

            Vector3 destination = GetProperty<Vector3>("Destination");
            float speed = GetProperty<float>("Speed");
            float angularSpeed = GetProperty<float>("AngularSpeed");
            float acceleration = GetProperty<float>("Acceleration");
            float stoppingDistance = GetProperty<float>("StoppingDistance");

            flight.SetDestination(destination);
            flight.speed = speed;
            flight.angularSpeed = angularSpeed;
            flight.acceleration = acceleration;
            flight.stoppingDistance = stoppingDistance;

            if (Vector3.Distance(agent.transform.position, destination) <= stoppingDistance)
            {
                animator.SetTrigger("Hover");
                state = NodeData.State.Success;
            }
            else
            {
                state = NodeData.State.Running;
            }
            return state;
        }

        public override void OnInit()
        {
            init = false;
        }

    }
}
