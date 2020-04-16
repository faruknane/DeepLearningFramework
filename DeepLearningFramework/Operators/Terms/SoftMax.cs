
using PerformanceWork.OptimizedNumerics;
using System;
using DeepLearningFramework.Core;
using PerformanceWork.DeepLearning.Kernels.Cpu;

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
            Tensor combined = CpuKernels.SoftmaxFloat_GetGradient_0(s, sm);
            Terms[0].Derivate(combined);
            combined.Dispose();
        }


        public override unsafe Tensor CalculateResult()
        {
            Tensor v = Terms[0].GetResult();
            return CpuKernels.SoftmaxFloat(v);
        }
      
    }

}
