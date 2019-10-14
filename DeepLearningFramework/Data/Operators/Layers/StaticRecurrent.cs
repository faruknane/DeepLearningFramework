using DeepLearningFramework.Data.Operators.Terms;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using DeepLearningFramework.Core;

namespace DeepLearningFramework.Data.Operators.Layers
{
    public class StaticRecurrent : Layer
    {
        public Layer L { get; internal set; }
        public override Dimension D1 { get; internal set; }
        public override Dimension D2 { get; internal set; }

        public Func<Layer, Layer, Layer> F;
        public Layer past;
        public Layer res;

        public StaticRecurrent(int size, Layer l, Func<Layer, Layer, Layer> func)
        {
            this.L = l;
            this.SequenceLength = L.SequenceLength;
            D1 = size;
            D2 = L.D2;
            this.past = new ShiftTime(this, -1);
            F = func;
            res = F(past, L);
            if (!res.D1.HardEquals(D1))
                throw new Exception("Dimensions should match!");
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public override Term CreateTerm(int time)
        {
            return res.GetTerm(time);
        }

        public override void DeleteTerms()
        {
            base.DeleteTerms();
            L.DeleteTerms();
            //past, res has already deleted their Terms.
        }
    }
}
