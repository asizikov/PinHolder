using System;
using PinHolder.Navigation;
using PinHolder.ViewModel;

namespace PinHolder.View
{
    public sealed partial class ViewPage
    {
        public ViewPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            string parameter;
            if (!NavigationContext.QueryString.TryGetValue(Keys.Id, out parameter)) return;
            var from = NavigationContext.QueryString.ContainsKey(Keys.From) ? From.MainPage : From.Tile;
            int id;
            if (Int32.TryParse(parameter, out id))
            {
                DataContext = ViewModelLocator.GetViewCardViewModel(id, from);
            }
        }
    }
}