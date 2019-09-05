using DeepLearningFramework.Data;
using PerformanceWork.OptimizedNumerics;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeepLearningFramework.Operators.Terms
{
    public class ExpandWithSame : Term
    {
        Term v1;
        public override Dimension D1 { get { return this.v1.D1 * RowMultiplier; } }
        public override Dimension D2 { get { return this.v1.D2 * ColumnMultiplier; } }
        public Dimension RowMultiplier { get; private set; }
        public Dimension ColumnMultiplier { get; private set; }
        public ExpandWithSame(Term v1, Dimension rowmul, Dimension colmul)
        {
            this.v1 = v1;
            RowMultiplier = rowmul;
            ColumnMultiplier = colmul;
        }

        public override void Derivate(MMDerivative s)
        {
            if (!RowMultiplier.HardEquals(RowMultiplier) || !ColumnMultiplier.HardEquals(ColumnMultiplier))
                throw new Exception("Dividers should have an exact value!");

            if (!D1.HardEquals(D1) || !D2.HardEquals(D2))
                throw new Exception("Terms should have an exact value!");

            //MMDerivative m = new MMDerivative(D1, D2, v1.D1, v1.D2);

            //for (int i1 = 0; i1 < RowMultiplier; i1   ++)
            //    for (int i2 = 0; i2 < ColumnMultiplier; i2++)
            //        for (int i3 = 0; i3 < v1.D1; i3++)
            //            for (int i4 = 0; i4 < v1.D2; i4++)
            //                m[i1 * v1.D1 + i3, i2 * v1.D2 + i4, i3, i4] = 1;


            MMDerivative combined = new MMDerivative(s.D1, s.D2, v1.D1, v1.D2);
            for (int x1 = 0; x1 < s.D1; x1++)
                for (int x2 = 0; x2 < s.D2; x2++)
                    for (int i1 = 0; i1 < RowMultiplier; i1++)
                        for (int i2 = 0; i2 < ColumnMultiplier; i2++)
                            for (int i3 = 0; i3 < v1.D1; i3++)
                                for (int i4 = 0; i4 < v1.D2; i4++)
                                    combined[x1, x2, i3, i4] += s[x1, x2, i1 * v1.D1 + i3, i2 * v1.D2 + i4];//m[i1 * v1.D1 + i3, i2 * v1.D2 + i4, i3, i4] = 1;
            combined.Negative = s.Negative;
            v1.Derivate(combined);
            combined.Dispose();
        }

        internal override Matrix CalculateResult()
        {
            if (!RowMultiplier.HardEquals(RowMultiplier) || !ColumnMultiplier.HardEquals(ColumnMultiplier))
                throw new Exception("Dividers should have an exact value!");

            if (!D1.HardEquals(D1) || !D2.HardEquals(D2))
                throw new Exception("Terms should have an exact value!");

            Matrix res = new Matrix(D1, D2);
            Matrix v = v1.GetResult();
            for (int i1 = 0; i1 < RowMultiplier; i1++)
                for (int i2 = 0; i2 < ColumnMultiplier; i2++)
                    for (int i3 = 0; i3 < v1.D1; i3++)
                        for (int i4 = 0; i4 < v1.D2; i4++)
                            res[i1 * v1.D1 + i3, i2 * v1.D2 + i4] = v[i3, i4];
            return res;
        }

        public override void DeleteResults()
        {
            base.DeleteResults();
            v1.DeleteResults();
        }
    }
}
