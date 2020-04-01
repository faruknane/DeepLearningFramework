using System;
using System.Collections.Generic;
using System.Text;

namespace DeepLearningFramework.Core
{
    /// <summary>
    /// A global helper class that contains many helper classes inside.
    /// </summary>
    public class Helper
    {
        /// <summary>
        /// Helps assigning unique identitiy.
        /// </summary>
        public static class Id
        {
            public static int UniqueIdAssigner { get; set; } = 0;
            /// <summary>
            /// Creates a new identity.
            /// </summary>
            /// <returns>New Identitiy</returns>
            public static int GetNewId()
            {
                return UniqueIdAssigner++;
            }
        }
    }
}
