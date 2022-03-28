using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeAI;

[CreateAssetMenu(fileName = "New Custom State", menuName = "NodeAI/Custom State/TestCustomState")]
public class CustomStateTest : CustomState
{
    Transform agentTransform;
    GameObject agentGameObject;
    public override void DoCustomState(NodeAI_Agent agent)
    {
        Debug.Log("DoCustomState");
        agent.agent.SetDestination(agent.agent.transform.position + Vector3.forward * 10);
    }

    public override void OnStateEnter(NodeAI_Agent agent)
    {
        Debug.Log("OnStateEnter");
        agentTransform = agent.transform;
        agentGameObject = agent.gameObject;
    }

    public override void OnStateExit(NodeAI_Agent agent)
    {
        Debug.Log("OnStateExit");
    }


}
