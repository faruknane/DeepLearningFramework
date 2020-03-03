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
        Plus,
        Power,
        ShrinkByAdding,
        Sigmoid,
        SoftMax,
        Variable,
        Experimental,
        Embedding
    }

    public abstract class Term : IDisposable
    {
        public Shape Shape { get; internal set; } 

        public Term[] Terms;
        public Tensor<float> Result { get; internal set; }
        public TermType Type { get; internal set; }

        public Tensor<float> SumOfDerivatives;

        internal int Used = 0;

        public bool IsDisposed = false;

        //add
        //contains trainable variable ? 
        //is variable ? 
        //how many times used

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public abstract Tensor<float> CalculateResult();

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public virtual Tensor<float> GetResult()
        {
            if (Result == null)
            {
                return Result = CalculateResult();
            }
            return Result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public abstract void CalculateDerivate(Tensor<float> s);

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public void Minimize()
        {
            this.DeleteResults();
            this.CalculateHowManyTimesUsed();
            this.GetResult();
            Tensor<float> I = Tensor<float>.DerivativeIdentity(this.Shape);
            this.Derivate(I);
            I.Dispose();
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public void Maximize()
        {
            this.DeleteResults();
            this.CalculateHowManyTimesUsed();
            this.GetResult();
            Tensor<float> I = Tensor<float>.DerivativeIdentity(this.Shape);
            I.MakeNegative();
            this.Derivate(I);
            I.Dispose();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public void Derivate(Tensor<float> m)
        {
            if (Used <= 0)
                throw new Exception("Impossible case!");

            Used--;

            if (Used == 0 && SumOfDerivatives == null)
            {
                CalculateDerivate(m);
                return;
            }

            AddDerivative(m);

            if (Used == 0)
            {
                CalculateDerivate(SumOfDerivatives);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        private void AddDerivative(Tensor<float> m)
        {
            if (SumOfDerivatives == null)
            {
                SumOfDerivatives = Tensor<float>.Clone(m);
            }
            else
            {
                SumOfDerivatives.Add(m);
            }
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
            if (Result == null) return;

            Used = 0;

            if (SumOfDerivatives != null)
            {
                SumOfDerivatives.Dispose();
                SumOfDerivatives = null;
            }

            if (Result != null)
            {
                Result.Dispose();
                Result = null;
            }

            for (int i = 0; i < Terms.Length; i++)
                Terms[i].DeleteResults();
        }

        public virtual void Dispose()
        {
            if (IsDisposed) return;
            IsDisposed = true;
            DeleteResults();
            Shape.Return(this.Shape);

            for (int i = 0; i < Terms.Length; i++)
                if (Terms[i].Type != TermType.Variable)
                    Terms[i].Dispose();

            GC.SuppressFinalize(this);
        }

    }
}
