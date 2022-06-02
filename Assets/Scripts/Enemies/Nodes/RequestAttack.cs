using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeAI;
/// <summary>
///  This class is used to represent a node in the node tree.
///  This node is used to request the ability to attack a target via its MobTarget script.
/// </summary>
public class RequestAttack : NodeAI.ActionBase
{
    float lastAttackTime = 0;
    public RequestAttack()
    {
        AddProperty<GameObject>("Target", null);
        AddProperty<float>("Attack cooldown", 1);
    }
    public override NodeData.State Eval(NodeAI_Agent agent, NodeTree.Leaf current)
    {
        if(GetProperty<GameObject>("Target") == null)
        {
            state = NodeData.State.Failure;
            return NodeData.State.Failure;
        }
        if(Time.time - lastAttackTime > GetProperty<float>("Attack cooldown") && GetProperty<GameObject>("Target").GetComponent<MobTarget>() && GetProperty<GameObject>("Target").GetComponent<MobTarget>().RegisterAttacker(agent))
        {
            lastAttackTime = Time.time;
            state = NodeData.State.Success;
            return NodeData.State.Success;
        }
        state = NodeData.State.Failure;
        return NodeData.State.Failure;

    }


}
