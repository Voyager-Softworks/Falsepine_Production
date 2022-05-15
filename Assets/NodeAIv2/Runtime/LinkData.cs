using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NodeAI
{
    [System.Serializable]
    public class LinkData 
    {
        public string baseGUID;
        public string targetGUID;

        public string targetPortName;
    }
}
