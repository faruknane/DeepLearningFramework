using System;
using System.Runtime.CompilerServices;
using DeepLearningFramework.Data;
using DeepLearningFramework.Data.Operators.Terms;
using PerformanceWork.OptimizedNumerics;

namespace DeepLearningFramework.Data.Operators.Layers
{
    public class Randomiz
    {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe float* Randomize(float* a, int Length)
        {
            Random r = new Random();
            for (int i = 0; i < Length; i++)
                a[i] = (float)(r.NextDouble() * 2 - 1f);
            return a;
        }
    }
 }
