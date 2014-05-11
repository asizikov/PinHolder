using System.Windows;
using PinHolder.DeveloperTools;
using PinHolder.Model;

namespace PinHolder.View
{
    public partial class DevPage
    {
        public DevPage()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var generator = new TestDataGenerator(ViewModelLocator.Container.Resolve<BaseCardProvider>());

            CardsStorageInfo.Text = "cards amount: " + generator.CreateTestData();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var generator = new TestDataGenerator(ViewModelLocator.Container.Resolve<BaseCardProvider>());

            CardsStorageInfo.Text = "cards amount: " + generator.DeleteAllCards();
        }

    }
}