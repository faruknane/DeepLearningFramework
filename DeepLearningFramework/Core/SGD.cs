using DeepLearningFramework.Data;
using PerformanceWork.OptimizedNumerics;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeepLearningFramework.Core
{
    public class SGD : VariableOptimizer
    {
        public SGD()
        {
            
        }

        public unsafe void UpdateWeights(Variable v, MMDerivative m)
        {
            if (m.D3 != v.D1 || m.D4 != v.D2)
                throw new Exception("Dimensions!");

            int neg = (m.Negative ? -1:1);
            m.MultiplyBy(neg * v.LearningRateMultiplier * Hyperparameters.LearningRate);

            float* ptr_v = v.Weights.GetPointer();

            fixed(float* ptr_m = m.Derivatives)
            for (int i1 = 0; i1 < m.D1; i1++)
                for (int i2 = 0; i2 < m.D2; i2++)
                {
                    int loc_m = i1 * m.D2 * m.D3 * m.D4 + i2 * m.D3 * m.D4;
                    Vectorization.ElementWiseSubtractAVX(ptr_v, ptr_m + loc_m, ptr_v, v.D1 * v.D2);
                }
            m.DivideBy(neg * v.LearningRateMultiplier * Hyperparameters.LearningRate);
        }
    }
}
