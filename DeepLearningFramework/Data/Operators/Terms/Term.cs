using DeepLearningFramework.Data;
using PerformanceWork.OptimizedNumerics;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeepLearningFramework.Data.Operators.Terms
{
    public enum TermType
    {
        ExpandWithSame,
        MatrixMultiply,
        Minus,
        MultiplyByNumber,
        PlaceHolder,
        Plus,
        Power,
        ShrinkByAdding,
        Sigmoid,
        SoftMax,
        Variable,
        Experimental
    }
    public class Term
    {
        public virtual Dimension D1 { get; internal set; } //Assign in in initializer.
        public virtual Dimension D2 { get; internal set; }
        public Matrix Result { get; internal set; }
        public TermType Type { get; internal set; }

        internal MMDerivative SumOfDerivative;

        internal int Used = 0;

        //add
        //contains trainable variable ? 
        //is variable ? 
        //how many times used

        internal virtual Matrix CalculateResult()
        {
            throw new NotImplementedException();
        }

        public virtual Matrix GetResult()
        {
            if(Result == null)
            {
                return Result = CalculateResult();
            }
            return Result;
        }

        public virtual void CalculateDerivate(MMDerivative s)
        {
            throw new NotImplementedException();
        }

        public virtual void Minimize()
        {
            this.DeleteResults();
            this.CalculateHowManyTimesUsed();
            MMDerivative I = MMDerivative.I(D1, D2);
            this.Derivate(I);
            I.Dispose();
        }


        public virtual void Maximize()
        {
            this.DeleteResults();
            this.CalculateHowManyTimesUsed();
            MMDerivative I = MMDerivative.I(D1, D2);
            I.Negative = true;
            this.Derivate(I);
            I.Dispose();
        }

        public void Derivate(MMDerivative m)
        {
            if (Used <= 0)
                throw new Exception("Impossible case!");

            Used--;

            if (Used == 0 && SumOfDerivative == null)
            {
                CalculateDerivate(m);
                return;
            }

            AddDerivative(m);

            if (Used == 0)
            {
                CalculateDerivate(SumOfDerivative);
            }
            //Console.WriteLine(Type.ToString() + " -> " + Used);
        }

        private void AddDerivative(MMDerivative m)
        {
            if (SumOfDerivative == null)
            {
                SumOfDerivative = MMDerivative.Clone(m);
            }
            else
            {
                SumOfDerivative.Add(m);
            }

            //if (Type == TermType.Minus)
            //{
            //    Console.WriteLine("SumOfDerivative -> " + SumOfDerivative.Derivatives[0] + ", " + SumOfDerivative.Derivatives[1] + ", " + SumOfDerivative.Derivatives[2]);
            //    Console.WriteLine("m -> " + m.Derivatives[0] + ", " + m.Derivatives[1] + ", " + m.Derivatives[2]);
            //}
        }

        public virtual void CalculateHowManyTimesUsed()
        {
            throw new NotImplementedException();
        }

        public virtual void DeleteResults()
        {
            if (SumOfDerivative != null)
            {
                SumOfDerivative.Dispose();
                SumOfDerivative = null;
            }

            Used = 0;

            if (Result != null)
            {
                if(Type != TermType.Variable && Type != TermType.PlaceHolder)
                    Result.Dispose();
                Result = null;
            }
        }
    }
}
