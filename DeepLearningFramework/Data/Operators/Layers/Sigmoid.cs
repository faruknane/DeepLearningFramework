using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace DeepLearningFramework.Data.Operators.Layers
{
    public class Sigmoid : Layer
    {
        public Layer L { get; private set; }
        public override Dimension D1 { get; internal set; }
        public override Dimension D2 { get; internal set; }
        public Sigmoid(Layer x)
        {
            this.L = x;
            D1 = x.D1;
            D2 = x.D2;
            this.SequenceLength = L.SequenceLength;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override Terms.Term CreateTerm(int time)
        {
            var aa = new Terms.Sigmoid(L.GetTerm(time));
            return aa;
        }
        public override void DeleteTerms()
        {
            base.DeleteTerms();
            L.DeleteTerms();
        }
    }
}
