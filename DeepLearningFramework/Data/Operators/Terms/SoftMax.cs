using DeepLearningFramework.Data;
using PerformanceWork.OptimizedNumerics;
using System;
using DeepLearningFramework.Core;

namespace DeepLearningFramework.Data.Operators.Terms
{
    public class SoftMax : Term
    {
        public SoftMax(Term v1)
        {
            Type = TermType.SoftMax;
            Terms = new Term[1] { v1 };
            this.Shape = v1.Shape.Clone();
        }

        public override unsafe void CalculateDerivate(Tensor<float> s)
        {
            Tensor<float> sm = GetResult();

            Tensor<float> combined = Tensor<float>.Clone(s);

            int groupsize = Shape[Shape.N - 1];

            for (int start = 0; start < combined.Shape.TotalSize; start += groupsize)
            {
                float averageK = Vectorization.SumOfProduction((float*)s.Array + start, (float*)sm.Array + start, groupsize);
                Vectorization.ElementWiseAddAVX((float*)combined.Array + start, -averageK, (float*)combined.Array + start, groupsize);
            }

            Vectorization.ElementWiseMultiplyAVX((float*)combined.Array, (float*)sm.Array, (float*)combined.Array, combined.Shape.TotalSize);

            Terms[0].Derivate(combined);
            combined.Dispose();
        }


        public override unsafe Tensor<float> CalculateResult()
        {
            Tensor<float> v = Terms[0].GetResult();
            Tensor<float> sm = new Tensor<float>(this.Shape.Clone());
            Vectorization.Softmax((float*)v.Array, (float*)sm.Array, Shape[Shape.N - 1], Shape.TotalSize);
            return sm;
        }
      
    }

}
