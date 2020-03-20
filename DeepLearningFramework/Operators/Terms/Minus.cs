
using PerformanceWork.OptimizedNumerics;
using System;
using DeepLearningFramework.Core;

namespace DeepLearningFramework.Operators.Terms
{
    public class Minus : Term
    {
        public Minus(Term v1, Term v2)
        {
            Type = TermType.Minus;
            Terms = new Term[2] { v1, v2 };
            if (!v1.Shape.EqualShape(v2.Shape))
                throw new Exception("Terms to be sum should have the same dimensions!");
            this.Shape = v1.Shape.Clone();
        }

        public override void CalculateDerivate(Tensor<float> s)
        {
            Terms[0].Derivate(s);
            //when we go out, s should remain the same as it was.
            s.MakeNegative();
            Terms[1].Derivate(s);
            s.MakeNegative();
        }

        public override Tensor<float> CalculateResult()
        {
            return Tensor<float>.Subtract(Terms[0].GetResult(), Terms[1].GetResult());
        }

    }
}
