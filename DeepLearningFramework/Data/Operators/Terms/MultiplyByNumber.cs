using DeepLearningFramework.Data;
using PerformanceWork.OptimizedNumerics;
using System;
using DeepLearningFramework.Core;

namespace DeepLearningFramework.Data.Operators.Terms
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

        public override void CalculateDerivate(Tensor<float> s)
        {
            s.MultiplyBy(Multiplier);
            Terms[0].Derivate(s);
            s.DivideBy(Multiplier);
        }

        public override Tensor<float> CalculateResult()
        {
            Tensor<float> res = Tensor<float>.Clone(Terms[0].GetResult());
            res.MultiplyBy(Multiplier);
            return res;
        }

    }
}
