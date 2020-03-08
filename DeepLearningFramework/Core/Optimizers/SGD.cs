
using DeepLearningFramework.Operators.Terms;
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
            if (m.Shape.EqualShape(v.Weights.Shape))
            {
                float* ptr_v = (float*)v.Weights.Array;
                float* ptr_m = (float*)m.Array;
                Vectorization.ElementWiseAddAVXBetaB(ptr_v, ptr_m, ptr_v, v.Weights.Shape.TotalSize, -v.LearningRateMultiplier * Hyperparameters.LearningRate);
            }
            else
                throw new Exception("Different Dimensions!");
        }
    }
}
