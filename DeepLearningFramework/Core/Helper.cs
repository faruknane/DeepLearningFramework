using System;
using System.Collections.Generic;
using System.Text;

namespace DeepLearningFramework.Core
{
    public class Helper
    {
        public static class Id
        {
            public static int UniqueIdAssigner { get; set; } = 0;
            public static int GetNewId()
            {
                return UniqueIdAssigner++;
            }
        }
    }
}
