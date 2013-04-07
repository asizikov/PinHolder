using System;
using PinHolder.Navigation;

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
            
            int id;
            if (Int32.TryParse(parameter, out id))
            {
                DataContext = ViewModelLocator.GetViewCardViewModel(id);
            }
        }
    }
}