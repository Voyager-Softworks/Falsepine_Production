using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeAI;

namespace NodeAI.Audio
{
    public class SetMusicTrigger : NodeAI.ActionBase
    {
        public SetMusicTrigger()
        {
            AddProperty<string>("Trigger", "");
        }

        public override NodeData.State Eval(NodeAI_Agent agent, NodeTree.Leaf current)
        {
            if (GetProperty<string>("Trigger") != "")
            {
                MusicManager musicManager = GameObject.FindObjectOfType<MusicManager>();
                if (musicManager != null)
                {
                    musicManager.Trigger(GetProperty<string>("Trigger"));
                    state = NodeData.State.Success;
                    return NodeData.State.Success;
                }
                else
                {
                    Debug.LogError("SetMusicTrigger: MusicManager not found");
                    state = NodeData.State.Failure;
                    return NodeData.State.Failure;
                }
            }
            Debug.LogError("SetMusicTrigger: Trigger is empty!");
            state = NodeData.State.Failure;
            return NodeData.State.Failure;
        }
    }
}
