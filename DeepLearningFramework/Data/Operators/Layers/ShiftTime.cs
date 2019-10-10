using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using DeepLearningFramework.Core;

namespace DeepLearningFramework.Data.Operators.Layers
{

    public class ShiftTime : Layer
    {
        Terms.Variable EmptyVariable;
        public Layer L { get; private set; }
        public override Dimension D1 { get; internal set; }
        public override Dimension D2 { get; internal set; }
        public int Shift { get; internal set; }
        public ShiftTime(Layer l, int shift)
        {
            this.L = l;
            D1 = l.D1;
            D2 = l.D2;
            this.SequenceLength = L.SequenceLength;
            this.Shift = shift;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public override Terms.Term CreateTerm(int time)
        {
            if(time >= this.SequenceLength - Shift || time < -Shift)
            {
                if (EmptyVariable == null)
                {
                    EmptyVariable = new Terms.Variable(D1, D2) { Trainable = false };
                    EmptyVariable.Weights.SetZero();
                }

                return EmptyVariable;
            }

            return L.GetTerm(time + Shift);
        }
        public override void DeleteTerms()
        {
            base.DeleteTerms();
            L.DeleteTerms();
        }
    }
}
