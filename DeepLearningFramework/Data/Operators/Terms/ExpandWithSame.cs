using DeepLearningFramework.Data;
using PerformanceWork.OptimizedNumerics;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DeepLearningFramework.Core;
using Index = PerformanceWork.OptimizedNumerics.Index;

namespace DeepLearningFramework.Data.Operators.Terms
{
    public class ExpandWithSame : Term
    {
        public Shape Multiplier { get; set; }
        public ExpandWithSame(Term v1, Shape multiplier)
        {
            Type = TermType.ExpandWithSame;
            Terms = new Term[1] { v1 };
            Multiplier = multiplier;
            Shape = Shape.Multiply(Terms[0].Shape, Multiplier);
        }

        public override unsafe void CalculateDerivate(Tensor<float> s)
        {
            Tensor<float> combined = new Tensor<float>(Terms[0].Shape.Clone());
            combined.SetValue(0);

            float* ptrcombined = (float*)combined.Array;
            float* ptrs = (float*)s.Array;

            Index iterator = Index.NewIndex(this.Shape);

            for (int i = 0; i < iterator.N; i++)
                iterator.Indexes[i] = 0;

            for (int h = 0; h < this.Shape.TotalSize; h++)
            {

                int indexs = 0;

                for (int i = iterator.N - 1; i >= 0; i--)
                {
                    if (iterator.Indexes[i] == this.Shape[i])
                    {
                        iterator.Indexes[i] = 0;
                        iterator.Indexes[i - 1]++;
                    }
                    indexs += (iterator.Indexes[i] / Multiplier[i]) * this.Terms[0].Shape.Multiplied[i + 1];
                }

                ptrcombined[indexs] += ptrs[h];
                iterator.Indexes[iterator.N - 1]++;
            }
            Index.Return(iterator);

            Terms[0].Derivate(combined);
            combined.Dispose();

        }

        public unsafe override Tensor<float> CalculateResult()
        {
            Tensor<float> res = new Tensor<float>(this.Shape.Clone());

            Tensor<float> v = Terms[0].GetResult();

            float* ptrres = (float*)res.Array;
            float* ptrv = (float*)v.Array;

            Index iterator = Index.NewIndex(this.Shape);

            for (int i = 0; i < iterator.N; i++)
                iterator.Indexes[i] = 0;

            for (int h = 0; h < this.Shape.TotalSize; h++)
            {
                int indexs = 0;

                for (int i = iterator.N - 1; i >= 0; i--)
                {
                    if (iterator.Indexes[i] == this.Shape[i])
                    {
                        iterator.Indexes[i] = 0;
                        iterator.Indexes[i - 1]++;
                    }
                    indexs += (iterator.Indexes[i] / Multiplier[i]) * this.Terms[0].Shape.Multiplied[i + 1];
                }
                ptrres[h] = ptrv[indexs];
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
            Shape.Return(Multiplier);
            base.Dispose();
        }

    }
}
