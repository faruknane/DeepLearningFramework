using DeepLearningFramework.Data;
using PerformanceWork.OptimizedNumerics;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeepLearningFramework.Data.Operators.Terms
{
    public class ShrinkByAdding : Term
    {
        Term v1;
        public override Dimension D1 { get { return this.v1.D1 / RowDivider; }  }
        public override Dimension D2 { get { return this.v1.D2 / ColumnDivider; }  }
        public Dimension RowDivider { get; private set; }
        public Dimension ColumnDivider { get; private set; }
        public ShrinkByAdding(Term v1, Dimension rowdiv, Dimension coldiv)
        {
            Type = TermType.ShrinkByAdding;
            this.v1 = v1;
            RowDivider = rowdiv;
            ColumnDivider = coldiv;
        }

        public override void CalculateDerivate(MMDerivative s)
        {
            if (!RowDivider.HardEquals(RowDivider) || !ColumnDivider.HardEquals(ColumnDivider))
                throw new Exception("Dividers should have an exact value!");

            if (!D1.HardEquals(D1) || !D2.HardEquals(D2))
                throw new Exception("Terms should have an exact value!");

            MMDerivative combined = new MMDerivative(s.D1, s.D2, v1.D1, v1.D2);
            for (int i1 = 0; i1 < s.D1; i1++)
                for (int i2 = 0; i2 < s.D2; i2++)
                    for (int i3 = 0; i3 < v1.D1; i3++)
                        for (int i4 = 0; i4 < v1.D2; i4++)
                            combined[i1, i2, i3, i4] += s[i1, i2, i3 / RowDivider, i4 / ColumnDivider];// * (m[i3 / RowDivider, i4 / ColumnDivider, i3, i4] = 1);

            combined.Negative = s.Negative;
            v1.Derivate(combined);
            combined.Dispose();
        }

        internal override Matrix CalculateResult()
        {
            if (!RowDivider.HardEquals(RowDivider) || !ColumnDivider.HardEquals(ColumnDivider))
                throw new Exception("Dividers should have an exact value!");

            if (!D1.HardEquals(D1) || !D2.HardEquals(D2))
                throw new Exception("Terms should have an exact value!");
            Matrix res = new Matrix(D1, D2);
            res.SetZero();
            Matrix v = v1.GetResult();

            for (int i3 = 0; i3 < v1.D1; i3++)
                for (int i4 = 0; i4 < v1.D2; i4++)
                    res[i3 / RowDivider, i4 / ColumnDivider] += v[i3, i4];
            return res;
        }
        public override void CalculateHowManyTimesUsed()
        {
            if (Used == 0)
            {
                v1.CalculateHowManyTimesUsed();
            }
            Used++;
        }
        public override void DeleteResults()
        {
            base.DeleteResults();
            v1.DeleteResults();
        }
    }
}
