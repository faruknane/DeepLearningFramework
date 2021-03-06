﻿using PerformanceWork.DeepLearning.Kernels.Cpu;
using PerformanceWork.OptimizedNumerics;
using System.Runtime.CompilerServices;

namespace DeepLearningFramework.Operators.Terms
{
    public class ReLU : Term
    {
        public ReLU(Term v1)
        {
            Type = TermType.ReLU;
            Terms = new Term[1] { v1 };
            this.Shape = v1.Shape.Clone();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public override unsafe void CalculateDerivate(Tensor s)
        {
            if (Terms[0].ContainsTrainable)
            {
                Tensor v = Terms[0].GetResult();
                Tensor combined = CpuKernels.ReluFloat_GetGradient_0(s, v);
                Terms[0].Derivate(combined);
                combined.Dispose();
            }
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public override unsafe Tensor CalculateResult()
        {
            Tensor v = Terms[0].GetResult();
            return CpuKernels.ReluFloat(v);
        }
    }
}
