using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeAI;

[CreateAssetMenu(fileName = "New Custom State", menuName = "NodeAI/Custom State/BossEscapeBearTrapState")]
public class BossEscapeBearTrapState : NodeAI.CustomState
{
    public override void OnStateEnter(NodeAI_Agent agent)
    {
        agent.agent.velocity =  Vector3.zero;
        agent.agent.isStopped = true;
    }

    public override void OnStateExit(NodeAI_Agent agent)
    {
        agent.SetBool("HitBearTrap", false);
    }

    public override void DrawStateGizmos(NodeAI_Agent agent)
    {
        Gizmos.color = Color.red;
    }
}
