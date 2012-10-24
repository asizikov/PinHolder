using System;
using System.Globalization;
using System.Windows.Controls;
using PinHolder.Navigation;
using PinHolder.ViewModel;

namespace PinHolder.View
{
    public partial class NewCardPage
    {
        public NewCardPage()
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
                    DataContext = ViewModelLocator.GetEditCardViewModel(id);
                    return;
                }
            }
            DataContext =  ViewModelLocator.GetNewCardViewModel();
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var tb = sender as TextBox;
            if (tb == null) return;
            
            if (tb.Text.Length > 0)
            {
                tb.Text = tb.Text[0].ToString(CultureInfo.InvariantCulture);
                Focus();
            }
        }
    }
}