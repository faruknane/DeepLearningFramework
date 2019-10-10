using DeepLearningFramework.Data;
using PerformanceWork.OptimizedNumerics;
using System;
using System.Collections.Generic;
using System.Text;
using DeepLearningFramework.Core;

namespace DeepLearningFramework.Data.Operators.Terms
{
    public class Power : Term
    {
        public override Dimension D1 { get; internal set; }
        public override Dimension D2 { get; internal set; }
        public int PowerOf { get; private set; }

        public Power(Term v1, int pow)
        {
            Type = TermType.Power;
            Terms = new Term[1] { v1 };
            this.PowerOf = pow;
            if (PowerOf <= 1)
                throw new Exception("Power cannot less than two!");
            D1 = this.Terms[0].D1;
            D2 = this.Terms[0].D2;
        }

        public override unsafe void CalculateDerivate(MMDerivative s)
        {
            if (!D1.HardEquals(D1) || !D2.HardEquals(D2))
                throw new Exception("Terms should have an exact value!");

            Matrix res = Terms[0].GetResult();
            MMDerivative combined = new MMDerivative(s.D1, s.D2, D1, D2, false);

            if (PowerOf == 2)
            {
                for (int x1 = 0; x1 < s.D1; x1++)
                    for (int x2 = 0; x2 < s.D2; x2++)
                    {
                        float* ptr_combined = combined.Derivatives + x1 * combined.D2 * combined.D3 * combined.D4 + x2 * combined.D3 * combined.D4;
                        float* ptr_s = s.Derivatives + x1 * s.D2 * s.D3 * s.D4 + x2 * s.D3 * s.D4;
                        Vectorization.ElementWise_A_MultipliedBy_B_MultipliedBy_C(res.Array, ptr_s, PowerOf, ptr_combined, res.D1 * res.D2);
                    }
                combined.Negative = s.Negative;
                Terms[0].Derivate(combined);
                combined.Dispose();
            }
            else
            {
                Matrix pow = Matrix.CreateCopy(res);//Res ^ 1

                for (int n = 0; n < PowerOf - 2; n++)
                    pow.ElementWiseMultiply(res);

                for (int x1 = 0; x1 < s.D1; x1++)
                    for (int x2 = 0; x2 < s.D2; x2++)
                    {
                        float* ptr_combined = combined.Derivatives + x1 * combined.D2 * combined.D3 * combined.D4 + x2 * combined.D3 * combined.D4;
                        float* ptr_s = s.Derivatives + x1 * s.D2 * s.D3 * s.D4 + x2 * s.D3 * s.D4;
                        Vectorization.ElementWise_A_MultipliedBy_B_MultipliedBy_C(pow.Array, ptr_s, PowerOf, ptr_combined, pow.D1 * pow.D2);
                    }

                combined.Negative = s.Negative;
                Terms[0].Derivate(combined);
                pow.Dispose();
                combined.Dispose();
            }
        }

        internal override Matrix CalculateResult()
        {
            if (!D1.HardEquals(D1) || !D2.HardEquals(D2))
                throw new Exception("Terms should have an exact value!");

            Matrix res = Terms[0].GetResult();
            Matrix m = Matrix.CreateCopy(res);
            for (int n = 0; n < PowerOf - 1; n++)
                m.ElementWiseMultiply(res);
            return m;
        }
       
    }
}
