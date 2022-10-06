using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeAI;

namespace NodeAI.Audio
{
    public class SetMusicTrigger : NodeAI.ActionBase
    {
        AudioController audioController;
        public SetMusicTrigger()
        {
            AddProperty<GameObject>("AudioController", null);
            AddProperty<string>("Trigger", "");
        }

        public override NodeData.State Eval(NodeAI_Agent agent, NodeTree.Leaf current)
        {
            if (GetProperty<string>("Trigger") != "")
            {
                if (audioController == null)
                {
                    audioController = GetProperty<GameObject>("AudioController").GetComponent<AudioController>();
                }
                if (audioController != null)
                {
                    audioController.Trigger(GetProperty<string>("Trigger"));
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
