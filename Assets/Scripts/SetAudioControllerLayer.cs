using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeAI;

namespace Audio
{
    /// <summary>
    ///  Node to set the current layer of an AudioController
    /// </summary>
    /// <remarks>
    ///  This is used by boss enemies to add layers to their boss music as they change stage.
    /// </remarks>
    public class SetAudioControllerLayer : NodeAI.ActionBase
    {
        AudioController audioController; //The AudioController to set the layer of

        /// <summary>
        ///  Constructor
        /// </summary>
        public SetAudioControllerLayer()
        {
            AddProperty<GameObject>("AudioController", null);
            AddProperty<string>("Channel Name", "");
            AddProperty<int>("Layer Index", 0);

        }

        /// <summary>
        ///  Sets the layer of the AudioController
        /// </summary>
        /// <param name="agent"></param>
        /// <param name="current"></param>
        /// <returns></returns>
        public override NodeData.State Eval(NodeAI_Agent agent, NodeTree.Leaf current)
        {
            if (audioController == null)
            {
                audioController = GetProperty<GameObject>("AudioController").GetComponent<AudioController>(); //Get the AudioController from the GameObject
            }
            int index = audioController.GetChannelIndex(GetProperty<string>("Channel Name")); //Get the index of the channel
            audioController.audioChannels[index].layerIndex = GetProperty<int>("Layer Index"); //Set the layer index of the channel
            state = NodeData.State.Success;
            return state;
        }
    }
}
