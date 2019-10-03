using DeepLearningFramework.Data;
using DeepLearningFramework.Data.Operators.Terms;
using PerformanceWork.OptimizedNumerics;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace DeepLearningFramework.Core
{

    public class SGD : VariableOptimizer
    {
        public SGD()
        {
            
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public unsafe void UpdateWeights(Trainable v, MMDerivative m)
        {
            //Console.WriteLine("Updating The Variable with ID " + v.UniqueId); trainable should have uniqueID

            if (m.D3 != v.Weights.D1 || m.D4 != v.Weights.D2)
                throw new Exception("Dimensions!");

            int neg = (m.Negative ? -1 : 1);
            float* ptr_v = v.Weights.GetPointer();
            float* ptr_m = m.Derivatives;

            for (int i1 = 0; i1 < m.D1; i1++)
                for (int i2 = 0; i2 < m.D2; i2++)
                {
                    int loc_m = i1 * m.D2 * m.D3 * m.D4 + i2 * m.D3 * m.D4;
                    Vectorization.ElementWiseSubtractAVXBetaB(ptr_v, ptr_m + loc_m, ptr_v, v.Weights.D1 * v.Weights.D2, neg * v.LearningRateMultiplier * Hyperparameters.LearningRate);
                }
        }
    }
}
