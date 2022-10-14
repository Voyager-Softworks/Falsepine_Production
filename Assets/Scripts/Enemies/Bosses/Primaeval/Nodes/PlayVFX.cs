using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeAI;

namespace VFX
{
    public class PlayVFX : NodeAI.ActionBase
    {

        public PlayVFX()
        {
            AddProperty<VFXController>("VFX Controller", null);
            AddProperty<string>("VFX Name", null);
        }

        public override NodeData.State Eval(NodeAI_Agent agent, NodeTree.Leaf current)
        {
            var vfxController = GetProperty<VFXController>("VFX Controller");
            var vfxName = GetProperty<string>("VFX Name");

            vfxController.PlayVFX(vfxName);
            state = NodeData.State.Success;
            return NodeData.State.Success;
        }
    }
}