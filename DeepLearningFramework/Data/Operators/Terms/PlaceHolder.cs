﻿using PerformanceWork.OptimizedNumerics;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeepLearningFramework.Data.Operators.Terms
{
    public class PlaceHolder : Term
    {
        Term v1;

        public override Dimension D1 { get; internal set; }
        public override Dimension D2 { get; internal set; }

        public PlaceHolder(int Size)
        {
            Type = TermType.PlaceHolder;
            D1 = Size;
            D2 = new Dimension();
        }
        public void SetVariable(Variable v)
        {
            if(v1 != null)
            {
                Variable v11 = (Variable)v1;
                v11.Weights.Dispose();
                v11 = null;
                v1 = null;
            }
            v1 = v;
            if (!D1.HardEquals(v1.D1))
                throw new Exception("Placeholder!");
            D2.Value = v.D2.Value;
        }

        public override void CalculateDerivate(MMDerivative s)
        {
            if (!D1.HardEquals(D1) || !D2.HardEquals(D2))
                throw new Exception("Terms should have an exact value!");
            v1.Derivate(s);
        }

        internal override Matrix CalculateResult()
        {
            if (!D1.HardEquals(D1) || !D2.HardEquals(D2))
                throw new Exception("Terms should have an exact value!");
            return v1.GetResult();
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