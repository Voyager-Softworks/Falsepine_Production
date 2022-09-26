using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NodeAI.Math.Vectors
{
    namespace Split
    {
        public class SplitVector3 : NodeAI.Query
        {
            public SplitVector3()
            {
                AddProperty<UnityEngine.Vector3>("Vector", UnityEngine.Vector3.zero, false);
                AddProperty<float>("X", 0f, true);
                AddProperty<float>("Y", 0f, true);
                AddProperty<float>("Z", 0f, true);
            }

            public override void GetNewValues(NodeAI_Agent agent)
            {
                UnityEngine.Vector3 vector = GetProperty<UnityEngine.Vector3>("Vector");
                SetProperty<float>("X", vector.x);
                SetProperty<float>("Y", vector.y);
                SetProperty<float>("Z", vector.z);
            }
        }

        public class SplitVector2 : NodeAI.Query
        {
            public SplitVector2()
            {
                AddProperty<UnityEngine.Vector2>("Vector", UnityEngine.Vector2.zero, false);
                AddProperty<float>("X", 0f, true);
                AddProperty<float>("Y", 0f, true);
            }

            public override void GetNewValues(NodeAI_Agent agent)
            {
                UnityEngine.Vector2 vector = GetProperty<UnityEngine.Vector2>("Vector");
                SetProperty<float>("X", vector.x);
                SetProperty<float>("Y", vector.y);
            }
        }

        public class SplitVector4 : NodeAI.Query
        {
            public SplitVector4()
            {
                AddProperty<UnityEngine.Vector4>("Vector", UnityEngine.Vector4.zero, false);
                AddProperty<float>("X", 0f, true);
                AddProperty<float>("Y", 0f, true);
                AddProperty<float>("Z", 0f, true);
                AddProperty<float>("W", 0f, true);
            }

            public override void GetNewValues(NodeAI_Agent agent)
            {
                UnityEngine.Vector4 vector = GetProperty<UnityEngine.Vector4>("Vector");
                SetProperty<float>("X", vector.x);
                SetProperty<float>("Y", vector.y);
                SetProperty<float>("Z", vector.z);
                SetProperty<float>("W", vector.w);
            }
        }
    }
    namespace Combine
    {
        public class CombineVector3 : NodeAI.Query
        {
            public CombineVector3()
            {
                AddProperty<float>("X", 0f, false);
                AddProperty<float>("Y", 0f, false);
                AddProperty<float>("Z", 0f, false);
                AddProperty<UnityEngine.Vector3>("Vector", UnityEngine.Vector3.zero, true);
            }

            public override void GetNewValues(NodeAI_Agent agent)
            {
                SetProperty<UnityEngine.Vector3>("Vector", new UnityEngine.Vector3(GetProperty<float>("X"), GetProperty<float>("Y"), GetProperty<float>("Z")));
            }
        }

        public class CombineVector2 : NodeAI.Query
        {
            public CombineVector2()
            {
                AddProperty<float>("X", 0f, false);
                AddProperty<float>("Y", 0f, false);
                AddProperty<UnityEngine.Vector2>("Vector", UnityEngine.Vector2.zero, true);
            }

            public override void GetNewValues(NodeAI_Agent agent)
            {
                SetProperty<UnityEngine.Vector2>("Vector", new UnityEngine.Vector2(GetProperty<float>("X"), GetProperty<float>("Y")));
            }
        }

        public class CombineVector4 : NodeAI.Query
        {
            public CombineVector4()
            {
                AddProperty<float>("X", 0f, false);
                AddProperty<float>("Y", 0f, false);
                AddProperty<float>("Z", 0f, false);
                AddProperty<float>("W", 0f, false);
                AddProperty<UnityEngine.Vector4>("Vector", UnityEngine.Vector4.zero, true);
            }

            public override void GetNewValues(NodeAI_Agent agent)
            {
                SetProperty<UnityEngine.Vector4>("Vector", new UnityEngine.Vector4(GetProperty<float>("X"), GetProperty<float>("Y"), GetProperty<float>("Z"), GetProperty<float>("W")));
            }
        }
    }

    namespace Magnitude
    {
        public class MagnitudeVector3 : NodeAI.Query
        {
            public MagnitudeVector3()
            {
                AddProperty<UnityEngine.Vector3>("Vector", UnityEngine.Vector3.zero, false);
                AddProperty<float>("Magnitude", 0f, true);
            }

            public override void GetNewValues(NodeAI_Agent agent)
            {
                SetProperty<float>("Magnitude", GetProperty<UnityEngine.Vector3>("Vector").magnitude);
            }
        }

        public class MagnitudeVector2 : NodeAI.Query
        {
            public MagnitudeVector2()
            {
                AddProperty<UnityEngine.Vector2>("Vector", UnityEngine.Vector2.zero, false);
                AddProperty<float>("Magnitude", 0f, true);
            }

            public override void GetNewValues(NodeAI_Agent agent)
            {
                SetProperty<float>("Magnitude", GetProperty<UnityEngine.Vector2>("Vector").magnitude);
            }
        }

        public class MagnitudeVector4 : NodeAI.Query
        {
            public MagnitudeVector4()
            {
                AddProperty<UnityEngine.Vector4>("Vector", UnityEngine.Vector4.zero, false);
                AddProperty<float>("Magnitude", 0f, true);
            }

            public override void GetNewValues(NodeAI_Agent agent)
            {
                SetProperty<float>("Magnitude", GetProperty<UnityEngine.Vector4>("Vector").magnitude);
            }
        }
    }

    namespace Normalize
    {
        public class NormalizeVector3 : NodeAI.Query
        {
            public NormalizeVector3()
            {
                AddProperty<UnityEngine.Vector3>("Vector", UnityEngine.Vector3.zero, false);
                AddProperty<UnityEngine.Vector3>("Normalized", UnityEngine.Vector3.zero, true);
            }

            public override void GetNewValues(NodeAI_Agent agent)
            {
                SetProperty<UnityEngine.Vector3>("Normalized", GetProperty<UnityEngine.Vector3>("Vector").normalized);
            }
        }

        public class NormalizeVector2 : NodeAI.Query
        {
            public NormalizeVector2()
            {
                AddProperty<UnityEngine.Vector2>("Vector", UnityEngine.Vector2.zero, false);
                AddProperty<UnityEngine.Vector2>("Normalized", UnityEngine.Vector2.zero, true);
            }

            public override void GetNewValues(NodeAI_Agent agent)
            {
                SetProperty<UnityEngine.Vector2>("Normalized", GetProperty<UnityEngine.Vector2>("Vector").normalized);
            }
        }

        public class NormalizeVector4 : NodeAI.Query
        {
            public NormalizeVector4()
            {
                AddProperty<UnityEngine.Vector4>("Vector", UnityEngine.Vector4.zero, false);
                AddProperty<UnityEngine.Vector4>("Normalized", UnityEngine.Vector4.zero, true);
            }

            public override void GetNewValues(NodeAI_Agent agent)
            {
                SetProperty<UnityEngine.Vector4>("Normalized", GetProperty<UnityEngine.Vector4>("Vector").normalized);
            }
        }
    }

    namespace Distance
    {
        public class DistanceVector3 : NodeAI.Query
        {
            public DistanceVector3()
            {
                AddProperty<UnityEngine.Vector3>("A", UnityEngine.Vector3.zero, false);
                AddProperty<UnityEngine.Vector3>("B", UnityEngine.Vector3.zero, false);
                AddProperty<float>("Distance", 0f, true);
            }

            public override void GetNewValues(NodeAI_Agent agent)
            {
                SetProperty<float>("Distance", UnityEngine.Vector3.Distance(GetProperty<UnityEngine.Vector3>("A"), GetProperty<UnityEngine.Vector3>("B")));
            }
        }

        public class DistanceVector2 : NodeAI.Query
        {
            public DistanceVector2()
            {
                AddProperty<UnityEngine.Vector2>("A", UnityEngine.Vector2.zero, false);
                AddProperty<UnityEngine.Vector2>("B", UnityEngine.Vector2.zero, false);
                AddProperty<float>("Distance", 0f, true);
            }

            public override void GetNewValues(NodeAI_Agent agent)
            {
                SetProperty<float>("Distance", UnityEngine.Vector2.Distance(GetProperty<UnityEngine.Vector2>("A"), GetProperty<UnityEngine.Vector2>("B")));
            }
        }

        public class DistanceVector4 : NodeAI.Query
        {
            public DistanceVector4()
            {
                AddProperty<UnityEngine.Vector4>("A", UnityEngine.Vector4.zero, false);
                AddProperty<UnityEngine.Vector4>("B", UnityEngine.Vector4.zero, false);
                AddProperty<float>("Distance", 0f, true);
            }

            public override void GetNewValues(NodeAI_Agent agent)
            {
                SetProperty<float>("Distance", UnityEngine.Vector4.Distance(GetProperty<UnityEngine.Vector4>("A"), GetProperty<UnityEngine.Vector4>("B")));
            }
        }
    }

    namespace Dot
    {
        public class DotVector3 : NodeAI.Query
        {
            public DotVector3()
            {
                AddProperty<UnityEngine.Vector3>("A", UnityEngine.Vector3.zero, false);
                AddProperty<UnityEngine.Vector3>("B", UnityEngine.Vector3.zero, false);
                AddProperty<float>("Dot", 0f, true);
            }

            public override void GetNewValues(NodeAI_Agent agent)
            {
                SetProperty<float>("Dot", UnityEngine.Vector3.Dot(GetProperty<UnityEngine.Vector3>("A"), GetProperty<UnityEngine.Vector3>("B")));
            }
        }

        public class DotVector2 : NodeAI.Query
        {
            public DotVector2()
            {
                AddProperty<UnityEngine.Vector2>("A", UnityEngine.Vector2.zero, false);
                AddProperty<UnityEngine.Vector2>("B", UnityEngine.Vector2.zero, false);
                AddProperty<float>("Dot", 0f, true);
            }

            public override void GetNewValues(NodeAI_Agent agent)
            {
                SetProperty<float>("Dot", UnityEngine.Vector2.Dot(GetProperty<UnityEngine.Vector2>("A"), GetProperty<UnityEngine.Vector2>("B")));
            }
        }

        public class DotVector4 : NodeAI.Query
        {
            public DotVector4()
            {
                AddProperty<UnityEngine.Vector4>("A", UnityEngine.Vector4.zero, false);
                AddProperty<UnityEngine.Vector4>("B", UnityEngine.Vector4.zero, false);
                AddProperty<float>("Dot", 0f, true);
            }

            public override void GetNewValues(NodeAI_Agent agent)
            {
                SetProperty<float>("Dot", UnityEngine.Vector4.Dot(GetProperty<UnityEngine.Vector4>("A"), GetProperty<UnityEngine.Vector4>("B")));
            }
        }
    }

    namespace Angle
    {
        public class AngleVector3 : NodeAI.Query
        {
            public AngleVector3()
            {
                AddProperty<UnityEngine.Vector3>("A", UnityEngine.Vector3.zero, false);
                AddProperty<UnityEngine.Vector3>("B", UnityEngine.Vector3.zero, false);
                AddProperty<float>("Angle", 0f, true);
            }

            public override void GetNewValues(NodeAI_Agent agent)
            {
                SetProperty<float>("Angle", UnityEngine.Vector3.Angle(GetProperty<UnityEngine.Vector3>("A"), GetProperty<UnityEngine.Vector3>("B")));
            }
        }

        public class AngleVector2 : NodeAI.Query
        {
            public AngleVector2()
            {
                AddProperty<UnityEngine.Vector2>("A", UnityEngine.Vector2.zero, false);
                AddProperty<UnityEngine.Vector2>("B", UnityEngine.Vector2.zero, false);
                AddProperty<float>("Angle", 0f, true);
            }

            public override void GetNewValues(NodeAI_Agent agent)
            {
                SetProperty<float>("Angle", UnityEngine.Vector2.Angle(GetProperty<UnityEngine.Vector2>("A"), GetProperty<UnityEngine.Vector2>("B")));
            }
        }
    }

    namespace Cross
    {
        public class CrossVector3 : NodeAI.Query
        {
            public CrossVector3()
            {
                AddProperty<UnityEngine.Vector3>("A", UnityEngine.Vector3.zero, false);
                AddProperty<UnityEngine.Vector3>("B", UnityEngine.Vector3.zero, false);
                AddProperty<UnityEngine.Vector3>("Cross", UnityEngine.Vector3.zero, true);
            }

            public override void GetNewValues(NodeAI_Agent agent)
            {
                SetProperty<UnityEngine.Vector3>("Cross", UnityEngine.Vector3.Cross(GetProperty<UnityEngine.Vector3>("A"), GetProperty<UnityEngine.Vector3>("B")));
            }
        }
    }

    namespace Lerp
    {
        public class LerpVector3 : NodeAI.Query
        {
            public LerpVector3()
            {
                AddProperty<UnityEngine.Vector3>("A", UnityEngine.Vector3.zero, false);
                AddProperty<UnityEngine.Vector3>("B", UnityEngine.Vector3.zero, false);
                AddProperty<float>("T", 0f, false);
                AddProperty<UnityEngine.Vector3>("Lerp", UnityEngine.Vector3.zero, true);
            }

            public override void GetNewValues(NodeAI_Agent agent)
            {
                SetProperty<UnityEngine.Vector3>("Lerp", UnityEngine.Vector3.Lerp(GetProperty<UnityEngine.Vector3>("A"), GetProperty<UnityEngine.Vector3>("B"), GetProperty<float>("T")));
            }
        }

        public class LerpVector2 : NodeAI.Query
        {
            public LerpVector2()
            {
                AddProperty<UnityEngine.Vector2>("A", UnityEngine.Vector2.zero, false);
                AddProperty<UnityEngine.Vector2>("B", UnityEngine.Vector2.zero, false);
                AddProperty<float>("T", 0f, false);
                AddProperty<UnityEngine.Vector2>("Lerp", UnityEngine.Vector2.zero, true);
            }

            public override void GetNewValues(NodeAI_Agent agent)
            {
                SetProperty<UnityEngine.Vector2>("Lerp", UnityEngine.Vector2.Lerp(GetProperty<UnityEngine.Vector2>("A"), GetProperty<UnityEngine.Vector2>("B"), GetProperty<float>("T")));
            }
        }

        public class LerpVector4 : NodeAI.Query
        {
            public LerpVector4()
            {
                AddProperty<UnityEngine.Vector4>("A", UnityEngine.Vector4.zero, false);
                AddProperty<UnityEngine.Vector4>("B", UnityEngine.Vector4.zero, false);
                AddProperty<float>("T", 0f, false);
                AddProperty<UnityEngine.Vector4>("Lerp", UnityEngine.Vector4.zero, true);
            }

            public override void GetNewValues(NodeAI_Agent agent)
            {
                SetProperty<UnityEngine.Vector4>("Lerp", UnityEngine.Vector4.Lerp(GetProperty<UnityEngine.Vector4>("A"), GetProperty<UnityEngine.Vector4>("B"), GetProperty<float>("T")));
            }
        }
    }

    namespace Scalar
    {
        public class ScalarVector3 : NodeAI.Query
        {
            public ScalarVector3()
            {
                AddProperty<UnityEngine.Vector3>("A", UnityEngine.Vector3.zero, false);
                AddProperty<UnityEngine.Vector3>("B", UnityEngine.Vector3.zero, false);
                AddProperty<float>("Scalar", 0f, true);
            }

            public override void GetNewValues(NodeAI_Agent agent)
            {
                SetProperty<float>("Scalar", UnityEngine.Vector3.SqrMagnitude(GetProperty<UnityEngine.Vector3>("A") - GetProperty<UnityEngine.Vector3>("B")));
            }
        }

        public class ScalarVector2 : NodeAI.Query
        {
            public ScalarVector2()
            {
                AddProperty<UnityEngine.Vector2>("A", UnityEngine.Vector2.zero, false);
                AddProperty<UnityEngine.Vector2>("B", UnityEngine.Vector2.zero, false);
                AddProperty<float>("Scalar", 0f, true);
            }

            public override void GetNewValues(NodeAI_Agent agent)
            {
                SetProperty<float>("Scalar", UnityEngine.Vector2.SqrMagnitude(GetProperty<UnityEngine.Vector2>("A") - GetProperty<UnityEngine.Vector2>("B")));
            }
        }

        public class ScalarVector4 : NodeAI.Query
        {
            public ScalarVector4()
            {
                AddProperty<UnityEngine.Vector4>("A", UnityEngine.Vector4.zero, false);
                AddProperty<UnityEngine.Vector4>("B", UnityEngine.Vector4.zero, false);
                AddProperty<float>("Scalar", 0f, true);
            }

            public override void GetNewValues(NodeAI_Agent agent)
            {
                SetProperty<float>("Scalar", UnityEngine.Vector4.SqrMagnitude(GetProperty<UnityEngine.Vector4>("A") - GetProperty<UnityEngine.Vector4>("B")));
            }
        }
    }
    namespace Arithmetic
    {
        namespace Add
        {
            public class AddVector3 : NodeAI.Query
            {
                public AddVector3()
                {
                    AddProperty<UnityEngine.Vector3>("A", UnityEngine.Vector3.zero, false);
                    AddProperty<UnityEngine.Vector3>("B", UnityEngine.Vector3.zero, false);
                    AddProperty<UnityEngine.Vector3>("Add", UnityEngine.Vector3.zero, true);
                }

                public override void GetNewValues(NodeAI_Agent agent)
                {
                    SetProperty<UnityEngine.Vector3>("Add", GetProperty<UnityEngine.Vector3>("A") + GetProperty<UnityEngine.Vector3>("B"));
                }
            }

            public class AddVector2 : NodeAI.Query
            {
                public AddVector2()
                {
                    AddProperty<UnityEngine.Vector2>("A", UnityEngine.Vector2.zero, false);
                    AddProperty<UnityEngine.Vector2>("B", UnityEngine.Vector2.zero, false);
                    AddProperty<UnityEngine.Vector2>("Add", UnityEngine.Vector2.zero, true);
                }

                public override void GetNewValues(NodeAI_Agent agent)
                {
                    SetProperty<UnityEngine.Vector2>("Add", GetProperty<UnityEngine.Vector2>("A") + GetProperty<UnityEngine.Vector2>("B"));
                }
            }

            public class AddVector4 : NodeAI.Query
            {
                public AddVector4()
                {
                    AddProperty<UnityEngine.Vector4>("A", UnityEngine.Vector4.zero, false);
                    AddProperty<UnityEngine.Vector4>("B", UnityEngine.Vector4.zero, false);
                    AddProperty<UnityEngine.Vector4>("Add", UnityEngine.Vector4.zero, true);
                }

                public override void GetNewValues(NodeAI_Agent agent)
                {
                    SetProperty<UnityEngine.Vector4>("Add", GetProperty<UnityEngine.Vector4>("A") + GetProperty<UnityEngine.Vector4>("B"));
                }
            }
        }
        namespace Subtract
        {
            public class SubtractVector3 : NodeAI.Query
            {
                public SubtractVector3()
                {
                    AddProperty<UnityEngine.Vector3>("A", UnityEngine.Vector3.zero, false);
                    AddProperty<UnityEngine.Vector3>("B", UnityEngine.Vector3.zero, false);
                    AddProperty<UnityEngine.Vector3>("Subtract", UnityEngine.Vector3.zero, true);
                }

                public override void GetNewValues(NodeAI_Agent agent)
                {
                    SetProperty<UnityEngine.Vector3>("Subtract", GetProperty<UnityEngine.Vector3>("A") - GetProperty<UnityEngine.Vector3>("B"));
                }
            }

            public class SubtractVector2 : NodeAI.Query
            {
                public SubtractVector2()
                {
                    AddProperty<UnityEngine.Vector2>("A", UnityEngine.Vector2.zero, false);
                    AddProperty<UnityEngine.Vector2>("B", UnityEngine.Vector2.zero, false);
                    AddProperty<UnityEngine.Vector2>("Subtract", UnityEngine.Vector2.zero, true);
                }

                public override void GetNewValues(NodeAI_Agent agent)
                {
                    SetProperty<UnityEngine.Vector2>("Subtract", GetProperty<UnityEngine.Vector2>("A") - GetProperty<UnityEngine.Vector2>("B"));
                }
            }

            public class SubtractVector4 : NodeAI.Query
            {
                public SubtractVector4()
                {
                    AddProperty<UnityEngine.Vector4>("A", UnityEngine.Vector4.zero, false);
                    AddProperty<UnityEngine.Vector4>("B", UnityEngine.Vector4.zero, false);
                    AddProperty<UnityEngine.Vector4>("Subtract", UnityEngine.Vector4.zero, true);
                }

                public override void GetNewValues(NodeAI_Agent agent)
                {
                    SetProperty<UnityEngine.Vector4>("Subtract", GetProperty<UnityEngine.Vector4>("A") - GetProperty<UnityEngine.Vector4>("B"));
                }
            }
        }

    }


}
