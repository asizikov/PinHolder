namespace PinHolder.PlatformAbstractions
{
    public abstract class StatisticsService
    {
        protected abstract void PublishEvent(string eventName);

        public abstract void Initialize();

        public void PublishAboutLoaded()
        {
            throw new System.NotImplementedException();
        }

        public void PublishAboutSupportButtonClicked()
        {
            throw new System.NotImplementedException();
        }

        public void PublishAboutRateButtonClicked()
        {
            throw new System.NotImplementedException();
        }
    }
}
