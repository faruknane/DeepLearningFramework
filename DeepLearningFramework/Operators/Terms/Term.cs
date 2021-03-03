
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

    public abstract class Term
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
            this.PreCalculation();
            this.GetResult();
            Tensor I = Tensor.DerivativeIdentity(this.Shape, DeviceConfig.Host_Float);
            this.Derivate(I);
            I.Dispose();
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public void Maximize()
        {
            this.DeleteResults();
            this.PreCalculation();
            this.GetResult();
            Tensor I = Tensor.DerivativeIdentity(this.Shape, DeviceConfig.Host_Float);
            I.MakeNegative();
            this.Derivate(I);
            I.Dispose();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public void Derivate(Tensor m)
        {
            lock (Shape)
            {
                if (!ContainsTrainable)
                    return;

                //Console.WriteLine(UniqueId + ", " + Type + ", " + Terms.Length +  " -> " + Used);
                if (Used <= 0)
                    throw new Exception("Impossible case, cannot calculate derivative of unused term!");

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

        public virtual void PreCalculation()
        {
            if (Used == 0)
            {
                for (int i = 0; i < Terms.Length; i++)
                    Terms[i].PreCalculation();

                ContainsTrainable = false;
                for (int i = 0; i < Terms.Length; i++)
                    ContainsTrainable = ContainsTrainable || Terms[i].ContainsTrainable;
            }
            Used++;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public virtual void DeleteResults()
        {
            if (SumOfDerivatives == null && Result == null) return;

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

        }
    }
}
