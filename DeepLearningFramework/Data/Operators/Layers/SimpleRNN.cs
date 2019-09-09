using DeepLearningFramework.Data.Operators.Terms;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeepLearningFramework.Data.Operators.Layers
{
    public class SimpleRNN : Layer
    {
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
                Dense.Randomize(W.Weights.Array);
                Dense.Randomize(B.Weights.Array);
            }
        }

        public Variable W, B, WH;

        public SimpleRNN(int size)
        {
            this.Size = size;
            WH = new Variable(Size, Size);
            WH.Name = "WH";

            //delete this after
            WH.Weights.SetZero();
            for (int i = 0; i < WH.D1; i++)
                    WH.Weights[i, i] = 1;
            //
        }

        public override Term CreateTerm(int time)
        {
            Term x = PreviousLayer.GetTerm(time);
            x = new Plus(new MatrixMultiply(W, x), new ExpandWithSame(B, 1, x.D2));

            if (time > 0)
                return new Plus(new MatrixMultiply(WH, GetTerm(time - 1)), x);
            return x;
        }
    }
}
