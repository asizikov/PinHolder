using System;
using Microsoft.Phone.Controls;

namespace PinHolder.Navigation
{
    public sealed class NavigationService : INavigationService
    {
        private readonly PhoneApplicationFrame _rootFrame;

        public NavigationService(PhoneApplicationFrame rootFrame)
        {
            _rootFrame = rootFrame;
        }

        public void Navigate(string pageName, string parameterQueue = null)
        {
            var uri = string.IsNullOrEmpty(parameterQueue)
                          ? pageName
                          : pageName + parameterQueue;
            _rootFrame.Navigate(new Uri(uri,UriKind.Relative));
        }

        public void GoBack()
        {
            if (_rootFrame.CanGoBack)
            {
                _rootFrame.GoBack();                
            }
        }
    }
}
