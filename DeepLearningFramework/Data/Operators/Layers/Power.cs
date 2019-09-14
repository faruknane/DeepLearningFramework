using DeepLearningFramework.Data.Operators.Terms;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace DeepLearningFramework.Data.Operators.Layers
{
    public class Power : Layer
    {
        public Layer L { get; private set; }
        public int PowerOf { get; private set; }
        public override Dimension D1 { get; internal set; }
        public override Dimension D2 { get; internal set; }
        
        public Power(Layer l1, int pow)
        {
            this.L = l1;
            this.PowerOf = pow;
            D1 = l1.D1;
            D2 = l1.D2;
            this.SequenceLength = L.SequenceLength;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override Term CreateTerm(int time)
        {
            return new Terms.Power(L.GetTerm(time), PowerOf);
        }
        public override void DeleteTerms()
        {
            base.DeleteTerms();
            L.DeleteTerms();
        }
    }
}
