using DeepLearningFramework.Core;
using DeepLearningFramework.Operators.Terms;
using PerformanceWork.OptimizedNumerics;
using System.Runtime.CompilerServices;
using Index = PerformanceWork.OptimizedNumerics.Index;

namespace DeepLearningFramework.Operators.Layers
{

    public class ShiftTime : Layer
    {
        public Shape ShiftShape { get; internal set; }
        public Dimension[] ShiftDimensions { get; internal set; }

        public ShiftTime(Layer l, Dimension[] shift)
        {
            this.InputLayers.Add(l);

            InnerDimensionCalculation();
            OuterDimensionCalculation();

            if (l.OuterDimensions.Length != shift.Length)
                throw new DimensionIncompability("ShiftTime: OuterDimensions does not match!");

            ShiftDimensions = shift;
        }

        public unsafe override void AfterPreCheck()
        {
            if (ShiftShape == null)
                ShiftShape = Shape.DimensionOf(ShiftDimensions.Length);

            for (int i = 0; i < ShiftShape.N; i++)
                ShiftShape.Dimensions[i] = ShiftDimensions[i].Value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public override Term CreateTerm(Index time)
        {
            time += ShiftShape;
            Term t = InputLayers[0].GetTerm(time);
            time -= ShiftShape;
            return t;
        }
    }
}
