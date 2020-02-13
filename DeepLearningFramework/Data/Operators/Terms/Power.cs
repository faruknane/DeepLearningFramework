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
        public int PowerOf { get; private set; }

        public Power(Term v1, int pow)
        {
            Type = TermType.Power;
            Terms = new Term[1] { v1 };

            this.PowerOf = pow;

            if (PowerOf <= 1)
                throw new Exception("Power cannot less than two!");

            Shape = v1.Shape.Clone();

        }

        public override unsafe void CalculateDerivate(Tensor<float> s)
        {
            Tensor<float> res = Terms[0].GetResult();
            Tensor<float> combined = new Tensor<float>(s.Shape.Clone()); // new MMDerivative(s.D1, s.D2, D1, D2, false);

            if (PowerOf == 2)
            {
                float* ptr_combined = (float*)combined.Array;
                float* ptr_s = (float*)s.Array;
                Vectorization.ElementWise_A_MultipliedBy_B_MultipliedBy_C((float*)res.Array, ptr_s, PowerOf, ptr_combined, res.Shape.TotalSize);
                Terms[0].Derivate(combined);
                combined.Dispose();
            }
            else
            {
                throw new Exception("Unsupported Power factor!");
                //Matrix pow = Matrix.CreateCopy(res);//Res ^ 1

                //for (int n = 0; n < PowerOf - 2; n++)
                //    pow.ElementWiseMultiply(res);

                //for (int x1 = 0; x1 < s.D1; x1++)
                //    for (int x2 = 0; x2 < s.D2; x2++)
                //    {
                //        float* ptr_combined = combined.Derivatives + x1 * combined.D2 * combined.D3 * combined.D4 + x2 * combined.D3 * combined.D4;
                //        float* ptr_s = s.Derivatives + x1 * s.D2 * s.D3 * s.D4 + x2 * s.D3 * s.D4;
                //        Vectorization.ElementWise_A_MultipliedBy_B_MultipliedBy_C(pow.Array, ptr_s, PowerOf, ptr_combined, pow.D1 * pow.D2);
                //    }

                //combined.Negative = s.Negative;
                //Terms[0].Derivate(combined);
                //pow.Dispose();
                //combined.Dispose();
                //not done 
            }
        }

        public unsafe override Tensor<float> CalculateResult()
        {
            Tensor<float> res = Terms[0].GetResult();
            Tensor<float> m = new Tensor<float>(res.Shape.Clone());

            Vectorization.ElementWiseAssignAVX((float*)m.Array, (float*)res.Array, res.Shape.TotalSize);

            for (int n = 0; n < PowerOf - 1; n++)
                Vectorization.ElementWiseMultiplyAVX((float*)m.Array, (float*)res.Array, (float*)m.Array, m.Shape.TotalSize);
            return m;
        }

    }
}
