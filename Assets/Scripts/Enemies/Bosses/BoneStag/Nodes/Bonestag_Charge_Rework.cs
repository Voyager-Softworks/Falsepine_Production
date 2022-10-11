using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeAI;
using UnityEngine.Animations.Rigging;

namespace Boss.Bonestag
{
    /// <summary>
    /// 
    /// </summary> @todo comment
    public class Bonestag_Charge_Rework : NodeAI.ActionBase
    {

        Vector3 targetLineA, targetLineB;
        Vector3 playerPos;
        Vector3 targetPos;
        Transform startTransform;
        float speed;
        bool hitPlayer = false;
        bool hasInit = false;

        public Bonestag_Charge_Rework()
        {
            tooltip = "Charges at the player";
            AddProperty<float>("Speed", 0f);
            AddProperty<GameObject>("Player", null);
            AddProperty<Transform>("Target Line Begin", null);
            AddProperty<Transform>("Target Line End", null);
            AddProperty<Transform>("Tunnel pos A", null);
            AddProperty<Transform>("Tunnel pos B", null);
            AddProperty<Transform>("Tunnel pos C", null);
            AddProperty<bool>("Interrupt", false);
            AddProperty<float>("Damage Radius", 0f);
            AddProperty<float>("Damage", 0f);
        }

        public override NodeData.State Eval(NodeAI_Agent agent, NodeTree.Leaf current)
        {
            if (!hasInit)
            {
                hasInit = true;
                agent.transform.position = startTransform.position;
                agent.transform.rotation = startTransform.rotation;
                targetPos = GetGoalPos(agent.transform, playerPos);
            }
            if (GetProperty<bool>("Interrupt"))
            {
                state = NodeData.State.Failure;
                return state;
            }
            if (Vector3.Distance(agent.transform.position, targetPos) < 1.0f)
            {
                agent.transform.position = targetPos;
                state = NodeData.State.Success;
                return state;
            }
            agent.transform.position = Vector3.MoveTowards(agent.transform.position, targetPos, speed * Time.deltaTime);
            agent.transform.rotation = Quaternion.Slerp(agent.transform.rotation, Quaternion.LookRotation(targetPos - agent.transform.position), 0.1f);
            if (!hitPlayer && Vector3.Distance(agent.transform.position, playerPos) < GetProperty<float>("Damage Radius"))
            {
                hitPlayer = true;
                GetProperty<GameObject>("Player").GetComponent<PlayerHealth>().TakeDamage(GetProperty<float>("Damage"));
            }
            return state;
        }

        public override void OnInit()
        {
            speed = GetProperty<float>("Speed");
            playerPos = GetProperty<GameObject>("Player").transform.position;
            targetLineA = GetProperty<Transform>("Target Line Begin").position;
            targetLineB = GetProperty<Transform>("Target Line End").position;
            // Set start transform to closest tunnel pos to player
            float distA = Vector3.Distance(playerPos, GetProperty<Transform>("Tunnel pos A").position);
            float distB = Vector3.Distance(playerPos, GetProperty<Transform>("Tunnel pos B").position);
            float distC = Vector3.Distance(playerPos, GetProperty<Transform>("Tunnel pos C").position);
            if (distA < distB && distA < distC)
            {
                startTransform = GetProperty<Transform>("Tunnel pos A");
            }
            else if (distB < distA && distB < distC)
            {
                startTransform = GetProperty<Transform>("Tunnel pos B");
            }
            else
            {
                startTransform = GetProperty<Transform>("Tunnel pos C");
            }
            hitPlayer = false;
            hasInit = false;
        }

        Vector3 GetGoalPos(Transform agentPos, Vector3 playerPos)
        {
            // Find the point on the line segment that is closest to the ray defined by the agent and the player
            Vector3 lineSegment = targetLineB - targetLineA;
            Vector3 lineOrigin = targetLineA;

            Vector3 ray = playerPos - agentPos.position;
            Vector3 rayOrigin = agentPos.position;

            Vector3 agentToLineA = targetLineA - agentPos.position;
            Vector3 agentToLineB = targetLineB - agentPos.position;

            float dotA = Vector3.Dot(agentToLineA, ray);
            float dotB = Vector3.Dot(agentToLineB, ray);

            // Get relative closeness to each dot product
            float dotAWeight = dotA / (dotA + dotB);
            float dotBWeight = dotB / (dotA + dotB);

            // Calculate a lerp value based on the relative closeness
            float lerpValue = dotAWeight * 0.0f + dotBWeight * 1.0f;

            // Get the point on the line segment that is closest to the ray
            Vector3 closestPoint = Vector3.Lerp(lineOrigin, lineOrigin + lineSegment, lerpValue);

            return closestPoint;

        }
    }
}

