using DeepLearningFramework.Operators.Terms;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using DeepLearningFramework.Core;
using Index = PerformanceWork.OptimizedNumerics.Index;

namespace DeepLearningFramework.Operators.Layers
{
    public class Power : Layer
    {
        public int PowerOf { get; private set; }

        public Power(Layer l, int pow)
        {
            this.InputLayers.Add(l);
            this.PowerOf = pow;

            InnerDimensionCalculation();
            OuterDimensionCalculation();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public override Term CreateTerm(Index time)
        {
            return new Terms.Power(InputLayers[0].GetTerm(time), PowerOf);
        }



    }
}
