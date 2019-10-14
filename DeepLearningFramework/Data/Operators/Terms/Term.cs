using DeepLearningFramework.Data;
using PerformanceWork.OptimizedNumerics;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using DeepLearningFramework.Core;

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
        Experimental,
        Embedding
    }

    public abstract class Term
    {
        public virtual Dimension D1 { get; internal set; } //Assign in in initializer.
        public virtual Dimension D2 { get; internal set; }
        public Term[] Terms;
        public Matrix Result { get; internal set; }
        public TermType Type { get; internal set; }

        internal MMDerivative SumOfDerivative;

        internal int Used = 0;

        //add
        //contains trainable variable ? 
        //is variable ? 
        //how many times used

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        internal abstract Matrix CalculateResult();

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public virtual Matrix GetResult()
        {
            if (Result == null)
            {
                return Result = CalculateResult();
            }
            return Result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public abstract void CalculateDerivate(MMDerivative s);

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public void Minimize()
        {
            this.DeleteResults();
            this.CalculateHowManyTimesUsed();
            MMDerivative I = MMDerivative.I(D1, D2);
            this.Derivate(I);
            I.Dispose();
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public void Maximize()
        {
            this.DeleteResults();
            this.CalculateHowManyTimesUsed();
            MMDerivative I = MMDerivative.I(D1, D2);
            I.Negative = true;
            this.Derivate(I);
            I.Dispose();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
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

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
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

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public void CalculateHowManyTimesUsed()
        {
            if (Used == 0)
            {
                for (int i = 0; i < Terms.Length; i++)
                    Terms[i].CalculateHowManyTimesUsed();
            }
            Used++;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public void DeleteResults()
        {
            Used = 0;

            if (SumOfDerivative != null)
            {
                SumOfDerivative.Dispose();
                SumOfDerivative = null;
            }

            if (Result != null)
            {
                if (Type != TermType.Variable && Type != TermType.PlaceHolder)
                    Result.Dispose();
                Result = null;
            }

            for (int i = 0; i < Terms.Length; i++)
                Terms[i].DeleteResults();
        }


    }
}
