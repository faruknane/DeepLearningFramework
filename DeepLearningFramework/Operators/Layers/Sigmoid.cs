using DeepLearningFramework.Core;
using DeepLearningFramework.Operators.Terms;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Index = PerformanceWork.OptimizedNumerics.Index;

namespace DeepLearningFramework.Operators.Layers
{
    public class Sigmoid : Layer
    {
        public Sigmoid(Layer x)
        {
            this.InputLayers.Add(x);
            this.OuterDimensions = new Dimension[x.OuterDimensions.Length];
            this.InnerDimensions = new Dimension[x.InnerDimensions.Length];
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public override Term CreateTerm(Index time)
        {
            return new Terms.Sigmoid(InputLayers[0].GetTerm(time));
        }
    }
}
