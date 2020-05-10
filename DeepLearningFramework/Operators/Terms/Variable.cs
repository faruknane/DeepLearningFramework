using DeepLearningFramework.Core;
using PerformanceWork;
using PerformanceWork.OptimizedNumerics;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace DeepLearningFramework.Operators.Terms
{
    public class Variable : Term, Trainable
    {
        private Tensor m;
        public String Name { get; set; }
        public bool Trainable { get; set; } = true;
        public float LearningRateMultiplier { get; set; } = 1;

        public Tensor Weights
        {
            get { return m; }
            set
            {
                if(this.Shape.N != value.Shape.N)
                    throw new Exception("The Tensor should have the same dimensions with the Variable!");

                for(int i = 0; i < this.Shape.N; i++)
                    if (this.Shape[i] != value.Shape[i])
                        throw new Exception("The Matrix should have the same dimensions with the Variable!");

                m = value;
            }
        }


        public Variable(Shape s)
        {
            Type = TermType.Variable;
            this.Shape = s;
            m = new Tensor(s.Clone(), DataType.Type.Float, DeviceIndicator.Host());
            Terms = Array.Empty<Term>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public Variable(Tensor m)
        {
            Type = TermType.Variable;
            this.Shape = m.Shape.Clone();
            this.m = m;
            Terms = Array.Empty<Term>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public void SetValue(Tensor n)
        {
            Weights = n;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public unsafe void SetValue(float[] n)
        {
            for (int i = 0; i < this.Shape.N; i++)
                if (this.Shape[i] != n.GetLength(i))
                    throw new Exception("The Matrix should have the same dimensions with the Variable!");

            fixed (float* ptr = n)
            {
                VectorizationFloat.ElementWiseAssignAVX((float*)this.Weights.Array, ptr, this.Shape.TotalSize);
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public unsafe void SetValue(float[,] n)
        {
            for (int i = 0; i < this.Shape.N; i++)
                if (this.Shape[i] != n.GetLength(i))
                    throw new Exception("The Matrix should have the same dimensions with the Variable!");

            fixed (float* ptr = n)
            {
                VectorizationFloat.ElementWiseAssignAVX((float*)this.Weights.Array, ptr, this.Shape.TotalSize);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public unsafe void SetValue(float[,,] n)
        {
            for (int i = 0; i < this.Shape.N; i++)
                if (this.Shape[i] != n.GetLength(i))
                    throw new Exception("The Matrix should have the same dimensions with the Variable!");

            fixed (float* ptr = n)
            {
                VectorizationFloat.ElementWiseAssignAVX((float*)this.Weights.Array, ptr, this.Shape.TotalSize);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public override Tensor CalculateResult()
        {
            if(Trainable)
                return Tensor.Clone(Weights);
            else
                return Weights;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public override void CalculateDerivate(Tensor s)
        {
            if (Trainable)
            {
                Hyperparameters.Optimizer.UpdateWeights(this, s);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public override void DeleteResults()
        {
            if (InRecursion) return;
            InRecursion = true;

            Used = 0;

            if (SumOfDerivatives != null && !SumOfDerivatives.ArrayReturned)
            {
                SumOfDerivatives.Dispose();
                SumOfDerivatives = null;
            }

            if (Result != null && !Result.ArrayReturned && Result != Weights)
            {
                Result.Dispose();
                Result = null;
            }

            for (int i = 0; i < Terms.Length; i++)
                Terms[i].DeleteResults();

            InRecursion = false;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public void Clean()
        {
            if (IsDisposed) return;
            IsDisposed = true;

            if (Weights != null && !Weights.ArrayReturned)
            {
                m.Dispose();
                m = null;
            }
            DeleteResults();
            Shape.Return(this.Shape);
            GC.SuppressFinalize(this);
        }

    }
}
