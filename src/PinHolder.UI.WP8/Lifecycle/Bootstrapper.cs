using Curacao.Mvvm.Abstractions.Services;
using Microsoft.Phone.Controls;
using PinHolder.Model;
using PinHolder.Navigation;
using PinHolder.PlatformAbstractions;
using PinHolder.PlatformSpecificFactories;
using PinHolder.PlatformSpecificImplementations;
using PinHolder.Resources;
using TinyIoC;

namespace PinHolder.Lifecycle
{
    internal static class Bootstrapper
    {
        public static void InitApplication(PhoneApplicationFrame rootFrame)
        {
            RegisterServices(rootFrame);
            ThemeManager.ToDarkTheme();
            TinyIoCContainer.Current.Resolve<StatisticsService>().Initialize();
        }

        private static void RegisterServices(PhoneApplicationFrame rootFrame)
        {
            var dispatcher = new SystemDispatcher();
            dispatcher.Initialize(rootFrame.Dispatcher);

            var ioc = TinyIoCContainer.Current;
            ioc.Register<ISystemDispatcher>(dispatcher);
            ioc.Register<INavigationService>((container, overloads) => new NavigationService(rootFrame));
            ioc.Register<ISecondaryTileService>((container, overloads) => new SecondaryTileService());
            ioc.Register<BaseCardProvider>((container, overloads) => new CardProvider());
            ioc.Register<ICollectionFactory>((container, overloads) => new CollectionFactory());
            ioc.Register<IUiStringsProvider>((container, overloads) => new UiStringsProvider());
            ioc.Register<ISettingsLoader>((container, overloads) => new SettingsLoader());
            ioc.Register((container, overloads) => new ApplicationSettingsProvider(container.Resolve<ISettingsLoader>()));
            ioc.Register<IPlatformTaskFactory>((container, overloads) => new PhoneTaskFactory());
            ioc.Register<StatisticsService>((container, overloads) => new YandexStatistics());
            
        }
    }
}