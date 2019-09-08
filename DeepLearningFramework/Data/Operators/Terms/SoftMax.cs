using DeepLearningFramework.Data;
using PerformanceWork.OptimizedNumerics;
using System;

namespace DeepLearningFramework.Data.Operators.Terms
{
    public class SoftMax : Term
    {
        Term v1;
        public override Dimension D1 { get; internal set; }
        public override Dimension D2 { get; internal set; }

        public SoftMax(Term v1)
        {
            Type = TermType.SoftMax;
            this.v1 = v1;
            D1 = this.v1.D1;
            D2 = this.v1.D2;
        }

        public override void CalculateDerivate(MMDerivative s)
        {

        }

        internal override Matrix CalculateResult()
        {
            return null; // v1.GetResult() + v2.GetResult();
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
