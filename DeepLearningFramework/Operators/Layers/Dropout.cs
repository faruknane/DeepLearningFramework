using DeepLearningFramework.Core;
using DeepLearningFramework.Operators.Terms;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Index = PerformanceWork.OptimizedNumerics.Index;


namespace DeepLearningFramework.Operators.Layers
{
    public class Dropout : Layer
    {
        public float Probability;
        public Dropout(Layer x, float prob)
        {
            InputLayers.Add(x);
            InnerDimensionCalculation();
            OuterDimensionCalculation();
            Probability = prob;
        }

        public override Term CreateTerm(Index time)
        {
            Term x = InputLayers[0].GetTerm(time);
            return new Multiply(new DropoutProbability(x.Shape, Probability), x);
        }
    }
}
