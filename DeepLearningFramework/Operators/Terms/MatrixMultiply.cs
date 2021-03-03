using PerformanceWork.DeepLearning.Kernels.Cpu;
using PerformanceWork.OptimizedNumerics;
using System;
using System.Runtime.CompilerServices;

namespace DeepLearningFramework.Operators.Terms
{
    public class MatrixMultiply : Term
    {
        public MatrixMultiply(Term v1, Term v2)
        {
            Type = TermType.MatrixMultiply;
            Terms = new Term[2] { v1, v2 };
            if (v1.Shape.N != 2 || v2.Shape.N != 2) 
                throw new Exception("the same dimensions should be 2!");
            if(this.Terms[0].Shape[1] != this.Terms[1].Shape[0])
                throw new Exception("the same dimensions should match correctly!");
            this.Shape = new Shape(this.Terms[0].Shape[0], this.Terms[1].Shape[1]);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public override unsafe void CalculateDerivate(Tensor s)
        {
            if (Terms[0].ContainsTrainable)
            {
                var combinedleft = CpuKernels.MatrixMultiplyFloat_GetGradient_0(s, Terms[1].GetResult(), this.Shape, Terms[0].Shape, Terms[1].Shape);
                Terms[0].Derivate(combinedleft);
                combinedleft.Dispose();
            }
            if (Terms[1].ContainsTrainable)
            {
                var combinedright = CpuKernels.MatrixMultiplyFloat_GetGradient_1(s, Terms[0].GetResult(), this.Shape, Terms[0].Shape, Terms[1].Shape);
                Terms[1].Derivate(combinedright);
                combinedright.Dispose();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public override Tensor CalculateResult()
        {
            var x1 = Terms[0].GetResult();
            var x2 = Terms[1].GetResult();
            return CpuKernels.MatrixMultiplyFloat(x1, x2);
        }

    }
}
