using System.Windows.Navigation;

namespace PinHolder.View
{
    public partial class CloudSettingsPage
    {
        public CloudSettingsPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            DataContext = ViewModelLocator.GetBackupPage();
        }
    }
}