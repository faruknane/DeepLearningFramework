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
        public static float[] Randomize(float[] a)
        {
            Random r = new Random();
            for (int i = 0; i < a.Length; i++)
                a[i] = (float)(r.NextDouble() * 2 - 1);
            return a;
        }
    }
 }
