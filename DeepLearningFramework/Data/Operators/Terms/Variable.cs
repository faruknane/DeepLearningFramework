using DeepLearningFramework.Core;
using PerformanceWork.OptimizedNumerics;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace DeepLearningFramework.Data.Operators.Terms
{
    public class Variable : Term, Trainable
    {
        private Matrix m;
        public String Name { get; set; }

        public int UniqueId { get; set; }
        public static int UniqueIdAssigner = 0;

        public bool Trainable { get; set; } = true;
        public float LearningRateMultiplier { get; set; } = 1;

        public Matrix Weights
        {
            get { return m; }
            set
            {
                if (value.D1 != m.D1 || value.D2 != m.D2)
                    throw new Exception("The Matrix should have the same dimensions with the Variable!");

                m = value;
            }
        }

        public override Dimension D1 { get; internal set; }
        public override Dimension D2 { get; internal set; }

        public Variable(int D1, int D2) // add initializer
        {
            Type = TermType.Variable;
            this.D1 = D1;
            this.D2 = D2;
            if (!this.D1.HardEquals(this.D1) || !this.D2.HardEquals(this.D2))
                throw new Exception("Terms should have an exact value!");
            m = new Matrix(D1, D2);
            UniqueId = UniqueIdAssigner;
            UniqueIdAssigner++;
        }
        public Variable(Matrix m) // add initializer
        {
            Type = TermType.Variable;
            this.D1 = m.D1;
            this.D2 = m.D2;
            if (!this.D1.HardEquals(this.D1) || !this.D2.HardEquals(this.D2))
                throw new Exception("Terms should have an exact value!");
            this.m = m;
            UniqueId = UniqueIdAssigner;
            UniqueIdAssigner++;
        }

        public void SetValue(Matrix n)
        {
            if (this.D1 != n.D1 || this.D2 != n.D2)
                throw new Exception("The Matrix should have the same dimensions with the Variable!");
            Weights = n;
        }
        public void SetValue(float[,] n)
        {
            if (this.D1 != n.GetLength(0) || this.D2 != n.GetLength(1))
                throw new Exception("The Matrix should have the same dimensions with the Variable!");
            Weights = n;
        }

        internal override Matrix CalculateResult()
        {
            return Weights;
        }

        public override void CalculateDerivate(MMDerivative s)
        {
            if (Trainable)
            {
                Hyperparameters.Optimizer.UpdateWeights(this, s);
            }
        }

        public override void CalculateHowManyTimesUsed()
        {
            Used++;
        }

        public override void DeleteResults()
        {
            base.DeleteResults();
        }
    }
}
