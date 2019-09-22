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
            if (!D1.HardEquals(D1) || !D2.HardEquals(D2))
                throw new Exception("Terms should have an exact value!");

            Matrix sm = GetResult();

            MMDerivative combined = new MMDerivative(s.D1, s.D2, D1, D2);

            //for (int i1 = 0; i1 < D1; i1++)
            //    for (int i2 = 0; i2 < D2; i2++)
            //        m[i1, i2, i1, i2] = sigmo[i1, i2] * (1 - sigmo[i1, i2]);


            for (int i1 = 0; i1 < s.D1; i1++)
                for (int i2 = 0; i2 < s.D2; i2++)
                    for (int x1 = 0; x1 < D1; x1++)
                        for (int x2 = 0; x2 < D2; x2++)
                            for (int x3 = 0; x3 < D1; x3++) // it has to be x4 = x2
                                if (x1 == x3)
                                    combined[i1, i2, x3, x2] += s[i1, i2, x1, x2] * (sm[x1, x2]) * (1 - sm[x1, x2]);
                                else
                                    combined[i1, i2, x3, x2] += s[i1, i2, x1, x2] * (-sm[x1, x2] * sm[x3, x2]); //this[x1,x2,x1,x2];

            combined.Negative = s.Negative;

            v1.Derivate(combined);
            combined.Dispose();
        }


        internal override Matrix CalculateResult()
        {
            if (!D1.HardEquals(D1) || !D2.HardEquals(D2))
                throw new Exception("Terms should have an exact value!");

            Matrix sm = new Matrix(D1, D2);
            Matrix sum = new Matrix(1, D2);
            sum.SetZero();
            Matrix v = v1.GetResult();

            unsafe
            {
                Vectorization.Exponential(v.Array, sm.Array, sm.D1 * sm.D2);

                for (int j = 0; j < D2; j++)
                {
                    for (int i = 0; i < D1; i++)
                    {
                        //sm[i, j] = MathF.Exp(v[i, j]);
                        sum[0, j] += sm[i, j];
                    }
                    for (int i = 0; i < D1; i++)
                        sm[i, j] = sm[i, j] / sum[0, j];
                }
            }

            sum.Dispose();
            return sm;
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
