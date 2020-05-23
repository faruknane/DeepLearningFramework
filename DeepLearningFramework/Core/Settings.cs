using PerformanceWork;

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
