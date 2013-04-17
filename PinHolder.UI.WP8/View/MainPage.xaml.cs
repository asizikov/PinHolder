using Microsoft.Phone.Controls;
using PinHolder.ViewModel;

namespace PinHolder.View
{
    public sealed partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            DataContext = ViewModelLocator.GetMainViewModel();
        }
    }
}