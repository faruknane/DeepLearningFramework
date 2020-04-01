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
    public class Add : Layer
    {
        private Term[] terms;

        public Add(params Layer[] input)
        {
            terms = new Term[input.Length];
         
            foreach (var item in input)
                InputLayers.Add(item);

            InnerDimensionCalculation();
            OuterDimensionCalculation();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public override Term CreateTerm(Index time)
        {
            for (int i = 0; i < terms.Length; i++)
                terms[i] = InputLayers[i].GetTerm(time);
            return new Terms.Add(terms);
        }
    }
}
