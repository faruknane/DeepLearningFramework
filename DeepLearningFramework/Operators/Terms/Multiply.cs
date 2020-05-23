using DeepLearningFramework.Core;
using PerformanceWork.DeepLearning.Kernels.Cpu;
using PerformanceWork.OptimizedNumerics;

namespace DeepLearningFramework.Operators.Terms
{
    public class Multiply : Term
    {
        public Multiply(Term x, Term y)
        {
            Type = TermType.Multiply;
            Terms = new Term[2] { x, y };
            if (!x.Shape.EqualShape(y.Shape))
                throw new DimensionIncompability("Mutliply term inner shapes");
            this.Shape = x.Shape.Clone();
        }

        public override void CalculateDerivate(Tensor s)
        {
            Tensor a = Terms[0].GetResult(), b = Terms[1].GetResult();
            if (Terms[0].ContainsTrainable)
            {
                Tensor g0 = CpuKernels.MultiplyFloat_GetGradient_0(s, a, b);
                Terms[0].Derivate(g0);
                g0.Dispose();
            }
            if (Terms[1].ContainsTrainable)
            {
                Tensor g1 = CpuKernels.MultiplyFloat_GetGradient_1(s, a, b);
                Terms[1].Derivate(g1);
                g1.Dispose();
            }
        }

        public override Tensor CalculateResult()
        {
            return CpuKernels.MultiplyFloat(Terms[0].GetResult(), Terms[1].GetResult());
        }
    }
}
