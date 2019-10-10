using DeepLearningFramework.Data;
using PerformanceWork.OptimizedNumerics;
using System;
using DeepLearningFramework.Core;

namespace DeepLearningFramework.Data.Operators.Terms
{
    public class Minus : Term
    {
        public override Dimension D1 { get; internal set; }
        public override Dimension D2 { get; internal set; }

        public Minus(Term v1, Term v2)
        {
            Type = TermType.Minus;
            Terms = new Term[2] { v1, v2 };
            if (!this.Terms[0].D1.SoftEquals(this.Terms[1].D1) || !this.Terms[0].D2.SoftEquals(this.Terms[1].D2))
                throw new Exception("Terms to be sum should have the same dimensions!");
            D1 = this.Terms[0].D1;
            D2 = this.Terms[0].D2;
        }

        public override void CalculateDerivate(MMDerivative s)
        {
            if (!this.Terms[0].D1.HardEquals(this.Terms[1].D1) || !this.Terms[0].D2.HardEquals(this.Terms[1].D2))
                throw new Exception("Terms to be sum should have the same dimensions!");
            Terms[0].Derivate(s);
            //when we go out, MMDerivative s should remain the same as it was.
            s.Negative = !s.Negative;
            Terms[1].Derivate(s);
            s.Negative = !s.Negative;
        }

        internal override Matrix CalculateResult()
        {
            if (!this.Terms[0].D1.HardEquals(this.Terms[1].D1) || !this.Terms[0].D2.HardEquals(this.Terms[1].D2))
                throw new Exception("Terms to be sum should have the same dimensions!");
            return Terms[0].GetResult() - Terms[1].GetResult();
        }

    }
}
