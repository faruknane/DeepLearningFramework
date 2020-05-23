using DeepLearningFramework.Operators.Terms;
using PerformanceWork.OptimizedNumerics;
using System.Runtime.CompilerServices;

namespace DeepLearningFramework.Operators.Layers
{
    public class SoftMax : Layer
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public SoftMax(Layer x)
        {
            this.InputLayers.Add(x);

            InnerDimensionCalculation();
            OuterDimensionCalculation();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public override Term CreateTerm(Index time)
        {
            return new Terms.SoftMax(InputLayers[0].GetTerm(time));
        }

    }
}
