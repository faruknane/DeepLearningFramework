using DeepLearningFramework.Data;
using PerformanceWork.OptimizedNumerics;
using System;

namespace DeepLearningFramework.Data.Operators.Terms
{
    public class Plus : Term
    {
        Term v1, v2;
        public override Dimension D1 { get; internal set; }
        public override Dimension D2 { get; internal set; }

        public Plus(Term v1, Term v2)
        {
            Type = TermType.Plus;
            this.v1 = v1;
            this.v2 = v2;
            if (!this.v1.D1.SoftEquals(this.v2.D1) || !this.v1.D2.SoftEquals(this.v2.D2))
                throw new Exception("Terms to be sum should have the same dimensions!");
            D1 = this.v1.D1;
            D2 = this.v1.D2;
        }

        public override void CalculateDerivate(MMDerivative s)
        {
            if (!this.v1.D1.HardEquals(this.v2.D1) || !this.v1.D2.HardEquals(this.v2.D2))
                throw new Exception("Terms to be sum should have the same dimensions!");
            v1.Derivate(s);
            v2.Derivate(s);
        }

        internal override Matrix CalculateResult()
        {
            if (!this.v1.D1.HardEquals(this.v2.D1) || !this.v1.D2.HardEquals(this.v2.D2))
                throw new Exception("Terms to be sum should have the same dimensions!");
            return v1.GetResult() + v2.GetResult();
        }
        public override void CalculateHowManyTimesUsed()
        {
            if (Used == 0)
            {
                v1.CalculateHowManyTimesUsed();
                v2.CalculateHowManyTimesUsed();
            }
            Used++;
        }
        public override void DeleteResults()
        {
            base.DeleteResults();
            v1.DeleteResults();
            v2.DeleteResults();
        }
    }
}