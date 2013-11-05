﻿namespace PinHolder.PlatformAbstractions
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

        public void PublishMainPageLoaded()
        {
            PublishEvent("Main page loaded");

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

        public void PublishViewCardPageLoaded()
        {
            PublishEvent("View card page loaded");
        }

        public void PublishViewCardPinButtonClick()
        {
            PublishEvent("View card: pin card button clicked");
        }
    }
}
