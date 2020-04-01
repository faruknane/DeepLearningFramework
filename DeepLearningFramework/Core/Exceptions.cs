using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepLearningFramework.Core
{
    /// <summary>
    /// DimensionIncompability throws when there is an incompability issue with dimensions or shapes.
    /// </summary>
    public class DimensionIncompability : Exception
    {
        public DimensionIncompability(string message) : base(message)
        {
        }

        public DimensionIncompability(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
