﻿using System;
using System.Runtime.CompilerServices;

namespace DeepLearningFramework.Operators.Layers
{
    public class Randomiz
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static unsafe float* Randomize(float* a, int Length)
        {
            float max = 0.15f;
            float min = -max;

            Random r = new Random();
            for (int i = 0; i < Length; i++)
                a[i] = (float)(r.NextDouble() * (max - min) + min);
            return a;
        }
    }
}
