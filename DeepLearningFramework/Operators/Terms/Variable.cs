using DeepLearningFramework.Core;
using PerformanceWork.OptimizedNumerics;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace DeepLearningFramework.Operators.Terms
{
    public class Variable : Term, Trainable
    {
        private Tensor<float> m;
        public String Name { get; set; }
        public bool Trainable { get; set; } = true;
        public float LearningRateMultiplier { get; set; } = 1;

        public Tensor<float> Weights
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


        public Variable(Shape s) // add initializer
        {
            Type = TermType.Variable;
            this.Shape = s;
            m = new Tensor<float>(s.Clone());
            Terms = Array.Empty<Term>();
        }

        public Variable(Tensor<float> m) // add initializer
        {
            Type = TermType.Variable;
            this.Shape = m.Shape.Clone();
            this.m = m;
            Terms = Array.Empty<Term>();
        }

        public void SetValue(Tensor<float> n)
        {
            Weights = n;
        }

        public unsafe void SetValue(float[] n)
        {
            for (int i = 0; i < this.Shape.N; i++)
                if (this.Shape[i] != n.GetLength(i))
                    throw new Exception("The Matrix should have the same dimensions with the Variable!");

            fixed (float* ptr = n)
            {
                Vectorization.ElementWiseAssignAVX((float*)this.Weights.Array, ptr, this.Shape.TotalSize);
            }
        }
        public unsafe void SetValue(float[,] n)
        {
            for (int i = 0; i < this.Shape.N; i++)
                if (this.Shape[i] != n.GetLength(i))
                    throw new Exception("The Matrix should have the same dimensions with the Variable!");

            fixed (float* ptr = n)
            {
                Vectorization.ElementWiseAssignAVX((float*)this.Weights.Array, ptr, this.Shape.TotalSize);
            }
        }

        public unsafe void SetValue(float[,,] n)
        {
            for (int i = 0; i < this.Shape.N; i++)
                if (this.Shape[i] != n.GetLength(i))
                    throw new Exception("The Matrix should have the same dimensions with the Variable!");

            fixed (float* ptr = n)
            {
                Vectorization.ElementWiseAssignAVX((float*)this.Weights.Array, ptr, this.Shape.TotalSize);
            }
        }

        public override Tensor<float> CalculateResult()
        {
            return Tensor<float>.Clone(Weights);
        }

        public override void CalculateDerivate(Tensor<float> s)
        {
            if (Trainable)
            {
                Hyperparameters.Optimizer.UpdateWeights(this, s);
            }
        }


        public void Clean()
        {
            if(Weights != null)
                Weights.Dispose();
            DeleteResults();
        }

    }
}
