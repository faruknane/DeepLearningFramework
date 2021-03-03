using PerformanceWork;

namespace DeepLearningFramework.Core
{
    public static class Settings
    {
        static DeviceConfig device;

        public static DeviceConfig Device
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
