using DeepLearningFramework.Operators.Terms;
using System.Runtime.CompilerServices;
using Index = PerformanceWork.OptimizedNumerics.Index;

namespace DeepLearningFramework.Operators.Layers
{
    public class Sigmoid : Layer
    {
        public Sigmoid(Layer x)
        {
            this.InputLayers.Add(x);
            InnerDimensionCalculation();
            OuterDimensionCalculation();
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public override Term CreateTerm(Index time)
        {
            return new Terms.Sigmoid(InputLayers[0].GetTerm(time));
        }
    }
}
