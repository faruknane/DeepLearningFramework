using DeepLearningFramework.Data;
using PerformanceWork.OptimizedNumerics;
using DeepLearningFramework.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace DeepLearningFramework.Data.Operators.Terms
{
    public class Sigmoid : Term
    {
        public Sigmoid(Term v1)
        {
            Type = TermType.Sigmoid;
            Terms = new Term[1] { v1 };
            this.Shape = v1.Shape.Clone();
        }

        public override unsafe void CalculateDerivate(Tensor<float> s)
        {
            Tensor<float> sigmo = GetResult();

            Tensor<float> combined = new Tensor<float>(s.Shape.Clone());

            //for (int i1 = 0; i1 < D1; i1++)
            //    for (int i2 = 0; i2 < D2; i2++)
            //        m[i1, i2, i1, i2] = sigmo[i1, i2] * (1 - sigmo[i1, i2]);
            int go = combined.Shape.TotalSize / this.Shape.TotalSize;
            int ek = 0;

            for (int i = 0; i < go; i++)
            {
                Vectorization.ElementWise_A_MultipliedBy_1_Minus_A_MultipliedByB((float*)sigmo.Array, (float*)s.Array + ek, (float*)combined.Array + ek, sigmo.Shape.TotalSize);
                ek += sigmo.Shape.TotalSize;
                //for (int i3 = 0; i3 < thisD1; i3++)
                //    for (int i4 = 0; i4 < thisD2; i4++)
                //        combined[i1, i2, i3, i4] = s[i1, i2, i3, i4] * (sigmo[i3, i4] * (1 - sigmo[i3, i4]));
            }

            Terms[0].Derivate(combined);
            combined.Dispose();
        }


        public override Tensor<float> CalculateResult()
        {
            Tensor<float> sigmo = new Tensor<float>(this.Shape.Clone());
            Tensor<float> v = Terms[0].GetResult();
            unsafe
            {
                Vectorization.Sigmoid((float*)v.Array, (float*)sigmo.Array, sigmo.Shape.TotalSize);
            }
            return sigmo;
        }
    }
}
