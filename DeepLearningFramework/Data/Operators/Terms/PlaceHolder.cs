using PerformanceWork.OptimizedNumerics;
using System;
using System.Collections.Generic;
using System.Text;
using DeepLearningFramework.Core;

namespace DeepLearningFramework.Data.Operators.Terms
{
    public class PlaceHolder : Term
    {
        public override Dimension D1 { get; internal set; }
        public override Dimension D2 { get; internal set; }

        public PlaceHolder(int Size)
        {
            Type = TermType.PlaceHolder;
            D1 = Size;
            D2 = new Dimension();
            Terms = new Term[1];
        }

        public void SetVariable(Variable v)
        {
            if (Terms[0] != null)
            {
                Variable v11 = (Variable)Terms[0];
                if (!v11.Weights.Returned)
                    v11.Weights.Dispose();
                v11 = null;
                Terms[0] = null;
            }
            Terms[0] = v;
            if (!D1.HardEquals(Terms[0].D1))
                throw new Exception("Placeholder dimension!");
            D2.Value = v.D2.Value;
        }

        public override void CalculateDerivate(MMDerivative s)
        {
            if (!D1.HardEquals(D1) || !D2.HardEquals(D2))
                throw new Exception("Terms should have an exact value!");
            Terms[0].Derivate(s);
        }

        internal override Matrix CalculateResult()
        {
            if (!D1.HardEquals(D1) || !D2.HardEquals(D2))
                throw new Exception("Terms should have an exact value!");
            return Terms[0].GetResult();
        }

    }
}
