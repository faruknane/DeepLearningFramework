using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using DeepLearningFramework.Data.Operators.Terms;
using DeepLearningFramework.Core;
using PerformanceWork.OptimizedNumerics;
using Index = PerformanceWork.OptimizedNumerics.Index;

namespace DeepLearningFramework.Data.Operators.Layers
{
    public class Plus : Layer
    {
        private Term[] terms;

        public Plus(params Layer[] input)
        {
            terms = new Term[input.Length];
            
            InnerShape = new Dimension[input[0].InnerShape.Length];
            OuterShape = new Dimension[input[0].OuterShape.Length];

            foreach (var item in input)
                InputLayers.Add(item);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public override Term CreateTerm(Index time)
        {
            for (int i = 0; i < terms.Length; i++)
                terms[i] = InputLayers[i].GetTerm(time);
            return new Terms.Plus(terms);
        }
    }
}
