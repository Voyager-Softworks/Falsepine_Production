using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NodeAI.Utility.Transforms
{
    public class Position : NodeAI.Query
    {
        public Position()
        {
            AddProperty<UnityEngine.Transform>("Transform", null, false);
            AddProperty<UnityEngine.Vector3>("Position", UnityEngine.Vector3.zero, true);
        }

        public override void GetNewValues(NodeAI_Agent agent)
        {
            if (GetProperty<UnityEngine.Transform>("Transform") == null)
            {
                SetProperty<UnityEngine.Vector3>("Position", UnityEngine.Vector3.zero);
                return;
            }
            SetProperty<UnityEngine.Vector3>("Position", GetProperty<UnityEngine.Transform>("Transform").position);
        }
    }

    public class Rotation : NodeAI.Query
    {
        public Rotation()
        {
            AddProperty<UnityEngine.Transform>("Transform", null, false);
            AddProperty<UnityEngine.Quaternion>("Rotation", UnityEngine.Quaternion.identity, true);
        }

        public override void GetNewValues(NodeAI_Agent agent)
        {
            if (GetProperty<UnityEngine.Transform>("Transform") == null)
            {
                SetProperty<UnityEngine.Quaternion>("Rotation", UnityEngine.Quaternion.identity);
                return;
            }
            SetProperty<UnityEngine.Quaternion>("Rotation", GetProperty<UnityEngine.Transform>("Transform").rotation);
        }
    }

    public class EulerRotation : NodeAI.Query
    {
        public EulerRotation()
        {
            AddProperty<UnityEngine.Transform>("Transform", null, false);
            AddProperty<UnityEngine.Vector3>("Rotation", UnityEngine.Vector3.zero, true);
        }

        public override void GetNewValues(NodeAI_Agent agent)
        {
            if (GetProperty<UnityEngine.Transform>("Transform") == null)
            {
                SetProperty<UnityEngine.Vector3>("Rotation", UnityEngine.Vector3.zero);
                return;
            }
            SetProperty<UnityEngine.Vector3>("Rotation", GetProperty<UnityEngine.Transform>("Transform").eulerAngles);
        }
    }

    public class Scale : NodeAI.Query
    {
        public Scale()
        {
            AddProperty<UnityEngine.Transform>("Transform", null, false);
            AddProperty<UnityEngine.Vector3>("Scale", UnityEngine.Vector3.one, true);
        }

        public override void GetNewValues(NodeAI_Agent agent)
        {
            if (GetProperty<UnityEngine.Transform>("Transform") == null)
            {
                SetProperty<UnityEngine.Vector3>("Scale", UnityEngine.Vector3.one);
                return;
            }
            SetProperty<UnityEngine.Vector3>("Scale", GetProperty<UnityEngine.Transform>("Transform").localScale);
        }
    }

    public class Forward : NodeAI.Query
    {
        public Forward()
        {
            AddProperty<UnityEngine.Transform>("Transform", null, false);
            AddProperty<UnityEngine.Vector3>("Forward", UnityEngine.Vector3.forward, true);
        }

        public override void GetNewValues(NodeAI_Agent agent)
        {
            if (GetProperty<UnityEngine.Transform>("Transform") == null)
            {
                SetProperty<UnityEngine.Vector3>("Forward", UnityEngine.Vector3.forward);
                return;
            }
            SetProperty<UnityEngine.Vector3>("Forward", GetProperty<UnityEngine.Transform>("Transform").forward);
        }
    }

    public class Right : NodeAI.Query
    {
        public Right()
        {
            AddProperty<UnityEngine.Transform>("Transform", null, false);
            AddProperty<UnityEngine.Vector3>("Right", UnityEngine.Vector3.right, true);
        }

        public override void GetNewValues(NodeAI_Agent agent)
        {
            if (GetProperty<UnityEngine.Transform>("Transform") == null)
            {
                SetProperty<UnityEngine.Vector3>("Right", UnityEngine.Vector3.right);
                return;
            }
            SetProperty<UnityEngine.Vector3>("Right", GetProperty<UnityEngine.Transform>("Transform").right);
        }
    }

    public class Up : NodeAI.Query
    {
        public Up()
        {
            AddProperty<UnityEngine.Transform>("Transform", null, false);
            AddProperty<UnityEngine.Vector3>("Up", UnityEngine.Vector3.up, true);
        }

        public override void GetNewValues(NodeAI_Agent agent)
        {
            if (GetProperty<UnityEngine.Transform>("Transform") == null)
            {
                SetProperty<UnityEngine.Vector3>("Up", UnityEngine.Vector3.up);
                return;
            }
            SetProperty<UnityEngine.Vector3>("Up", GetProperty<UnityEngine.Transform>("Transform").up);
        }
    }

    public class ThisTransform : NodeAI.Query
    {
        public ThisTransform()
        {
            AddProperty<UnityEngine.Transform>("Transform", null, true);
        }

        public override void GetNewValues(NodeAI_Agent agent)
        {
            SetProperty<UnityEngine.Transform>("Transform", agent.transform);
        }
    }


}
