using DeepLearningFramework.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeepLearningFramework
{
    public interface VariableOptimizer
    {
        public void UpdateWeights(Variable x, MMDerivative derivatives)
        {

        }
    }
}
