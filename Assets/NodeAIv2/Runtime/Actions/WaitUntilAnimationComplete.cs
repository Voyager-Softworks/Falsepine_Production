using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NodeAI
{
    public class WaitUntilAnimationComplete : ActionBase
    {
        Animator animator;
        public override NodeData.State Eval(NodeAI_Agent agent, NodeTree.Leaf current)
        {
            if (animator == null)
            {
                animator = agent.GetComponent<Animator>();
                if (animator == null)
                {
                    Debug.LogError("WaitUntilAnimationComplete: No animator found on agent");
                    state = NodeData.State.Failure;
                    return NodeData.State.Failure;
                }
            }
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
            {
                state = NodeData.State.Success;
                return NodeData.State.Success;
            }
            else
            {
                state = NodeData.State.Running;
                return NodeData.State.Running;
            }
        }
    }
}
public class WaitUntilAnimationComplete : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
