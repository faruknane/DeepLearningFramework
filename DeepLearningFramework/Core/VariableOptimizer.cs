﻿using DeepLearningFramework.Data;
using DeepLearningFramework.Data.Operators.Terms;
using PerformanceWork.OptimizedNumerics;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace DeepLearningFramework.Core
{
    public interface VariableOptimizer
    {
        public void UpdateWeights(Trainable x, MMDerivative derivatives);
    }
}
