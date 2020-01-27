using DeepLearningFramework.Data;
using PerformanceWork.OptimizedNumerics;
using System;
using System.Collections.Generic;
using System.Text;
using DeepLearningFramework.Core;
using Index = PerformanceWork.OptimizedNumerics.Index;

namespace DeepLearningFramework.Data.Operators.Terms
{
    public class ShrinkByAdding : Term
    {
        public Shape Divisor { get; set; }

        public unsafe ShrinkByAdding(Term v1, Shape divisor)
        {
            Type = TermType.ShrinkByAdding;
            Terms = new Term[1] { v1 };
            Divisor = divisor;
            this.Shape = Shape.Divide(v1.Shape, divisor);
        }

        public unsafe override void CalculateDerivate(Tensor<float> s)
        {
            Tensor<float> combined = new Tensor<float>(Shape.SwapTail(s.Shape, this.Shape, Terms[0].Shape));

            int go = s.Shape.TotalSize / this.Shape.TotalSize;

            float* ptrcombined = (float*)combined.Array;
            float* ptrs = (float*)s.Array;

            Index iterator = Index.NewIndex(this.Terms[0].Shape);

            int ek = 0;
            int ek2 = 0;
            for (int j = 0; j < go; j++)
            {
                for (int i = 0; i < iterator.N; i++)
                    iterator.Indexes[i] = 0;

                for (int h = 0; h < this.Terms[0].Shape.TotalSize; h++)
                {

                    int indexs = 0;

                    for (int i = iterator.N - 1; i >= 0; i--)
                    {
                        if (iterator.Indexes[i] == this.Terms[0].Shape[i])
                        {
                            iterator.Indexes[i] = 0;
                            iterator.Indexes[i - 1]++;
                        }
                        if (i + 1 < this.Shape.N)
                            indexs += (iterator.Indexes[i] / Divisor[i]) * this.Shape.Multiplied[i + 1];
                        else
                            indexs += (iterator.Indexes[i] / Divisor[i]);
                    }

                    ptrcombined[ek + h] = ptrs[ek2 + indexs];
                    iterator.Indexes[iterator.N - 1]++;
                }

                ek += this.Terms[0].Shape.TotalSize;
                ek2 += this.Shape.TotalSize;
            }
            //for (int i1 = 0; i1 < s.D1; i1++)
            //    for (int i2 = 0; i2 < s.D2; i2++)
            //        for (int i3 = 0; i3 < v1d1; i3++)
            //            for (int i4 = 0; i4 < v1d2; i4++)
            //                combined[i1, i2, i3, i4] += s[i1, i2, i3 / RowDivider, i4 / ColumnDivider];// * (m[i3 / RowDivider, i4 / ColumnDivider, i3, i4] = 1);
            
            Terms[0].Derivate(combined);
            Index.Return(iterator);
            combined.Dispose();
        }
     
        public unsafe override Tensor<float> CalculateResult()
        {
            Tensor<float> res = new Tensor<float>(this.Shape.Clone());
            res.SetValue(0);

            Tensor<float> v = Terms[0].GetResult();

            float* ptrres = (float*)res.Array;
            float* ptrv = (float*)v.Array;

            Index iterator = Index.NewIndex(this.Terms[0].Shape);

            for (int i = 0; i < iterator.N; i++)
                iterator.Indexes[i] = 0;

            for (int h = 0; h < this.Terms[0].Shape.TotalSize; h++)
            {
                int indexs = 0;

                for (int i = iterator.N - 1; i >= 0; i--)
                {
                    if (iterator.Indexes[i] == this.Terms[0].Shape[i])
                    {
                        iterator.Indexes[i] = 0;
                        iterator.Indexes[i - 1]++;
                    }
                    if (i + 1 < this.Shape.N)
                        indexs += (iterator.Indexes[i] / Divisor[i]) * this.Shape.Multiplied[i + 1];
                    else
                        indexs += (iterator.Indexes[i] / Divisor[i]);
                }
                ptrres[indexs] += ptrv[h];
                iterator.Indexes[iterator.N - 1]++;
            }
            Index.Return(iterator);
            //int v1d1 = Terms[0].D1;
            //int v1d2 = Terms[0].D2;

            //for (int i3 = 0; i3 < v1d1; i3++)
            //    for (int i4 = 0; i4 < v1d2; i4++)
            //        res[i3 / RowDivider, i4 / ColumnDivider] += v[i3, i4];
            return res;
        }

        public override void Dispose()
        {
            Shape.Return(Divisor);
            base.Dispose();
        }

    }
}
