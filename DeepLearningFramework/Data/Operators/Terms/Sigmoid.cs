﻿using DeepLearningFramework.Data;
using PerformanceWork.OptimizedNumerics;
using DeepLearningFramework.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace DeepLearningFramework.Data.Operators.Terms
{
    public class Sigmoid : Term
    {
        public override Dimension D1 { get; internal set; }
        public override Dimension D2 { get; internal set; }

        public Sigmoid(Term v1)
        {
            Type = TermType.Sigmoid;
            Terms = new Term[1] { v1 };
            D1 = this.Terms[0].D1;
            D2 = this.Terms[0].D2;
        }

        public override unsafe void CalculateDerivate(MMDerivative s)
        {
            if (!D1.HardEquals(D1) || !D2.HardEquals(D2))
                throw new Exception("Terms should have an exact value!");

            Matrix sigmo = GetResult();

            MMDerivative combined = new MMDerivative(s.D1, s.D2, D1, D2, false);

            //for (int i1 = 0; i1 < D1; i1++)
            //    for (int i2 = 0; i2 < D2; i2++)
            //        m[i1, i2, i1, i2] = sigmo[i1, i2] * (1 - sigmo[i1, i2]);

            int thisD1 = D1.Value;
            int thisD2 = D2.Value;
            for (int i1 = 0; i1 < s.D1; i1++)
                for (int i2 = 0; i2 < s.D2; i2++)
                {
                    Vectorization.ElementWise_A_MultipliedBy_1_Minus_A_MultipliedByB(sigmo.Array, s.Derivatives + i1 * s.D2 * s.D3 * s.D4 + i2 * s.D3 * s.D4, combined.Derivatives + i1 * s.D2 * s.D3 * s.D4 + i2 * s.D3 * s.D4, sigmo.D1 * sigmo.D2);
                    //for (int i3 = 0; i3 < thisD1; i3++)
                    //    for (int i4 = 0; i4 < thisD2; i4++)
                    //        combined[i1, i2, i3, i4] = s[i1, i2, i3, i4] * (sigmo[i3, i4] * (1 - sigmo[i3, i4]));
                }
            combined.Negative = s.Negative;

            Terms[0].Derivate(combined);
            combined.Dispose();
        }


        internal override Matrix CalculateResult()
        {
            if (!D1.HardEquals(D1) || !D2.HardEquals(D2))
                throw new Exception("Terms should have an exact value!");

            Matrix sigmo = new Matrix(D1, D2);
            Matrix v = Terms[0].GetResult();
            unsafe
            {
                Vectorization.Sigmoid(v.Array, sigmo.Array, sigmo.D1 * sigmo.D2);
            }
            return sigmo;
        }
    }
}
