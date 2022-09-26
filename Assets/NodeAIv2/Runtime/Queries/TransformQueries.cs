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


}
