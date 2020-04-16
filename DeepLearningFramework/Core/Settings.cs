using PerformanceWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepLearningFramework.Core
{
    public static class Settings
    {
        static DeviceIndicator device;

        public static DeviceIndicator Device 
        {
            get 
            {
                return device;
            }
            set
            {
                device = value;
            }
        }

    }
}
