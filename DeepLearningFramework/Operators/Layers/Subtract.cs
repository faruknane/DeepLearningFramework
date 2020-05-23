using DeepLearningFramework.Operators.Terms;
using System.Runtime.CompilerServices;
using Index = PerformanceWork.OptimizedNumerics.Index;

namespace DeepLearningFramework.Operators.Layers
{
    public class Subtract : Layer
    {
        public Subtract(Layer l1, Layer l2)
        {
            InputLayers.Add(l1);
            InputLayers.Add(l2);

            InnerDimensionCalculation();
            OuterDimensionCalculation();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public override Term CreateTerm(Index time)
        {
            return new Terms.Subtract(InputLayers[0].GetTerm(time), InputLayers[1].GetTerm(time));
        }
    }
}
