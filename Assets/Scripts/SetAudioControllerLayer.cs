using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeAI;

namespace Audio
{
    public class SetAudioControllerLayer : NodeAI.ActionBase
    {
        AudioController audioController;

        public SetAudioControllerLayer()
        {
            AddProperty<GameObject>("AudioController", null);
            AddProperty<string>("Channel Name", "");
            AddProperty<int>("Layer Index", 0);

        }

        public override NodeData.State Eval(NodeAI_Agent agent, NodeTree.Leaf current)
        {
            if (audioController == null)
            {
                audioController = GetProperty<GameObject>("AudioController").GetComponent<AudioController>();
            }
            int index = audioController.GetChannelIndex(GetProperty<string>("Channel Name"));
            audioController.audioChannels[index].layerIndex = GetProperty<int>("Layer Index");
            state = NodeData.State.Success;
            return state;
        }
    }
}
