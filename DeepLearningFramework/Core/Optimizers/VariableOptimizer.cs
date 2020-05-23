using PerformanceWork.OptimizedNumerics;

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

        void UpdateWeights(Trainable x, Tensor derivatives);

        //We can add refresh method to clear the instance if necessary
    }
}
