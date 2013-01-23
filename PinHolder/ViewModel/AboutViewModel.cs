using Microsoft.Phone.Tasks;
using PinHolder.Command;

namespace PinHolder.ViewModel
{
    public class AboutViewModel: BaseViewModel
    {

        public AboutViewModel()
        {
            RateCommand = new RelayCommand(ShowRateTask);   
        }

        private void ShowRateTask()
        {
            var task = new MarketplaceReviewTask();
            task.Show();
        }

        public RelayCommand RateCommand { get; private set; }
    }
}
