using DeepLearningFramework.Core;
using DeepLearningFramework.Operators.Terms;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Index = PerformanceWork.OptimizedNumerics.Index;

namespace DeepLearningFramework.Operators.Layers
{
    public class ReLU : Layer
    {
        public ReLU(Layer x)
        {
            this.InputLayers.Add(x);
            InnerDimensionCalculation();
            OuterDimensionCalculation();
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public override Term CreateTerm(Index time)
        {
            return new Terms.ReLU(InputLayers[0].GetTerm(time));
        }
    }
}
