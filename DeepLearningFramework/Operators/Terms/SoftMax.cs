
using PerformanceWork.OptimizedNumerics;
using System;
using DeepLearningFramework.Core;

namespace DeepLearningFramework.Operators.Terms
{
    public class SoftMax : Term
    {
        public SoftMax(Term v1)
        {
            Type = TermType.SoftMax;
            Terms = new Term[1] { v1 };
            this.Shape = v1.Shape.Clone();
        }

        public override unsafe void CalculateDerivate(Tensor s)
        {
            Tensor sm = GetResult();

            Tensor combined = Tensor.Clone(s);

            int groupsize = Shape[Shape.N - 1];

            for (int start = 0; start < combined.Shape.TotalSize; start += groupsize)
            {
                float averageK = VectorizationFloat.SumOfProduction((float*)s.Array + start, (float*)sm.Array + start, groupsize);
                VectorizationFloat.ElementWiseAddAVX((float*)combined.Array + start, -averageK, (float*)combined.Array + start, groupsize);
            }

            VectorizationFloat.ElementWiseMultiplyAVX((float*)combined.Array, (float*)sm.Array, (float*)combined.Array, combined.Shape.TotalSize);

            Terms[0].Derivate(combined);
            combined.Dispose();
        }


        public override unsafe Tensor CalculateResult()
        {
            Tensor v = Terms[0].GetResult();
            Tensor sm = new Tensor(this.Shape.Clone(), Data.Type.Float, DeviceIndicator.Host());
            VectorizationFloat.Softmax((float*)v.Array, (float*)sm.Array, Shape[Shape.N - 1], Shape.TotalSize);
            return sm;
        }
      
    }

}
