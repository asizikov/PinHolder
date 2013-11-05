using System;
using PinHolder.Lifecycle;
using PinHolder.PlatformAbstractions;
using Yandex.Metrica;

namespace PinHolder.PlatformSpecificImplementations
{
    internal class YandexStatistics : StatisticsService
    {
        protected override void PublishEvent(string eventName)
        {
            Counter.ReportEvent(eventName);
        }

        public override void Initialize()
        {
            Counter.CustomAppVersion = new Version(Configuration.MajorVersion, Configuration.MinorVersion);
            Counter.TrackLocationEnabled = true;
            Counter.Start(Configuration.StatisticsKey);
        }
    }
}
