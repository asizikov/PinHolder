using System;

namespace PinHolder.Lifecycle
{
    public static class Configuration
    {
        public static Version Version
        {
            get { return new Version(1, 1, 6); }
        }

        public static uint StatisticsKey
        {
            get { return 0; }
        }
    }
}