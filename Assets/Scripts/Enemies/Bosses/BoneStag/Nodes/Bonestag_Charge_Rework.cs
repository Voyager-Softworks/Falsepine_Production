using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeAI;

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
        float speed;
        bool hitPlayer = false;

        public Bonestag_Charge_Rework()
        {
            tooltip = "Charges at the player";
            AddProperty<float>("Speed", 0f);
            AddProperty<GameObject>("Player", null);
            AddProperty<Transform>("Target Line Begin", null);
            AddProperty<Transform>("Target Line End", null);
            AddProperty<bool>("Interrupt", false);
            AddProperty<float>("Damage Radius", 0f);
            AddProperty<float>("Damage", 0f);
        }

        public override NodeData.State Eval(NodeAI_Agent agent, NodeTree.Leaf current)
        {
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
            targetPos = GetGoalPos();
            hitPlayer = false;
        }

        Vector3 GetGoalPos()
        {
            //find the point on the target line closest to the player
            Vector3 playerToLineA = playerPos - targetLineA;
            Vector3 lineDir = targetLineB - targetLineA;
            float lineLength = lineDir.magnitude;
            lineDir.Normalize();
            float dot = Vector3.Dot(playerToLineA, lineDir);
            float closestPoint = Mathf.Clamp(dot, 0, lineLength);
            Vector3 closestPointOnLine = targetLineA + lineDir * closestPoint;
            return closestPointOnLine;
        }
    }
}

