
using DeepLearningFramework.Operators.Terms;
using PerformanceWork.OptimizedNumerics;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace DeepLearningFramework.Core.Optimizers
{
    /// <summary>
    /// Stochastic Gradient Descent class is an optimizer method derived from VariableOptimizer Interface.
    /// </summary>
    public class SGD : VariableOptimizer
    {
        /// <summary>
        /// 
        /// </summary>
        public SGD()
        {

        }

        /// <summary>
        /// Updates the weights according to the derivative result.
        /// </summary>
        /// <param name="v">Trainable Variable</param>
        /// <param name="g">Gradient Tensor</param>
        /// <exception cref="DimensionIncompability">Throws if the shapes of Variable and Gradient are different!</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public unsafe void UpdateWeights(Trainable v, Tensor g)
        {
            //Console.WriteLine("Updating The Variable with ID " + v.UniqueId); trainable should have uniqueID
            if (g.Shape.EqualShape(v.Weights.Shape))
            {
                float* ptr_v = (float*)v.Weights.Array;
                float* ptr_m = (float*)g.Array;
                VectorizationFloat.ElementWiseAddAVXBetaB(ptr_v, ptr_m, ptr_v, v.Weights.Shape.TotalSize, -v.LearningRateMultiplier * Hyperparameters.LearningRate);
            }
            else
                throw new DimensionIncompability("The shapes of Variable and Gradient are different!");
        }
    }
}
