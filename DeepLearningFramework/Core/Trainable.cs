using DeepLearningFramework.Data;
using PerformanceWork.OptimizedNumerics;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeepLearningFramework.Core
{
    public interface Trainable
    {
        public bool Trainable { get; }
        public float LearningRateMultiplier { get; }
        public Matrix Weights { get;  }
        public int UniqueId { get; }
        
    }
}
