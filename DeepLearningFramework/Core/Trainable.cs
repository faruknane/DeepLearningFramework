
using PerformanceWork.OptimizedNumerics;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeepLearningFramework.Core
{
    public interface Trainable
    {
        bool Trainable { get; }
        float LearningRateMultiplier { get; }
        Tensor<float> Weights { get;  }
        int UniqueId { get; }
    }
}
