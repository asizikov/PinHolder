namespace PinHolder.View
{
    public partial class About
    {
        public About()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            DataContext = ViewModelLocator.GetAboutViewModel();
        }
    }
}