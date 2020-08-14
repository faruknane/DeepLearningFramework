﻿
using DeepLearningFramework.Core;
using PerformanceWork;
using PerformanceWork.OptimizedNumerics;
using System;
using System.Runtime.CompilerServices;

namespace DeepLearningFramework.Operators.Terms
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
        Embedding,
        ReLU,
        Multiply,
        DropoutProbability,
        Dropout
    }

    public abstract class Term : IDisposable
    {
        public Shape Shape { get; internal set; }

        public Term[] Terms;
        public Tensor Result { get; internal set; }
        public TermType Type { get; internal set; }

        public Tensor SumOfDerivatives;
        public int UniqueId { get; set; } = Helper.Id.GetNewId();

        volatile internal int Used = 0;

        public bool IsDisposed { get; internal set; } = false;
        public bool ContainsTrainable { get; internal set; } = false;
        internal bool IsCalculated = false;

        //add
        //contains trainable variable ? 
        //is variable ? 
        //how many times used

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public abstract Tensor CalculateResult();

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public virtual Tensor GetResult()
        {
            lock (Terms)
            {
                if (Result == null)
                {
                    return Result = CalculateResult();
                }
                return Result;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public abstract void CalculateDerivate(Tensor s);

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public void Minimize()
        {
            this.DeleteResults();
            this.CalculateContainsTrainable();
            this.CalculateHowManyTimesUsed();
            this.GetResult();
            Tensor I = Tensor.DerivativeIdentity(this.Shape, DataType.Type.Float, DeviceIndicator.Host());
            this.Derivate(I);
            I.Dispose();
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public void Maximize()
        {
            this.DeleteResults();
            this.CalculateContainsTrainable();
            this.CalculateHowManyTimesUsed();
            this.GetResult();
            Tensor I = Tensor.DerivativeIdentity(this.Shape, DataType.Type.Float, DeviceIndicator.Host());
            I.MakeNegative();
            this.Derivate(I);
            I.Dispose();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public void Derivate(Tensor m)
        {
            lock (Shape)
            {
                //Console.WriteLine(UniqueId + ", " + Type + ", " + Terms.Length +  " -> " + Used);
                if (Used <= 0)
                    throw new Exception("Impossible case a!");

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
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        private void AddDerivative(Tensor m)
        {
            if (SumOfDerivatives == null)
            {
                SumOfDerivatives = Tensor.Clone(m);
            }
            else
            {
                SumOfDerivatives.AddTensor(m);
            }
        }

        public virtual void CalculateContainsTrainable()
        {
            if (IsCalculated) return;
            IsCalculated = true;

            for (int i = 0; i < Terms.Length; i++)
                Terms[i].CalculateContainsTrainable();

            ContainsTrainable = false;
            for (int i = 0; i < Terms.Length; i++)
                ContainsTrainable = ContainsTrainable || Terms[i].ContainsTrainable;
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
        public virtual void DeleteResults()
        {
            if (SumOfDerivatives == null && Result == null) return;

            Used = 0;
            IsCalculated = false;

            if (SumOfDerivatives != null && !SumOfDerivatives.ArrayReturned)
            {
                SumOfDerivatives.Dispose();
                SumOfDerivatives = null;
            }

            if (Result != null && !Result.ArrayReturned)
            {
                Result.Dispose();
                Result = null;
            }

            for (int i = 0; i < Terms.Length; i++)
                Terms[i].DeleteResults();

        }

        public virtual void Dispose()
        {
            if (this.Type == TermType.Variable) return;
            if (IsDisposed) return;
            IsDisposed = true;
            DeleteResults();

            for (int i = 0; i < Terms.Length; i++)
                if (Terms[i].Type != TermType.Variable)
                    Terms[i].Dispose();

            GC.SuppressFinalize(this);
        }

    }
}
