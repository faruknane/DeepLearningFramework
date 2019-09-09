using System;
using System.Collections.Generic;
using System.Text;
using DeepLearningFramework.Data.Operators.Terms;

namespace DeepLearningFramework.Data.Operators.Layers
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
            Term lossdiscrete = new Power(new Minus(PreviousLayer.GetTerm(time), Y.GetTerm(time)), 2);
            Term loss = new ShrinkByAdding(lossdiscrete, lossdiscrete.D1, lossdiscrete.D2);
            if(time > 0)
                loss = new Plus(this.GetTerm(time - 1), loss);
            return loss; 
        }
    }
}
