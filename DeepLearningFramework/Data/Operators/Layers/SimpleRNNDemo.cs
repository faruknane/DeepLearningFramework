using DeepLearningFramework.Data.Operators.Terms;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace DeepLearningFramework.Data.Operators.Layers
{
    public class SimpleRNNDemo : Layer
    {
        public Layer L { get; internal set; }
        public override Dimension D1 { get; internal set; }
        public override Dimension D2 { get; internal set; }

        Terms.Variable W, B, WH;

        public SimpleRNNDemo(int size, Layer l)
        {
            this.L = l;
            D1 = size;
            D2 = l.D2;
            WH = new Terms.Variable(D1, D1);
            W = new Terms.Variable(D1, l.D1);
            B = new Terms.Variable(D1, 1);
            this.SequenceLength = L.SequenceLength;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public override Term CreateTerm(int time)
        {
            Term x = L.GetTerm(time);
            x = new Terms.Plus(new Terms.MatrixMultiply(W, x), new Terms.ExpandWithSame(B, 1, x.D2));

            if (time > 0)
                return new Terms.Plus(new Terms.MatrixMultiply(WH, GetTerm(time - 1)), x);
            return x;
        }

        public override void DeleteTerms()
        {
            base.DeleteTerms();
            L.DeleteTerms();
        }
    }
}
