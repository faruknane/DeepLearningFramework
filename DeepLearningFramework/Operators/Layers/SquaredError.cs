using System;
using System.Collections.Generic;
using System.Text;
using DeepLearningFramework.Operators.Terms;

namespace DeepLearningFramework.Operators.Layers
{
    public class SquaredError : Layer
    {
        Layer prev;
        Layer y;

        public Layer PreviousLayer
        {
            get
            {
                return prev;
            }
            set
            {
                Terms.Clear();
                prev = value;
            }
        }
        public Layer Y
        {
            get
            {
                return y;
            }
            set
            {
                Terms.Clear();
                y = value;
            }
        }

        public SquaredError(Layer x1, Layer x2)
        {
            PreviousLayer = x1;
            Y = x2;
        }

        public override Term CreateTerm(int time)
        {
            var lossdiscrete = new Power(new Minus(PreviousLayer.GetTerm(time), Y.GetTerm(time)), 2);
            var loss = new ShrinkByAdding(lossdiscrete, lossdiscrete.D1, lossdiscrete.D2);
            return loss; 
        }
    }
}
