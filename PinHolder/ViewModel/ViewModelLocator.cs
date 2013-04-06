using PinHolder.Annotations;
using PinHolder.Lifecycle;
using PinHolder.Model;
using PinHolder.Navigation;

namespace PinHolder.ViewModel
{
    public static class ViewModelLocator
    {
        [NotNull]
        private static TinyIoC.TinyIoCContainer Container
        {
            get
            {
                return TinyIoC.TinyIoCContainer.Current;
            }
        }

        public static MainViewModel GetMainViewModel()
        {
            return new MainViewModel(
                Container.Resolve<INavigationService>(), 
                Container.Resolve<ICardProvider>(), 
                new SettingsProvider());
        }

        public static NewCardViewModel GetNewCardViewModel()
        {
            return new NewCardViewModel(
                Container.Resolve<INavigationService>(), 
                Container.Resolve<ICardProvider>());
        }

        public static ViewCardViewModel GetViewCardViewModel(int id)
        {
            return new ViewCardViewModel(
                Container.Resolve<INavigationService>(), 
                Container.Resolve<ICardProvider>(), 
                Container.Resolve<ISecondaryTileService>(), id);
        }

        public static EditCardViewModel GetEditCardViewModel(int id)
        {
            return new EditCardViewModel(
                Container.Resolve<INavigationService>(), 
                Container.Resolve<ICardProvider>(), 
                id);
        }

        public static SettingsViewModel GetSettingsViewModel()
        {
            return new SettingsViewModel(
                new SettingsProvider(), 
                Container.Resolve<INavigationService>());
        }

        public static AboutViewModel GetAboutViewModel()
        {
            return new AboutViewModel();
        }
    }
}
