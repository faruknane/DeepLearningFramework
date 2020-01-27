using DeepLearningFramework.Data;
using DeepLearningFramework.Data.Operators.Terms;
using PerformanceWork.OptimizedNumerics;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace DeepLearningFramework.Core.Optimizers
{
    public interface VariableOptimizer
    {
        void UpdateWeights(Trainable x, Tensor<float> derivatives);
    }
}
