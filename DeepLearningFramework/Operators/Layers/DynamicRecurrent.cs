using DeepLearningFramework.Core;
using DeepLearningFramework.Operators.Terms;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Index = PerformanceWork.OptimizedNumerics.Index;

namespace DeepLearningFramework.Operators.Layers
{
    public class DynamicRecurrent : Layer
    {

        Func<Layer, List<Layer>, Index, Term> F;

        public DynamicRecurrent(Dimension[] outerDimensions, Dimension[] innerDimensions, Layer[] l, Func<Layer, List<Layer>, Index, Term> func)
        {
            foreach (var item in l)
                InputLayers.Add(item);

            this.OuterDimensions = outerDimensions;
            this.InnerDimensions = innerDimensions;
            this.F = func;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public override Term CreateTerm(Index time)
        {
            return F(this, InputLayers, time);
        }

    }
}
