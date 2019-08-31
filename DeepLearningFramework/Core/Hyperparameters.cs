﻿using System;
using System.Collections.Generic;
using System.Text;

namespace DeepLearningFramework.Core
{
    public static class Hyperparameters
    {
        public static float LearningRate = 0.2f;
        public static VariableOptimizer Optimizer = new SGD();

    }
}
