﻿using DeepLearningFramework.Core;
using PerformanceWork;
using PerformanceWork.DeepLearning.Kernels.Cpu;
using PerformanceWork.OptimizedNumerics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace DeepLearningFramework.Operators.Terms
{
    public class MatrixMultiply : Term
    {
        public MatrixMultiply(Term v1, Term v2)
        {
            Type = TermType.MatrixMultiply;
            Terms = new Term[2] { v1, v2 };
            if (v1.Shape.N != 2 || v2.Shape.N != 2 || this.Terms[0].Shape[1] != this.Terms[1].Shape[0])
                throw new Exception("the same dimensions should match correctly!");
            this.Shape = Shape.NewShape(this.Terms[0].Shape[0], this.Terms[1].Shape[1]);
        }

        public override unsafe void CalculateDerivate(Tensor s)
        {
            var combinedleft = CpuKernels.MatrixMultiplyFloat_GetGradient_0(s, Terms[1].GetResult(), this.Shape, Terms[0].Shape, Terms[1].Shape);
            Terms[0].Derivate(combinedleft);
            combinedleft.Dispose();

            var combinedright = CpuKernels.MatrixMultiplyFloat_GetGradient_1(s, Terms[0].GetResult(), this.Shape, Terms[0].Shape, Terms[1].Shape);
            Terms[1].Derivate(combinedright);
            combinedright.Dispose();
        }

        public override Tensor CalculateResult()
        {
            return CpuKernels.MatrixMultiplyFloat(Terms[0].GetResult(), Terms[1].GetResult());
        }

    }
}
