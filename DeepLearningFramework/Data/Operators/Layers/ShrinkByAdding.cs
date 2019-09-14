using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using DeepLearningFramework.Data.Operators.Terms;

namespace DeepLearningFramework.Data.Operators.Layers
{
    public class ShrinkSizeToOneByAdding : Layer
    {
        public Layer L { get; private set; }
        public override Dimension D1 { get; internal set; }
        public override Dimension D2 { get; internal set; }
        public ShrinkSizeToOneByAdding(Layer x)
        {
            this.L = x;
            D1 = 1;
            D2 = 1;
            this.SequenceLength = L.SequenceLength;
        }

        public override Term CreateTerm(int time)
        {
            Term t = L.GetTerm(time);
            return new Terms.ShrinkByAdding(t, t.D1, t.D2);
        }

        public override void DeleteTerms()
        {
            base.DeleteTerms();
            L.DeleteTerms();
        }

    }
    public class SumSequenceToOneByAdding : Layer
    {
        public Layer L { get; private set; }
        public override Dimension D1 { get; internal set; }
        public override Dimension D2 { get; internal set; }
        public SumSequenceToOneByAdding(Layer x)
        {
            this.L = x;
            D1 = x.D1;
            D2 = x.D2;
            this.SequenceLength = 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override Term CreateTerm(int time)
        {
            Term t = null;
            for (int i = 0; i < L.SequenceLength; i++)
            {
                if (t == null)
                    t = L.GetTerm(i);
                else
                    t = new Terms.Plus(t, L.GetTerm(i));
            }
            return t;
        }
        public override void DeleteTerms()
        {
            base.DeleteTerms();
            L.DeleteTerms();
        }
    }

    public class SumSequenceByAdding : Layer
    {
        public Layer L { get; private set; }
        public override Dimension D1 { get; internal set; }
        public override Dimension D2 { get; internal set; }
        public SumSequenceByAdding(Layer x)
        {
            this.L = x;
            D1 = x.D1;
            D2 = x.D2;
            this.SequenceLength = this.L.SequenceLength;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override Term CreateTerm(int time)
        {
            Term t = L.GetTerm(time);
            if (time > 0)
                t = new Terms.Plus(L.GetTerm(time - 1), t);
            return t;
        }
        public override void DeleteTerms()
        {
            base.DeleteTerms();
            L.DeleteTerms();
        }
    }
}
