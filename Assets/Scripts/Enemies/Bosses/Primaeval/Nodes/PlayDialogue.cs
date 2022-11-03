using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeAI;

public class PlayDialogue : NodeAI.ActionBase
{
    bool init = false;
    public PlayDialogue()
    {
        tooltip = "Plays Dialogue for the Primaeval";
    }

    public override NodeData.State Eval(NodeAI_Agent agent, NodeTree.Leaf current)
    {
        if (!init)
        {
            agent.GetComponentInChildren<PrimaevalDialogue>().BeginDialogue();
            init = true;
        }
        if (!agent.GetComponentInChildren<PrimaevalDialogue>().dialogueActive)
        {
            init = false;
            state = NodeData.State.Success;
            return NodeData.State.Success;
        }
        state = NodeData.State.Running;
        return NodeData.State.Running;
    }
}
