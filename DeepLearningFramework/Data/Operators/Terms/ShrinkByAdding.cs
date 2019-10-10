using DeepLearningFramework.Data;
using PerformanceWork.OptimizedNumerics;
using System;
using System.Collections.Generic;
using System.Text;
using DeepLearningFramework.Core;

namespace DeepLearningFramework.Data.Operators.Terms
{
    public class ShrinkByAdding : Term
    {
        public override Dimension D1 { get; internal set; }
        public override Dimension D2 { get; internal set; }
        public Dimension RowDivider { get; private set; }
        public Dimension ColumnDivider { get; private set; }
        public ShrinkByAdding(Term v1, Dimension rowdiv, Dimension coldiv)
        {
            Type = TermType.ShrinkByAdding;
            Terms = new Term[1] { v1 };
            RowDivider = rowdiv;
            ColumnDivider = coldiv;
            D1 = this.Terms[0].D1 / RowDivider;
            D2 = this.Terms[0].D2 / ColumnDivider;
        }

        public override void CalculateDerivate(MMDerivative s)
        {
            if (!RowDivider.HardEquals(RowDivider) || !ColumnDivider.HardEquals(ColumnDivider))
                throw new Exception("Dividers should have an exact value!");

            if (!D1.HardEquals(D1) || !D2.HardEquals(D2))
                throw new Exception("Terms should have an exact value!");

            int v1d1 = Terms[0].D1;
            int v1d2 = Terms[0].D2;

            MMDerivative combined = new MMDerivative(s.D1, s.D2, v1d1, v1d2, true);

            for (int i1 = 0; i1 < s.D1; i1++)
                for (int i2 = 0; i2 < s.D2; i2++)
                    for (int i3 = 0; i3 < v1d1; i3++)
                        for (int i4 = 0; i4 < v1d2; i4++)
                            combined[i1, i2, i3, i4] += s[i1, i2, i3 / RowDivider.Value, i4 / ColumnDivider.Value];// * (m[i3 / RowDivider, i4 / ColumnDivider, i3, i4] = 1);

            combined.Negative = s.Negative;
            Terms[0].Derivate(combined);
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
            
            Matrix v = Terms[0].GetResult();
            
            int v1d1 = Terms[0].D1;
            int v1d2 = Terms[0].D2;
            
            for (int i3 = 0; i3 < v1d1; i3++)
                for (int i4 = 0; i4 < v1d2; i4++)
                    res[i3 / RowDivider.Value, i4 / ColumnDivider.Value] += v[i3, i4];
            return res;
        }
       
    }
}
