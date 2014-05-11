using System;
using BindableApplicationBar;
using Curacao.Mvvm.Commands;

namespace PinHolder.View
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            InitializeComponent();

#if DEBUG
            ApplicationBar.MenuItems.Add(new BindableApplicationBarMenuItem
            {
                Text = "developer tools",
                Command = new RelayCommand(_=> NavigationService.Navigate(new Uri("/View/DevPage.xaml", UriKind.RelativeOrAbsolute)))
            }); 
#endif
        }


        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            DataContext = ViewModelLocator.GetMainViewModel();
        }
    }
}