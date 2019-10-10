using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using DeepLearningFramework.Core;

namespace DeepLearningFramework.Data.Operators.Layers
{
    public class SoftMax : Layer
    {
        public Layer L { get; private set; }
        public override Dimension D1 { get; internal set; }
        public override Dimension D2 { get; internal set; }
        public SoftMax(Layer x)
        {
            this.L = x;
            D1 = x.D1;
            D2 = x.D2;
            this.SequenceLength = L.SequenceLength;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public override Terms.Term CreateTerm(int time)
        {
            var aa = new Terms.SoftMax(L.GetTerm(time));
            return aa;
        }
        public override void DeleteTerms()
        {
            base.DeleteTerms();
            L.DeleteTerms();
        }
    }
}
