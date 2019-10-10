using DeepLearningFramework.Data;
using PerformanceWork.OptimizedNumerics;
using System;
using DeepLearningFramework.Core;

namespace DeepLearningFramework.Data.Operators.Terms
{
    public class MultiplyByNumber : Term
    {
        float v2;
        public override Dimension D1 { get; internal set; }
        public override Dimension D2 { get; internal set; }

        public MultiplyByNumber(Term v1, float v2)
        {
            Type = TermType.MultiplyByNumber;
            Terms = new Term[1] { v1 };
            this.v2 = v2;
            D1 = this.Terms[0].D1;
            D2 = this.Terms[0].D2;
        }

        public override void CalculateDerivate(MMDerivative s)
        {
            if (!D1.HardEquals(D1) || !D2.HardEquals(D2))
                throw new Exception("Terms should have an exact value!");
            s.MultiplyBy(v2);
            Terms[0].Derivate(s);
            s.DivideBy(v2);
        }

        internal override Matrix CalculateResult()
        {
            if (!D1.HardEquals(D1) || !D2.HardEquals(D2))
                throw new Exception("Terms should have an exact value!");
            return Terms[0].GetResult() * v2;
        }

    }
}
