﻿
using PerformanceWork.OptimizedNumerics;
using System;
using System.Runtime.CompilerServices;

namespace DeepLearningFramework.Operators.Terms
{
    public class MultiplyByNumber : Term
    {
        public float Multiplier { get; set; }

        public MultiplyByNumber(Term v1, float mult)
        {
            Type = TermType.MultiplyByNumber;
            Terms = new Term[1] { v1 };
            this.Multiplier = mult;
            this.Shape = v1.Shape.Clone();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public override void CalculateDerivate(Tensor s)
        {
            throw new Exception("hatalı, s should never change in multi thread version");
            s.MultiplyByFloat(Multiplier);
            Terms[0].Derivate(s);
            s.DivideByFloat(Multiplier);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public override Tensor CalculateResult()
        {
            Tensor res = Tensor.Clone(Terms[0].GetResult());
            res.MultiplyByFloat(Multiplier);
            return res;
        }

    }
}
