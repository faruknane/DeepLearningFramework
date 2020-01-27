using DeepLearningFramework.Data;
using PerformanceWork.OptimizedNumerics;
using System;
using DeepLearningFramework.Core;

namespace DeepLearningFramework.Data.Operators.Terms
{
    public class SoftMax : Term
    {
        public SoftMax(Term v1)
        {
            Type = TermType.SoftMax;
            Terms = new Term[1] { v1 };
            if (v1.Shape.N != 2)
                throw new Exception("not supported shape!");
            this.Shape = v1.Shape.Clone();
        }

        public override unsafe void CalculateDerivate(Tensor<float> s)
        {
            Tensor<float> sm = GetResult();

            Tensor<float> combined = new Tensor<float>(Shape.SwapTail(s.Shape, this.Shape, Terms[0].Shape));

            //for (int i1 = 0; i1 < D1; i1++)
            //    for (int i2 = 0; i2 < D2; i2++)   
            //        m[i1, i2, i1, i2] = sigmo[i1, i2] * (1 - sigmo[i1, i2]);

            Tensor<float> sum = new Tensor<float>(Shape.NewShape(s.Shape[0], s.Shape[1] * this.Shape[1])); //column sum is needed!

            for (int i1 = 0; i1 < s.Shape[0]; i1++)
                for (int i2 = 0; i2 < s.Shape[1]; i2++)
                {
                    int ek = i1 * s.Shape.Multiplied[1] + i2 * s.Shape.Multiplied[2];
                    Vectorization.ElementWiseMultiplyAVX((float*)sm.Array, (float*)s.Array + ek, (float*)combined.Array + ek, sm.Shape.TotalSize);
                    Vectorization.SumOfPerColumn((float*)combined.Array + ek, (float*)sum.Array + i1 * s.Shape[1] * this.Shape[1] + i2 * this.Shape[1], this.Shape[0], this.Shape[1]);
                    //combined[i1, i2, x3, x2] = s[i1, i2, x3, x2] * sm[x3, x2]; yes
                    //combined[i1, i2, x3, x2] += s[i1, i2, x3, x2] * (sm[x3, x2]) * (1 - sm[x3, x2]); not
                }

            //combined[i1, i2, x3, x2] += s[i1, i2, x1, x2] * (-sm[x1, x2] * sm[x3, x2]);

            float* ptrcombined = (float*)combined.Array;
            float* ptrsum = (float*)sum.Array;
            float* ptrsm = (float*)sm.Array;

            for (int i1 = 0; i1 < s.Shape[0]; i1++)
                for (int i2 = 0; i2 < s.Shape[1]; i2++)
                    for (int x3 = 0; x3 < this.Shape[0]; x3++)
                        for (int x2 = 0; x2 < this.Shape[1]; x2++)
                            ptrcombined[combined.Shape.Index(i1, i2, x3, x2)] -= ptrsum[i1 * s.Shape[1] * this.Shape[1] + i2 * this.Shape[1] + x2] * ptrsm[sm.Shape.Index(x3, x2)];
            //for an x3, combined[i1, i2, x3, x2] += sm[x3, x2] * (- s[i1, i2, x1, x2] * sm[x1, x2], for all x1)


            /*working properly
            for (int i1 = 0; i1 < s.D1; i1++)
                for (int i2 = 0; i2 < s.D2; i2++)
                    for (int x1 = 0; x1 < thisD1; x1++)
                        for (int x3 = 0; x3 < thisD1; x3++)
                            for (int x2 = 0; x2 < thisD2; x2++)
                                combined[i1, i2, x3, x2] += -s[i1, i2, x1, x2] * sm[x1, x2] * sm[x3, x2];*/


            sum.Dispose();
            Terms[0].Derivate(combined);
            combined.Dispose();
        }


        public override unsafe Tensor<float> CalculateResult()
        {
            Tensor<float> sm = new Tensor<float>(this.Shape.Clone());

            Tensor<float> sum = new Tensor<float>(Shape.NewShape(1, this.Shape[1]));

            Tensor<float> v = Terms[0].GetResult();

            unsafe
            {
                Vectorization.Exponential((float*)v.Array, (float*)sm.Array, sm.Shape.TotalSize);
                Vectorization.SumOfPerColumn((float*)sm.Array, (float*)sum.Array, this.Shape[0], this.Shape[1]);

                for (int i = 0; i < this.Shape[0]; i++)
                {
                    Vectorization.ElementWiseDivideAVX((float*)sm.Array + i * this.Shape[1], (float*)sum.Array, (float*)sm.Array + i * this.Shape[1], this.Shape[1]);
                }
            }

            sum.Dispose();
            return sm;
        }
      
    }

}
