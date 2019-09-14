using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using DeepLearningFramework.Data.Operators.Terms;

namespace DeepLearningFramework.Data.Operators.Layers
{
    public class ExpandWithSame : Layer
    {
        public Layer L { get; private set; }
        public override Dimension D1 { get; internal set; }
        public override Dimension D2 { get; internal set; }
        public Dimension RowMultiplier { get; internal set; }
        public Dimension ColumnMultiplier { get; internal set; }
        public ExpandWithSame(Layer l1, Dimension rowmult, Dimension colmult) 
        {
            this.L = l1;
            this.RowMultiplier = rowmult;
            this.ColumnMultiplier = colmult;
            D1 = l1.D1 * RowMultiplier;
            D2 = l1.D2 * ColumnMultiplier;
            this.SequenceLength = L.SequenceLength;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override Term CreateTerm(int time)
        {
            return new Terms.ExpandWithSame(L.GetTerm(time), RowMultiplier,ColumnMultiplier);
        }

        public override void DeleteTerms()
        {
            base.DeleteTerms();
            L.DeleteTerms();
        }
    }
}
