using System;
using Microsoft.Phone.Controls;
using PinHolder.Navigation;
using PinHolder.ViewModel;

namespace PinHolder.View
{
    public partial class ViewPage : PhoneApplicationPage
    {
        public ViewPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            string parameter;
            if (NavigationContext.QueryString.TryGetValue(Keys.Id, out parameter))
            {
                int id;
                if (Int32.TryParse(parameter, out id))
                {
                   DataContext = ViewModelLocator.GetViewCardViewModel(id);
                }
            }
        }
    }
}