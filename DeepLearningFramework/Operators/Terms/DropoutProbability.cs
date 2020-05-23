using PerformanceWork;
using PerformanceWork.DeepLearning.Kernels.Cpu;
using PerformanceWork.OptimizedNumerics;
using System;

namespace DeepLearningFramework.Operators.Terms
{
    public class DropoutProbability : Term
    {
        public float Probability;
        public DropoutProbability(Shape s, float p)
        {
            Probability = p;
            Type = TermType.DropoutProbability;
            Terms = Array.Empty<Term>();
            Shape = s.Clone();
        }

        public override void CalculateDerivate(Tensor s)
        {
            Console.WriteLine("Bug! Please report that bug: DropoutProbability");
        }

        public override Tensor CalculateResult()
        {
            Tensor t = new Tensor(Shape.Clone(), DataType.Type.Float, DeviceIndicator.Host());
            CpuKernels.Probability.DropoutFloat(t, Probability);
            return t;
        }
    }
}
