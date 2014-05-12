namespace PinHolder.PlatformAbstractions
{
    public abstract class StatisticsService
    {
        protected abstract void PublishEvent(string eventName);

        public abstract void Initialize();

        public void PublishAboutLoaded()
        {
            PublishEvent("About page loaded");
        }

        public void PublishAboutSupportButtonClicked()
        {
            PublishEvent("About page: support button clicked");
        }

        public void PublishAboutRateButtonClicked()
        {
            PublishEvent("About page: rate button clicked");
        }

        public void PublishReorderPageLoaded()
        {
            PublishEvent("Reorder page loaded");
        }

        public void PublishEditPageLoaded()
        {
            PublishEvent("Edit page loaded");
        }

        public void PublishEditCardSaved()
        {
            PublishEvent("Edit page: save card button clicked");
        }

        public void PublishEditCardDeleted()
        {
            PublishEvent("Edit page: delet card button clicked");
        }

        public void PublishMainPageLoaded(int count)
        {
            PublishEvent("Main page loaded. Cards amount: " + count);
        }

        public void PublishMainAddNewButtonClick()
        {
            PublishEvent("Main page: new card button clicked");
        }

        public void PublishMainAboutButtonClick()
        {
            PublishEvent("Main page: about button clicked");
        }

        public void PublishMainHelpButtonClick()
        {
            PublishEvent("Main page: help button clicked");
        }

        public void PublishMainReorderButtonClick()
        {
            PublishEvent("Main page: reorder button clicked");
        }

        public void PublishNewCardPageLoaded()
        {
            PublishEvent("New card page loaded");
        }

        public void PublishNewCardSaveCardButtonClick()
        {
            PublishEvent("New card page: save card button clicked");
        }

        public void PublishViewCardPageLoaded(bool isFromTile = false)
        {
            PublishEvent(string.Format("View card page loaded: {0}", (isFromTile ? "from tile" : "from main page")));
        }

        public void PublishViewCardPinButtonClick()
        {
            PublishEvent("View card: pin card button clicked");
        }

        public void PublishCloudSettingsButtonClick()
        {
            PublishEvent("Main page: cloud settings button cliked");
        }
    }
}