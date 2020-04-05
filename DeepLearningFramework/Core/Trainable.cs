
using PerformanceWork.OptimizedNumerics;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeepLearningFramework.Core
{
    /// <summary>
    /// Trainable interface helps to create a trainable object that is sent to optimizers.
    /// </summary>
    public interface Trainable
    {
        /// <summary>
        /// The current statue of the Trainable.
        /// </summary>
        bool Trainable { get; }

        /// <summary>
        /// Additional Learning Rate of the Trainable.
        /// </summary>
        float LearningRateMultiplier { get; }

        /// <summary>
        /// Weights or data that is to be trained.
        /// </summary>
        Tensor Weights { get;  }

        /// <summary>
        /// Unique Id.
        /// </summary>
        int UniqueId { get; }
    }
}
