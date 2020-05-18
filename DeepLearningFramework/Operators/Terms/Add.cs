
using PerformanceWork.OptimizedNumerics;
using System;
using System.Threading.Tasks;
using DeepLearningFramework.Core;
using System.Threading;
using PerformanceWork.DeepLearning.Kernels.Cpu;
using System.Runtime.CompilerServices;

namespace DeepLearningFramework.Operators.Terms
{
    public class Add : Term
    {
        readonly Tensor[] tensors;

        public Add(params Term[] v) // make args
        {
            if (v.Length < 2)
                throw new Exception("length < 2!");
            Type = TermType.Plus;
            Terms = v;
            tensors = new Tensor[v.Length];
            for (int i = 0; i < Terms.Length - 1; i++)
                if (!this.Terms[i].Shape.EqualShape(this.Terms[i + 1].Shape)) //will be shape, not d1 or d2
                {
                    throw new Exception("Terms to be sum should have the same dimensions!");
                }
            this.Shape = v[0].Shape.Clone();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public override void CalculateDerivate(Tensor s)
        {
            for (int i = 0; i < Terms.Length; i++)
                if(Terms[i].ContainsTrainable)
                    Terms[i].Derivate(s);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public unsafe override Tensor CalculateResult()
        {
            for (int i = 0; i < this.Terms.Length; i++)
                tensors[i] = Terms[i].GetResult();

            return CpuKernels.AddFloat(tensors);
        }

    }
}