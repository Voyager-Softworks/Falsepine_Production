using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NodeAI
{
    public abstract class CustomState : ScriptableObject
    {
        public virtual void DoCustomState(NodeAI_Agent agent){}
        public virtual void OnStateEnter(NodeAI_Agent agent){}
        public virtual void OnStateExit(NodeAI_Agent agent){}
    }
}
