using PinHolder.Annotations;
using PinHolder.Lifecycle;
using PinHolder.Model;
using PinHolder.Navigation;
using PinHolder.PlatformAbstractions;
using PinHolder.ViewModel;

namespace PinHolder
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
                Container.Resolve<BaseCardProvider>(),
                Container.Resolve<ICollectionFactory>());
        }

        public static NewCardViewModel GetNewCardViewModel()
        {
            return new NewCardViewModel(
                Container.Resolve<INavigationService>(),
                Container.Resolve<BaseCardProvider>(),
                Container.Resolve<IUiStringsProvider>());
        }

        public static ViewCardViewModel GetViewCardViewModel(int id)
        {
            return new ViewCardViewModel(
                Container.Resolve<INavigationService>(),
                Container.Resolve<BaseCardProvider>(), 
                Container.Resolve<ISecondaryTileService>(),
                new LockerViewModel(Container.Resolve<ApplicationSettingsProvider>()), id);
        }

        public static EditCardViewModel GetEditCardViewModel(int id)
        {
            return new EditCardViewModel(
                Container.Resolve<INavigationService>(),
                Container.Resolve<BaseCardProvider>(),
                Container.Resolve<ISecondaryTileService>(),
                Container.Resolve<IUiStringsProvider>(),
                id);
        }

        public static AboutViewModel GetAboutViewModel()
        {
            return new AboutViewModel(Container.Resolve<IPlatformTaskFactory>());
        }
    }
}
