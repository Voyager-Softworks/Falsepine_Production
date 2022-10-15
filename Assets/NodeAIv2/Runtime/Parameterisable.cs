using System;
using NodeAI;

namespace NodeAI
{
    [System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Struct)]
    public class Parameterisable : System.Attribute
    {
        public Parameterisable()
        {

        }
    }
}