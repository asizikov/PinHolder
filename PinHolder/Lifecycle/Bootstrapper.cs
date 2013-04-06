﻿using Microsoft.Phone.Controls;
using PinHolder.Model;
using PinHolder.Navigation;
using TinyIoC;

namespace PinHolder.Lifecycle
{
    public static class Bootstrapper
    {
        public static void InitApplication(PhoneApplicationFrame rootFrame)
        {
            RegisterServices(rootFrame);
        }

        private static void RegisterServices(PhoneApplicationFrame rootFrame)
        {
            var ioc = TinyIoCContainer.Current;
            ioc.Register<INavigationService>((container, overloads) => new NavigationService(rootFrame));
            ioc.Register<ISecondaryTileService>((container, overloads) => new SecondaryTileService());
            ioc.Register<BaseCardProvider>((container, overloads) => new CardProvider());
        }
    }
}