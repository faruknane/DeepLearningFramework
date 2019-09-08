using System;
using DeepLearningFramework.Data;
using DeepLearningFramework.Data.Operators.Terms;
using PerformanceWork.OptimizedNumerics;

namespace DeepLearningFramework.Data.Operators.Layers
{
    public class Dense : Layer
    {
        public static float[] Randomize(float[] a)
        {
            Random r = new Random();
            for (int i = 0; i < a.Length; i++)
                a[i] = (float)(r.NextDouble() * 2 - 1);
            return a;
        }

        Layer prev;

        public Layer PreviousLayer
        {
            get
            {
                return prev;
            }
            set // make private later
            {
                Terms.Clear();
                if (prev != null && prev.Size == value.Size)
                {
                    prev = value;
                    return;
                } // We are able to bind the layers to more than one layer with the same weights.
                prev = value;
                W = new Variable(Size, prev.Size);
                B = new Variable(Size, 1);
                W.Name = "W";
                B.Name = "B";
                Randomize(W.Weights.Array);
                Randomize(B.Weights.Array);
            }
        }

        public Variable W, B;

        public Dense(int size)
        {
            this.Size = size;
        }
        public Dense(int size, string act)
        {
            this.Size = size;
            this.Activation = act;
        }

        public override Term CreateTerm(int time)
        {
            Term x = PreviousLayer.GetTerm(time);
            if(Activation == "sigmoid")
                return new Sigmoid( new Plus(new MatrixMultiply(W, x), new ExpandWithSame(B, 1, x.D2)));

            return new Plus(new MatrixMultiply(W, x), new ExpandWithSame(B, 1, x.D2));
        }
    }
}
