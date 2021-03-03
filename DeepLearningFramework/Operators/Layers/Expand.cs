using DeepLearningFramework.Core;
using DeepLearningFramework.Operators.Terms;
using PerformanceWork.OptimizedNumerics;
using System.Runtime.CompilerServices;

namespace DeepLearningFramework.Operators.Layers
{
    public class Expand : Layer
    {
        public Dimension[] MultiplierDimensions { get; internal set; }
        public Shape MultiplierShape { get; internal set; }

        public Expand(Layer x, Dimension[] m)
        {
            this.InputLayers.Add(x);
            MultiplierDimensions = m;
            InnerDimensionCalculation();
            OuterDimensionCalculation();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public override void InnerDimensionCalculation()
        {
            this.InnerDimensions = new Dimension[InputLayers[0].InnerDimensions.Length];
            for (int i = 0; i < InnerDimensions.Length; i++)
                this.InnerDimensions[i] = new DimensionMultiply(InputLayers[0].InnerDimensions[i], MultiplierDimensions[i]);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public override unsafe void AfterPreCheck()
        {
            if (MultiplierShape == null)
                MultiplierShape = Shape.DimensionOf(MultiplierDimensions.Length);

            for (int i = 0; i < MultiplierShape.N; i++)
                MultiplierShape.Dimensions[i] = MultiplierDimensions[i];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public override Term CreateTerm(Index time)
        {
            return new Terms.Expand(InputLayers[0].GetTerm(time), MultiplierShape);
        }

    }
}
