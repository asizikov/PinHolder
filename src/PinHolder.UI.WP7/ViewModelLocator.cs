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
        internal static TinyIoC.TinyIoCContainer Container
        {
            get { return TinyIoC.TinyIoCContainer.Current; }
        }

        public static MainViewModel GetMainViewModel()
        {
            return new MainViewModel(
                Container.Resolve<INavigationService>(),
                Container.Resolve<BaseCardProvider>(),
                Container.Resolve<ICollectionFactory>(),
                Container.Resolve<StatisticsService>());
        }

        public static ReorderViewModel GetReorderViewModel()
        {
            return new ReorderViewModel(
                Container.Resolve<BaseCardProvider>(),
                Container.Resolve<ICollectionFactory>(), Container.Resolve<StatisticsService>());
        }

        public static NewCardViewModel GetNewCardViewModel()
        {
            return new NewCardViewModel(
                Container.Resolve<INavigationService>(),
                Container.Resolve<BaseCardProvider>(),
                Container.Resolve<IUiStringsProvider>(),
                Container.Resolve<StatisticsService>());
        }

        public static ViewCardViewModel GetViewCardViewModel(int id, From from)
        {
            return new ViewCardViewModel(
                Container.Resolve<INavigationService>(),
                Container.Resolve<BaseCardProvider>(),
                Container.Resolve<ISecondaryTileService>(),
                Container.Resolve<StatisticsService>(), from, id);
        }

        public static EditCardViewModel GetEditCardViewModel(int id)
        {
            return new EditCardViewModel(
                Container.Resolve<INavigationService>(),
                Container.Resolve<BaseCardProvider>(),
                Container.Resolve<ISecondaryTileService>(),
                Container.Resolve<StatisticsService>(),
                Container.Resolve<IUiStringsProvider>(),
                id);
        }

        public static AboutViewModel GetAboutViewModel()
        {
            return new AboutViewModel(Container.Resolve<IPlatformTaskFactory>(), Container.Resolve<StatisticsService>());
        }
    }
}