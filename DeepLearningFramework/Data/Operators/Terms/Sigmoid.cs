using DeepLearningFramework.Data;
using PerformanceWork.OptimizedNumerics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace DeepLearningFramework.Data.Operators.Terms
{
    public class Sigmoid : Term
    {
        Term v1;
        public override Dimension D1 { get; internal set; }
        public override Dimension D2 { get; internal set; }

        public Sigmoid(Term v1)
        {
            Type = TermType.Sigmoid;
            this.v1 = v1;
            D1 = this.v1.D1;
            D2 = this.v1.D2;
        }

        public override void CalculateDerivate(MMDerivative s)
        {
            if (!D1.HardEquals(D1) || !D2.HardEquals(D2))
                throw new Exception("Terms should have an exact value!");

            Matrix sigmo = GetResult();

            MMDerivative combined = new MMDerivative(s.D1, s.D2, D1, D2);

            //for (int i1 = 0; i1 < D1; i1++)
            //    for (int i2 = 0; i2 < D2; i2++)
            //        m[i1, i2, i1, i2] = sigmo[i1, i2] * (1 - sigmo[i1, i2]);


            for (int i1 = 0; i1 < s.D1; i1++)
                for (int i2 = 0; i2 < s.D2; i2++)
                    for (int i3 = 0; i3 < D1; i3++)
                        for (int i4 = 0; i4 < D2; i4++)
                            combined[i1, i2, i3, i4] = s[i1, i2, i3, i4] * (sigmo[i3, i4] * (1 - sigmo[i3, i4]));

            combined.Negative = s.Negative;

            v1.Derivate(combined);
            combined.Dispose();
        }


        internal override Matrix CalculateResult()
        {
            if (!D1.HardEquals(D1) || !D2.HardEquals(D2))
                throw new Exception("Terms should have an exact value!");

            Matrix sigmo = new Matrix(D1, D2);
            Matrix v = v1.GetResult();
            for (int i = 0; i < D1; i++)
                for (int j = 0; j < D2; j++)
                    sigmo[i, j] = (float)(1 / (1 + Math.Exp(-v[i, j])));
            return sigmo;
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
