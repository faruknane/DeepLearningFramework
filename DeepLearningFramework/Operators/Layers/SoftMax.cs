using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using DeepLearningFramework.Core;
using DeepLearningFramework.Operators.Terms;
using PerformanceWork.OptimizedNumerics;

namespace DeepLearningFramework.Operators.Layers
{
    public class SoftMax : Layer
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public SoftMax(Layer x)
        {
            this.InputLayers.Add(x);
            this.OuterShape = new Dimension[x.OuterShape.Length];
            this.InnerShape = new Dimension[x.InnerShape.Length]; 
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public override Term CreateTerm(Index time)
        {
            return new Terms.SoftMax(InputLayers[0].GetTerm(time));
        }

    }
}
