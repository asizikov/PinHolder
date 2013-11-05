using System;
using PinHolder.PlatformAbstractions;

namespace PinHolder.PlatformSpecificImplementations
{
    internal class YandexStatistics : StatisticsService
    {
        protected override void PublishEvent(string eventName)
        {
            throw new NotImplementedException();
        }

        public override void Initialize()
        {
            throw new NotImplementedException();
        }
    }
}
