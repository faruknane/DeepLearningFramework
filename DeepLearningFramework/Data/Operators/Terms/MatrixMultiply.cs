using DeepLearningFramework.Core;
using DeepLearningFramework.Data;
using PerformanceWork.OptimizedNumerics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace DeepLearningFramework.Data.Operators.Terms
{
    public class MatrixMultiply : Term
    {
        public MatrixMultiply(Term v1, Term v2)
        {
            Type = TermType.MatrixMultiply;
            Terms = new Term[2] { v1, v2 };
            if (v1.Shape.N != 2 || v2.Shape.N != 2 || this.Terms[0].Shape[1] != this.Terms[1].Shape[0])
                throw new Exception("the same dimensions should match correctly!");
            this.Shape = Shape.NewShape(this.Terms[0].Shape[0], this.Terms[1].Shape[1]);
        }

        public override unsafe void CalculateDerivate(Tensor<float> s)
        {
            {
                Tensor<float> B = Terms[1].GetResult();
                var combinedleft = new Tensor<float>(Terms[0].Shape.Clone());
                float* ptr_left = (float*)combinedleft.Array, ptr_s = (float*)s.Array, ptr_b = (float*)B.Array;
                Vectorization.TransposeBandMatrixMultiply(ptr_s, this.Shape[0], this.Shape[1], ptr_b, B.Shape[0], B.Shape[1], ptr_left);

                Terms[0].Derivate(combinedleft);
                combinedleft.Dispose();
            }

            {
                Tensor<float> A = Terms[0].GetResult();
                var combinedright = new Tensor<float>(Terms[1].Shape.Clone());
                float* ptr_right = (float*)combinedright.Array, ptr_a = (float*)A.Array, ptr_s = (float*)s.Array;
                Vectorization.TransposeAandMatrixMultiply(ptr_a, A.Shape[0], A.Shape[1], ptr_s, this.Shape[0], this.Shape[1], ptr_right);
                Terms[1].Derivate(combinedright);
                combinedright.Dispose();
            }
        }

        public override Tensor<float> CalculateResult()
        {
            return Tensor<float>.MatrixMultiply(Terms[0].GetResult(), Terms[1].GetResult());
        }

    }
}
