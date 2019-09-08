using DeepLearningFramework.Data;
using PerformanceWork.OptimizedNumerics;
using System;

namespace DeepLearningFramework.Data.Operators.Terms
{
    public class MultiplyByNumber : Term
    {
        Term v1;
        float v2;
        public override Dimension D1 { get; internal set; }
        public override Dimension D2 { get; internal set; }

        public MultiplyByNumber(Term v1, float v2)
        {
            Type = TermType.MultiplyByNumber;
            this.v1 = v1;
            this.v2 = v2;
            D1 = this.v1.D1;
            D2 = this.v1.D2;
        }

        public override void CalculateDerivate(MMDerivative s)
        {
            if (!D1.HardEquals(D1) || !D2.HardEquals(D2))
                throw new Exception("Terms should have an exact value!");
            s.MultiplyBy(v2);
            v1.Derivate(s);
            s.DivideBy(v2);
        }

        internal override Matrix CalculateResult()
        {
            if (!D1.HardEquals(D1) || !D2.HardEquals(D2))
                throw new Exception("Terms should have an exact value!");
            return v1.GetResult() * v2;
        }
        public override void CalculateHowManyTimesUsed()
        {
            if (Used == 0)
            {
                v1.CalculateHowManyTimesUsed();
            }
            Used++;
        }
        public override void DeleteResults()
        {
            base.DeleteResults();
            v1.DeleteResults();
        }
    }
}
