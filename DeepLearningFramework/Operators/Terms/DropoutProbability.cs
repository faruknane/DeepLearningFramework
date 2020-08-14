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
            Shape = s;
        }

        public override void CalculateDerivate(Tensor s)
        {
            Console.WriteLine("If code comes here, it means there is a bug? There is no trainable parameter here!");
        }

        public override Tensor CalculateResult()
        {
            Tensor t = new Tensor(Shape.Clone(), DataType.Type.Float, DeviceIndicator.Host());
            CpuKernels.Probability.DropoutFloat(t, Probability);
            return t;
        }
    }
}
