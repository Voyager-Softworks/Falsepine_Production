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
        float flyStopDuration = 3f;
        float flyStopTimer = 0.0f; ///< The timer for when to stop flying.
        bool reachedDestination = false; ///< Whether the boss has reached its destination.
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
            if (!reachedDestination)
            {
                flight.SetDestination(destination);
                flight.speed = speed;
                flight.angularSpeed = angularSpeed;
                flight.acceleration = acceleration;
                flight.stoppingDistance = stoppingDistance;

                if (Vector3.Distance(agent.transform.position, destination) <= stoppingDistance)
                {
                    animator.SetTrigger("Hover");
                    reachedDestination = true;
                    state = NodeData.State.Running;
                }
                else
                {
                    state = NodeData.State.Running;
                }
            }
            else
            {
                if (flyStopTimer < flyStopDuration)
                {
                    flyStopTimer += Time.deltaTime;
                    agent.transform.position = Vector3.Lerp(agent.transform.position, destination, flyStopTimer / flyStopDuration);
                    state = NodeData.State.Running;
                }
                else
                {
                    state = NodeData.State.Success;
                }
            }
            return state;
        }

        public override void OnInit()
        {
            init = false;
            reachedDestination = false;
            flyStopTimer = 0.0f;
        }

    }
}
