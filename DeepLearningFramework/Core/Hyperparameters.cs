using System;
using System.Collections.Generic;
using System.Text;
using DeepLearningFramework.Core.Optimizers;

namespace DeepLearningFramework.Core
{
    /// <summary>
    /// Hyperparameters that are to be used in training session.
    /// </summary>
    public static class Hyperparameters
    {
        /// <summary>
        /// Learning Rate.
        /// </summary>
        public static float LearningRate = 0.2f;
        /// <summary>
        /// Optimizer Method.
        /// </summary>
        public static VariableOptimizer Optimizer = new SGD();
    }
}
