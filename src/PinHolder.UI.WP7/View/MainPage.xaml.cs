using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PinHolder.View
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            InitializeComponent();
        }


        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            DataContext = ViewModelLocator.GetMainViewModel();
//            LayoutRoot.Background = new ImageBrush
//            {
//                ImageSource = new BitmapImage(
//                    new Uri("../Resources/Images/bg.jpg", UriKind.RelativeOrAbsolute))
//            };
        }
    }
}