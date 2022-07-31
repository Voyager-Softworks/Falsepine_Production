using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeAI;

public class TestAction : NodeAI.ActionBase  /// @todo Comment
{
    public TestAction() /// @bug This is a test change to the bug to see if the auto doxygen github thing works
    {
        AddProperty<GameObject>("Target", null);
        AddProperty<Color>("Color", Color.white);
        AddProperty<AudioClip>("Audio", null);
        AddProperty<Transform>("Transform", null);
        AddProperty<Vector3>("Position", Vector3.zero);
        AddProperty<Animator>("Animator", null);
    }
    public override NodeData.State Eval(NodeAI_Agent agent, NodeTree.Leaf current)
    {
        Debug.Log(GetProperty<GameObject>("Target").name);
        return base.Eval(agent, current);
    }
}
