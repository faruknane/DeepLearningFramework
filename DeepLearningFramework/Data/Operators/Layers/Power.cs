using DeepLearningFramework.Data.Operators.Terms;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using DeepLearningFramework.Core;
using Index = PerformanceWork.OptimizedNumerics.Index;

namespace DeepLearningFramework.Data.Operators.Layers
{
    public class Power : Layer
    {
        public int PowerOf { get; private set; }

        public Power(Layer l, int pow)
        {
            this.InputLayers.Add(l);
            this.PowerOf = pow;

            this.OuterShape = new Dimension[l.OuterShape.Length];
            this.InnerShape = new Dimension[l.InnerShape.Length];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public override Term CreateTerm(Index time)
        {
            return new Terms.Power(InputLayers[0].GetTerm(time), PowerOf);
        }



    }
}
