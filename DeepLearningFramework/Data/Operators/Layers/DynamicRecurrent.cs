using DeepLearningFramework.Core;
using DeepLearningFramework.Data.Operators.Terms;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace DeepLearningFramework.Data.Operators.Layers
{
    public class DynamicRecurrent : Layer
    {
        public Layer L { get; internal set; }
        public override Dimension D1 { get; internal set; }
        public override Dimension D2 { get; internal set; }

        Func<Layer, Layer, int, Term> F;

        Terms.Variable EmptyVariable;

        public DynamicRecurrent(int size, Layer l, Func<Layer, Layer, int, Term> func)
        {
            this.L = l;
            this.SequenceLength = L.SequenceLength;
            D1 = size;
            D2 = L.D2;
            this.F = func;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public override Term CreateTerm(int time)
        {
            Term res = F(this, L, time);
            if (!res.D1.HardEquals(D1) || !res.D2.HardEquals(D2))
                throw new Exception($"DynamicRecurrent has returned {res.D1.Value} x {res.D2.Value}. Expected: {D1.Value} x {D2.Value}");
            return res;
        }

        public override void DeleteTerms()
        {
            base.DeleteTerms();
            L.DeleteTerms();
        }
    }
}
