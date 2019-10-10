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
        public override Dimension D1 { get; internal set; }
        public override Dimension D2 { get; internal set; }

        public MatrixMultiply(Term v1, Term v2)
        {
            Type = TermType.MatrixMultiply;
            Terms = new Term[2] { v1, v2 };
            if (!this.Terms[0].D2.SoftEquals(this.Terms[1].D1))
                throw new Exception("the same dimensions should match correctly!");
            D1 = this.Terms[0].D1;
            D2 = this.Terms[1].D2;
        }

        public override unsafe void CalculateDerivate(MMDerivative s)
        {
            if (!this.Terms[0].D2.HardEquals(this.Terms[1].D1))
                throw new Exception("the same dimensions should match correctly!");
            if (!D1.HardEquals(D1) || !D2.HardEquals(D2))
                throw new Exception("Terms should have an exact value!");


            {
                Matrix B = Terms[1].GetResult();
                var combinedleft = new MMDerivative(s.D1, s.D2, Terms[0].D1, Terms[0].D2, false);
                float* ptr_left = combinedleft.Derivatives, ptr_s = s.Derivatives, ptr_b = B.Array;
                for (int i1 = 0; i1 < s.D1; i1++)
                    for (int i2 = 0; i2 < s.D2; i2++)
                    {
                        int loc_left = i1 * combinedleft.D2 * combinedleft.D3 * combinedleft.D4 + i2 * combinedleft.D3 * combinedleft.D4;
                        int loc_s = i1 * s.D2 * s.D3 * s.D4 + i2 * s.D3 * s.D4;
                        Vectorization.TransposeBandMatrixMultiply(ptr_s + loc_s, s.D3, s.D4, ptr_b, B.D1, B.D2, ptr_left + loc_left);
                    }
                combinedleft.Negative = s.Negative;
                Terms[0].Derivate(combinedleft);
                combinedleft.Dispose();
            }

            {
                Matrix A = Terms[0].GetResult();
                var combinedright = new MMDerivative(s.D1, s.D2, Terms[1].D1, Terms[1].D2, false);
                float* ptr_right = combinedright.Derivatives, ptr_a = A.Array, ptr_s = s.Derivatives;
                for (int i1 = 0; i1 < s.D1; i1++)
                    for (int i2 = 0; i2 < s.D2; i2++)
                    {
                        int loc_right = i1 * combinedright.D2 * combinedright.D3 * combinedright.D4 + i2 * combinedright.D3 * combinedright.D4;
                        int loc_s = i1 * s.D2 * s.D3 * s.D4 + i2 * s.D3 * s.D4;
                        Vectorization.TransposeAandMatrixMultiply(ptr_a, A.D1, A.D2, ptr_s + loc_s, s.D3, s.D4, ptr_right + loc_right);
                    }
                combinedright.Negative = s.Negative;
                Terms[1].Derivate(combinedright);
                combinedright.Dispose();
            }
        }

        internal override Matrix CalculateResult()
        {
            if (!this.Terms[0].D2.HardEquals(this.Terms[1].D1))
                throw new Exception("the same dimensions should match correctly!");
            if (!D1.HardEquals(D1) || !D2.HardEquals(D2))
                throw new Exception("Terms should have an exact value!");

            Matrix a = Terms[0].GetResult();
            Matrix b = Terms[1].GetResult();
            Matrix res = Matrix.MatrixMultiply(a, b);
            return res;
        }
      
    }
}
