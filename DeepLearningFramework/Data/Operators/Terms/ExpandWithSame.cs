//using DeepLearningFramework.Data;
//using PerformanceWork.OptimizedNumerics;
//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Threading.Tasks;
//using DeepLearningFramework.Core;

//namespace DeepLearningFramework.Data.Operators.Terms
//{
//    public class ExpandWithSame : Term
//    {
//        public int RowMultiplier { get; private set; }
//        public int ColumnMultiplier { get; private set; }
//        public ExpandWithSame(Term v1, int rowmul, int colmul)
//        {
//            Type = TermType.ExpandWithSame;
//            Terms = new Term[1] { v1 };
//            RowMultiplier = rowmul;
//            ColumnMultiplier = colmul;

//            D1 = this.Terms[0].D1 * RowMultiplier;
//            D2 = this.Terms[0].D2 * ColumnMultiplier;

//            if (this.D1 <= 0 || this.D2 <= 0)
//                throw new Exception("Dimensions should have an exact value!");
//        }

//        public override unsafe void CalculateDerivate(MMDerivative s)
//        {

//            if (RowMultiplier == 1 && this.Terms[0].D2 == 1 && ColumnMultiplier > 1)
//            {
//                //Optimized Enough
//                MMDerivative combined = new MMDerivative(s.D1, s.D2, Terms[0].D1, Terms[0].D2, true);
                
//                int v1d1 = Terms[0].D1;
//                int v1d2 = Terms[0].D2;

//                for (int x1 = 0; x1 < s.D1; x1++)
//                    for (int x2 = 0; x2 < s.D2; x2++)
//                    {
//                        float* ss = s.Derivatives + x1 * s.D2 * s.D3 * s.D4 + x2 * s.D3 * s.D4;
//                        for (int i3 = 0; i3 < v1d1; i3++)
//                            combined[x1, x2, i3, 0] = Vectorization.SumOfVector(ss + i3 * s.D4, ColumnMultiplier);
//                    }
//                combined.Negative = s.Negative;
//                Terms[0].Derivate(combined);
//                combined.Dispose();
//            }
//            else
//            {
//                //Unoptimized
//                //make warning system!
//                MMDerivative combined = new MMDerivative(s.D1, s.D2, Terms[0].D1, Terms[0].D2, true);

//                int v1d1 = Terms[0].D1;
//                int v1d2 = Terms[0].D2;

//                for (int x1 = 0; x1 < s.D1; x1++)
//                    for (int x2 = 0; x2 < s.D2; x2++)
//                        for (int i1 = 0; i1 < RowMultiplier; i1++)
//                            for (int i2 = 0; i2 < ColumnMultiplier; i2++)
//                                for (int i3 = 0; i3 < v1d1; i3++)
//                                    for (int i4 = 0; i4 < v1d2; i4++)
//                                        combined[x1, x2, i3, i4] += s[x1, x2, i1 * v1d1 + i3, i2 * v1d2 + i4];//m[i1 * v1.D1 + i3, i2 * v1.D2 + i4, i3, i4] = 1;

//                combined.Negative = s.Negative;
//                Terms[0].Derivate(combined);
//                combined.Dispose();
//            }
//        }

//        internal override unsafe Matrix CalculateResult()
//        {
//            if (RowMultiplier == 1 && this.Terms[0].D2 == 1 && ColumnMultiplier > 1)
//            {
//                //Optimized Enough
//                Matrix res = new Matrix(D1, D2);
//                Matrix v = Terms[0].GetResult();
//                int v1d1 = Terms[0].D1;
//                for (int i3 = 0; i3 < v1d1; i3++)
//                    Vectorization.ElementWiseSetValueAVX(res.Array + i3 * res.D2, v[i3], ColumnMultiplier);
//                return res;
//            }
//            else
//            {
//                //Unoptimized
//                Matrix res = new Matrix(D1, D2);
//                Matrix v = Terms[0].GetResult();
//                int v1d1 = Terms[0].D1;
//                int v1d2 = Terms[0].D2;
//                for (int i1 = 0; i1 < RowMultiplier; i1++)
//                    for (int i2 = 0; i2 < ColumnMultiplier; i2++)
//                        for (int i3 = 0; i3 < v1d1; i3++)
//                            for (int i4 = 0; i4 < v1d2; i4++)
//                                res[i1 * v1d1 + i3, i2 * v1d2 + i4] = v[i3, i4];

//                return res;
//            }
//        }

//    }
//}
