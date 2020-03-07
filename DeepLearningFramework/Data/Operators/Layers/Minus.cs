﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using DeepLearningFramework.Data.Operators.Terms;
using DeepLearningFramework.Core;
using PerformanceWork.OptimizedNumerics;
using Index = PerformanceWork.OptimizedNumerics.Index;

namespace DeepLearningFramework.Data.Operators.Layers
{
    public class Minus : Layer
    {
        public Minus(Layer l1, Layer l2)
        {
            InnerShape = new Dimension[l1.InnerShape.Length];
            OuterShape = new Dimension[l1.OuterShape.Length];

            InputLayers.Add(l1);
            InputLayers.Add(l2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public override Term CreateTerm(Index time)
        {
            return new Terms.Minus(InputLayers[0].GetTerm(time), InputLayers[1].GetTerm(time));
        }
    }
}
