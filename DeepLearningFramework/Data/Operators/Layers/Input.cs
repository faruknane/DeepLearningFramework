using System;
using System.Collections.Generic;
using System.Text;
using DeepLearningFramework.Data;
using DeepLearningFramework.Data.Operators.Layers;
using DeepLearningFramework.Data.Operators.Terms;
using PerformanceWork.OptimizedNumerics;

namespace DeepLearningFramework.Data.Operators.Layers
{
    public class Input : Layer
    {
        public Input(int size)
        {
            this.Size = size;
        }

        public override Term CreateTerm(int time)
        {
            return new PlaceHolder(Size);
        }

        public void SetInput(int time, Matrix inp)
        {
            PlaceHolder h = (PlaceHolder)GetTerm(time);
            h.SetVariable(new Variable(inp.D1, inp.D2) { Name = "Input", Trainable = false, Weights = inp });
        }
    }
}
