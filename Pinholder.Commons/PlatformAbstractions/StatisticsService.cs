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

        public void PublishReorderPageLoaded()
        {
            

        }

        public void PublishEditPageLoaded()
        {
            throw new System.NotImplementedException();
        }

        public void PublishEditCardSaved()
        {
            

        }

        public void PublishEditCardDeleted()
        {
            

        }

        public void PublishMainPageLoaded()
        {
            

        }

        public void PublishMainAddNewButtonClick()
        {
            throw new System.NotImplementedException();
        }

        public void PublishMainAboutButtonClick()
        {
            throw new System.NotImplementedException();
        }

        public void PublishMainHelpButtonClick()
        {
            throw new System.NotImplementedException();
        }

        public void PublishMainReorderButtonClick()
        {
            throw new System.NotImplementedException();
        }

        public void PublishNewCardPageLoaded()
        {
            throw new System.NotImplementedException();
        }

        public void PublishNewCardSaveCardButtonClick()
        {
            throw new System.NotImplementedException();
        }

        public void PublishViewCardPageLoaded()
        {
            throw new System.NotImplementedException();
        }

        public void PublishViewCardPinButtonClick()
        {
            throw new System.NotImplementedException();
        }
    }
}
