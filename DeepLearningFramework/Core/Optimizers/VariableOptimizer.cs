using DeepLearningFramework.Operators.Terms;
using PerformanceWork.OptimizedNumerics;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace DeepLearningFramework.Core.Optimizers
{
    /// <summary>
    /// Optimizer Interface.
    /// </summary>
    public interface VariableOptimizer
    {
        /// <summary>
        /// Weight updating method.
        /// </summary>
        /// <param name="v">Trainable Variable</param>
        /// <param name="g">Gradient Tensor</param>
        /// <exception cref="DimensionIncompability">Throws if the shapes of Variable and Gradient are different!</exception>

        void UpdateWeights(Trainable x, Tensor<float> derivatives);

        //We can add refresh method to clear the instance if necessary
    }
}
