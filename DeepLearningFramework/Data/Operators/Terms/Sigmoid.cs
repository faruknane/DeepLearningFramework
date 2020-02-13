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

            //m[i1, i2, i1, i2] = sigmo[i1, i2] * (1 - sigmo[i1, i2]);
            
            Vectorization.ElementWise_A_MultipliedBy_1_Minus_A_MultipliedByB((float*)sigmo.Array, (float*)s.Array, (float*)combined.Array, sigmo.Shape.TotalSize);
               
            Terms[0].Derivate(combined);
            combined.Dispose();
        }


        public override unsafe Tensor<float> CalculateResult()
        {
            Tensor<float> sigmo = new Tensor<float>(this.Shape.Clone());
            Tensor<float> v = Terms[0].GetResult();
            Vectorization.Sigmoid((float*)v.Array, (float*)sigmo.Array, sigmo.Shape.TotalSize);
            return sigmo;
        }
    }
}
