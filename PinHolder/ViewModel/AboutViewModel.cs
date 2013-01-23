using System.ComponentModel;
using System.Reflection;
using Microsoft.Phone.Tasks;
using PinHolder.Annotations;
using PinHolder.Command;

namespace PinHolder.ViewModel
{
    public class AboutViewModel: BaseViewModel
    {
        private const string SUPPORT_EMAIL = "pinholder@yandex.ru";


        public AboutViewModel()
        {
            RateCommand = new RelayCommand(ShowRateTask);   
        }

        private void ShowRateTask()
        {
            var task = new MarketplaceReviewTask();
            task.Show();
        }

        [UsedImplicitly]
        public RelayCommand RateCommand { get; private set; }

        [UsedImplicitly]
        public string ApplicationVersion
        {
            get
            {
                if (DesignerProperties.IsInDesignTool)
                    return "version x.x.x";

                Assembly assembly = Assembly.GetExecutingAssembly();
                var name = new AssemblyName(assembly.FullName);
                return name.Version.ToString(3);
            }
        }

        [UsedImplicitly]
        public RelayCommand SupportQuestionCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    var emailComposeTask = new EmailComposeTask
                    {
                        To = SUPPORT_EMAIL,
                        Subject =
                           "PinHolder " +
                          ApplicationVersion
                    };
                    emailComposeTask.Show();
                });
            }
        }

    }
}
