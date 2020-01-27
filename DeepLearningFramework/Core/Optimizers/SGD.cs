using DeepLearningFramework.Data;
using DeepLearningFramework.Data.Operators.Terms;
using PerformanceWork.OptimizedNumerics;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace DeepLearningFramework.Core.Optimizers
{

    public class SGD : VariableOptimizer
    {
        public SGD()
        {

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public unsafe void UpdateWeights(Trainable v, Tensor<float> m)
        {
            //Console.WriteLine("Updating The Variable with ID " + v.UniqueId); trainable should have uniqueID

            bool suit = true;
            
            if (m.Shape.N > v.Weights.Shape.N)
                for (int i = 0; i < v.Weights.Shape.N; i++)
                    suit &= v.Weights.Shape[i] == m.Shape[m.Shape.N - (v.Weights.Shape.N - i)];
            else
                suit = false;

            if (!suit)
                throw new Exception("Dimensions!");

            float* ptr_v = (float*)v.Weights.Array;
            float* ptr_m = (float*)m.Array;

            int travel = m.Shape.TotalSize / v.Weights.Shape.TotalSize;
            for (int i = 0; i < travel; i++)
            {
                int loc_m = i * v.Weights.Shape.TotalSize;
                Vectorization.ElementWiseAddAVXBetaB(ptr_v, ptr_m + loc_m, ptr_v, v.Weights.Shape.TotalSize, -v.LearningRateMultiplier * Hyperparameters.LearningRate);
            }
        }
    }
}
