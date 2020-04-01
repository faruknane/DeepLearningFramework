using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using DeepLearningFramework.Operators.Terms;
using DeepLearningFramework.Core;
using PerformanceWork.OptimizedNumerics;
using Index = PerformanceWork.OptimizedNumerics.Index;

namespace DeepLearningFramework.Operators.Layers
{
    public class MatrixMultiply : Layer
    {
        public MatrixMultiply(Layer x1, Layer x2)
        {
            this.InputLayers.Add(x1);
            this.InputLayers.Add(x2);

            if (x1.InnerDimensions.Length != 2 || x2.InnerDimensions.Length != 2)
                throw new Exception("Inner Dimension should be 2!");

            InnerDimensionCalculation();
            OuterDimensionCalculation();
        }

        public override void InnerDimensionCalculation()
        {
            if (InputLayers.Count > 0)
            {
                this.InnerDimensions = new Dimension[2];

                this.InnerDimensions[0] = InputLayers[0].InnerDimensions[0];
                this.InnerDimensions[1] = InputLayers[1].InnerDimensions[1];
            }
        }

        public override void InnerDimensionCheck()
        {
            if (InputLayers.Count != 2)
                throw new Exception("The number of layers should be 2");

            var item = InputLayers[0];
            var item2 = InputLayers[1];

            if (item.InnerDimensions[0].Value <= 0 || item.InnerDimensions[1].Value <= 0 || item2.InnerDimensions[0].Value <= 0 || item2.InnerDimensions[1].Value <= 0
                || item.InnerDimensions[1].Value != item2.InnerDimensions[0].Value)
                throw new Exception("Inner dimension incompatibility!");
        }
       

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public override Term CreateTerm(Index time)
        {
            return new Terms.MatrixMultiply(InputLayers[0].GetTerm(time), InputLayers[1].GetTerm(time));
        }

    }
}
