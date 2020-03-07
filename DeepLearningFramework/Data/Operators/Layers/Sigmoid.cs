using DeepLearningFramework.Core;
using DeepLearningFramework.Data.Operators.Terms;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace DeepLearningFramework.Data.Operators.Layers
{
    public class Sigmoid : Layer
    {
        public Sigmoid(Layer x)
        {
            this.InputLayers.Add(x);
            this.OuterShape = new Dimension[x.OuterShape.Length];
            this.InnerShape = new Dimension[x.InnerShape.Length];
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public override Term CreateTerm(PerformanceWork.OptimizedNumerics.Index time)
        {
            return new Terms.Sigmoid(InputLayers[0].GetTerm(time));
        }
    }
}
