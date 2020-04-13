
using PerformanceWork.OptimizedNumerics;
using DeepLearningFramework.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using PerformanceWork.DeepLearning.Kernels.Cpu;

namespace DeepLearningFramework.Operators.Terms
{
    public class Sigmoid : Term
    {
        public Sigmoid(Term v1)
        {
            Type = TermType.Sigmoid;
            Terms = new Term[1] { v1 };
            this.Shape = v1.Shape.Clone();
        }

        public override unsafe void CalculateDerivate(Tensor s)
        {
            Tensor sigmo = GetResult();

            Tensor combined = CpuKernels.SigmoidFloat_GetGradient_0(s, sigmo);
            
            Terms[0].Derivate(combined);
            
            combined.Dispose();
        }


        public override unsafe Tensor CalculateResult()
        {
            Tensor v = Terms[0].GetResult();
            return CpuKernels.SigmoidFloat(v);
        }
    }
}
