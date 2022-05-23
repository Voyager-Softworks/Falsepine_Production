using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeAI;

namespace Boss.Bonestag
{
    public class Bonestag_CheckPhaseChange : NodeAI.ConditionBase
    {
        public Bonestag_CheckPhaseChange()
        {
            AddProperty<bool>("IsSecondPhase", false);
            AddProperty<float>("Phase transition threshold", 0.5f);
        }

        public override NodeData.State Eval(NodeAI_Agent agent, NodeTree.Leaf current)
        {
            if (!GetProperty<bool>("IsSecondPhase"))
            {
                if (agent.GetComponent<HealthScript>().currentHealth <= GetProperty<float>("Phase transition threshold"))
                {
                    SetProperty<bool>("IsSecondPhase", true);
                    agent.SetParameter("SecondPhase", true);
                    FindObjectOfType<BossArenaController>().StartSecondPhase();
                    state = NodeData.State.Success;
                    return NodeData.State.Success;
                }
            }
            state = NodeData.State.Failure;
            return NodeData.State.Failure;
            
        }


    }
}
