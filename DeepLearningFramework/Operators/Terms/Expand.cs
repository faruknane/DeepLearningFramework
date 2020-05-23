
using PerformanceWork.DeepLearning.Kernels.Cpu;
using PerformanceWork.OptimizedNumerics;
using System.Runtime.CompilerServices;

namespace DeepLearningFramework.Operators.Terms
{
    public class Expand : Term
    {
        public Shape Multiplier { get; set; }

        /// <summary>
        /// Shape object wont be returned to the pool back. 
        /// </summary>
        public Expand(Term v1, Shape multiplier)
        {
            Type = TermType.ExpandWithSame;
            Terms = new Term[1] { v1 };
            Multiplier = multiplier;
            Shape = Shape.Multiply(Terms[0].Shape, Multiplier);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public override unsafe void CalculateDerivate(Tensor s)
        {
            if (Terms[0].ContainsTrainable)
            {
                Tensor combined = CpuKernels.ExpandFloat_GetGradient_0(s, this.Shape, Terms[0].Shape, Multiplier);
                Terms[0].Derivate(combined);
                combined.Dispose();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public unsafe override Tensor CalculateResult()
        {
            Tensor v = Terms[0].GetResult();
            Tensor res = CpuKernels.ExpandFloat(v, this.Shape, Terms[0].Shape, Multiplier);
            return res;
        }


    }
}
