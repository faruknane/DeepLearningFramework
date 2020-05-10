using PerformanceWork.OptimizedNumerics;
using System;
using DeepLearningFramework.Core;
using PerformanceWork.DeepLearning.Kernels.Cpu;
using System.Runtime.CompilerServices;

namespace DeepLearningFramework.Operators.Terms
{
    public class Subtract : Term
    {
        public Subtract(Term v1, Term v2)
        {
            Type = TermType.Minus;
            Terms = new Term[2] { v1, v2 };
            if (!v1.Shape.EqualShape(v2.Shape))
                throw new Exception("Terms to be sum should have the same dimensions!");
            this.Shape = v1.Shape.Clone();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public override void CalculateDerivate(Tensor s)
        {
            Tensor d2 = CpuKernels.SubtractFloat_GetGradient_1(s, Terms[0].GetResult(), Terms[1].GetResult());
            Terms[0].Derivate(s);
            Terms[1].Derivate(d2);
            d2.Dispose();
        }

        //todo memoryi azaltmak için aynı s kullanılabilir, nasıl olmalı? seçenek olmalı mı
        //Terms[0].Derivate(s);
        //s.MakeNegative();
        //Terms[1].Derivate(s);
        //s.MakeNegative();

        //diğer bir policy, s değişmeden her yerde kullanılabilir.
        //Terms[0].Derivate(s);
        //Terms[1].Derivate(negative of s);

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public override Tensor CalculateResult()
        {
            Tensor v1 = Terms[0].GetResult();
            Tensor v2 = Terms[1].GetResult();
            Tensor res = CpuKernels.SubtractFloat(v1, v2);
            return res;
        }

    }
}
