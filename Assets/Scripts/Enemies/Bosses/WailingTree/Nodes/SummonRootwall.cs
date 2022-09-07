using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeAI;

/// <summary>
/// The Wailing Tree Boss Enemy
/// </summary>
namespace Boss.WailingTree
{
    /// <summary>
    ///  A node for the Wailing Tree bosses rootwall summon attack.
    /// </summary>
    public class SummonRootwall : NodeAI.ActionBase
    {
        public SummonRootwall()
        {
            AddProperty<int>("Count", 1);
            AddProperty<float>("Delay", 0.5f);
        }
        float timer = 0;
        int spawned = 0;
        float lastTime = 0;
        public override NodeData.State Eval(NodeAI_Agent agent, NodeTree.Leaf current)
        {
            if (timer == 0)
            {
                timer = GetProperty<float>("Delay");
                spawned = 0;
                lastTime = Time.time;
            }
            if (Time.time - lastTime > timer)
            {
                lastTime = Time.time;
                FindObjectOfType<RootwallManager>().SpawnRootwall();
                spawned++;
                if (spawned >= GetProperty<int>("Count"))
                {
                    timer = 0;
                    state = NodeData.State.Success;
                    return NodeData.State.Success;
                }
            }
            state = NodeData.State.Running;
            return NodeData.State.Running;
        }

        public override void OnInit()
        {
            timer = 0;
            spawned = 0;
            lastTime = Time.time;
        }
    }
}

