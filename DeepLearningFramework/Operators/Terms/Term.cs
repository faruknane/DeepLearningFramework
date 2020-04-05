
using PerformanceWork.OptimizedNumerics;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using DeepLearningFramework.Core;

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
        Embedding
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

        public bool IsDisposed = false;
        public bool InRecursion = false;

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
            this.CalculateHowManyTimesUsed();
            this.GetResult();
            Tensor I = Tensor.DerivativeIdentity(this.Shape, Data.Type.Float, DeviceIndicator.Host());
            this.Derivate(I);
            I.Dispose();
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public void Maximize()
        {
            this.DeleteResults();
            this.CalculateHowManyTimesUsed();
            this.GetResult();
            Tensor I = Tensor.DerivativeIdentity(this.Shape, Data.Type.Float, DeviceIndicator.Host());
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

            if (InRecursion) return;
            InRecursion = true;

            Used = 0;

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

            InRecursion = false;
        }

        public virtual void Dispose()
        {
            if (this.Type == TermType.Variable) return;
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
