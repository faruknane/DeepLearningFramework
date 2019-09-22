using DeepLearningFramework.Data;
using PerformanceWork.OptimizedNumerics;
using System;
using System.Diagnostics;

namespace DeepLearningFramework.Data.Operators.Terms
{
    public class MatrixMultiply : Term
    {
        Term v1, v2;
        public override Dimension D1 { get; internal set; }
        public override Dimension D2 { get; internal set; }

        public MatrixMultiply(Term v1, Term v2)
        {
            Type = TermType.MatrixMultiply;
            this.v1 = v1;
            this.v2 = v2;
            if (!this.v1.D2.SoftEquals(this.v2.D1))
                throw new Exception("the same dimensions should match correctly!");
            D1 = this.v1.D1;
            D2 = this.v2.D2;
            //Console.WriteLine("-> " + v1.D1.Value + ", " + v1.D2.Value + ", " + v2.D2.Value);
        }

        public override unsafe void CalculateDerivate(MMDerivative s)
        {
            if (!this.v1.D2.HardEquals(this.v2.D1))
                throw new Exception("the same dimensions should match correctly!");
            if(!D1.HardEquals(D1) || !D2.HardEquals(D2))
                throw new Exception("Terms should have an exact value!");

            Matrix B = v2.GetResult();
            Matrix A = v1.GetResult();
            //B = Matrix.TranposeOf(B);
            //A = Matrix.TranposeOf(A);

            //MMDerivative WRTLeft = new MMDerivative(v1.D1, v2.D2, v1.D1, v1.D2);
            //for (int a = 0; a < v1.D1; a++)
            //    for (int b = 0; b < v2.D2; b++)
            //        for (int d = 0; d < v1.D2; d++)
            //            WRTLeft[a, b, a, d] = B[d, b];

            //MMDerivative WRTRight = new MMDerivative(v1.D1, v2.D2, v2.D1, v2.D2);
            //for (int a = 0; a < v1.D1; a++)
            //    for (int b = 0; b < v2.D2; b++)
            //        for (int c = 0; c < v2.D1; c++)
            //            WRTRight[a, b, c, b] = A[a, c];

            var combinedleft = new MMDerivative(s.D1, s.D2, v1.D1, v1.D2);
            var combinedright = new MMDerivative(s.D1, s.D2, v2.D1, v2.D2);


            float* ptr_left = combinedleft.Derivatives, ptr_s = s.Derivatives, ptr_b = B.Array;
                for (int i1 = 0; i1 < s.D1; i1++)
                    for (int i2 = 0; i2 < s.D2; i2++)
                    {
                        //i1 * D2 * D3 * D4 + i2 * D3 * D4
                        int loc_left = i1 * combinedleft.D2 * combinedleft.D3 * combinedleft.D4 + i2 * combinedleft.D3 * combinedleft.D4;
                        int loc_s = i1 * s.D2 * s.D3 * s.D4 + i2 * s.D3 * s.D4;
                        Vectorization.TransposeBandMatrixMultiply(ptr_s + loc_s, s.D3, s.D4, ptr_b, B.D1, B.D2, ptr_left + loc_left);
                        //combinedleft[i1, i2, i3, x2] += s[i1, i2, i3, i4] * B[i4, x2];//for transpose of B
                    }

            //for (int i1 = 0; i1 < s.D1; i1++)
            //    for (int i2 = 0; i2 < s.D2; i2++)
            //        for (int i3 = 0; i3 < s.D3; i3++) //v1.D1
            //            for (int x2 = 0; x2 < combinedleft.D4; x2++)
            //                for (int i4 = 0; i4 < s.D4; i4++) //v2.D2
            //                    combinedleft[i1, i2, i3, x2] += s[i1, i2, i3, i4] * B[x2, i4];//for normal B 

            float* ptr_right = combinedright.Derivatives, ptr_a = A.Array; ptr_s = s.Derivatives;
                for (int i1 = 0; i1 < s.D1; i1++)
                    for (int i2 = 0; i2 < s.D2; i2++)
                    {
                        int loc_right = i1 * combinedright.D2 * combinedright.D3 * combinedright.D4 + i2 * combinedright.D3 * combinedright.D4;
                        int loc_s = i1 * s.D2 * s.D3 * s.D4 + i2 * s.D3 * s.D4;
                        Vectorization.TransposeAandMatrixMultiply(ptr_a, A.D1, A.D2, ptr_s + loc_s, s.D3, s.D4, ptr_right + loc_right);
                        //combinedright[i1, i2, x1, i4] += s[i1, i2, i3, i4] * A[x1, i3];
                    }
            //for (int i1 = 0; i1 < s.D1; i1++)
            //    for (int i2 = 0; i2 < s.D2; i2++)
            //        for (int i3 = 0; i3 < s.D3; i3++) //v1.D1
            //            for (int x1 = 0; x1 < combinedright.D3; x1++)
            //                for (int i4 = 0; i4 < s.D4; i4++) //v2.D2
            //                    combinedright[i1, i2, x1, i4] += s[i1, i2, i3, i4] * A[i3, x1];

            combinedleft.Negative = s.Negative;
            combinedright.Negative = s.Negative;

            v1.Derivate(combinedleft);
            v2.Derivate(combinedright);
            //A.Dispose();
            //B.Dispose();
            combinedleft.Dispose();
            combinedright.Dispose();

        }

        internal override Matrix CalculateResult()
        {
            if (!this.v1.D2.HardEquals(this.v2.D1))
                throw new Exception("the same dimensions should match correctly!");
            if (!D1.HardEquals(D1) || !D2.HardEquals(D2))
                throw new Exception("Terms should have an exact value!");

            Matrix a = v1.GetResult();
            Matrix b = v2.GetResult();
            Matrix res = Matrix.MatrixMultiply(a, b);
            return res;
        }
        public override void CalculateHowManyTimesUsed()
        {
            if (Used == 0)
            {
                v1.CalculateHowManyTimesUsed();
                v2.CalculateHowManyTimesUsed();
            }
            Used++;
        }
        public override void DeleteResults()
        {
            base.DeleteResults();
            v1.DeleteResults();
            v2.DeleteResults();
        }
    }
}
