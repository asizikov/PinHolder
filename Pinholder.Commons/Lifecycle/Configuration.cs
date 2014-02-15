using System;

namespace PinHolder.Lifecycle
{
    public static class Configuration
    {
        public static bool TrackStatistics
        {
            get { return true; }
        }
        public static Version Version
        {
            get { return new Version(1, 1, 8); }
        }

        public static uint StatisticsKey
        {
            get { return 0; }
        }
    }
}