using System.Windows.Navigation;
using PinHolder.ViewModel;

namespace PinHolder.View
{
    public partial class ReoderPage
    {
        private ReorderViewModel _viewModel;

        public ReoderPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            _viewModel = ViewModelLocator.GetReorderViewModel();
            DataContext = _viewModel;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            _viewModel.ApplyChangesCommand.Execute(null);
        }
    }
}