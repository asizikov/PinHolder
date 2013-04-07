using System;
using System.Reflection;
using PinHolder.Annotations;
using PinHolder.Command;
using PinHolder.PlatformAbstractions;

namespace PinHolder.ViewModel
{
    public class AboutViewModel : BaseViewModel
    {
        private readonly IPlatformTaskFactory _platformTaskFactory;
        private const string SUPPORT_EMAIL = "pinholder@yandex.ru";


        public AboutViewModel([NotNull] IPlatformTaskFactory platformTaskFactory)
        {
            if (platformTaskFactory == null) throw new ArgumentNullException("platformTaskFactory");
            _platformTaskFactory = platformTaskFactory;

            RateCommand = new RelayCommand(ShowRateTask);
        }

        private void ShowRateTask()
        {
            var task = _platformTaskFactory.GetRateTask();
            task.Show();
        }

        [UsedImplicitly]
        public RelayCommand RateCommand { get; private set; }

        [UsedImplicitly]
        public string ApplicationVersion
        {
            get
            {
                var assembly = Assembly.GetExecutingAssembly();
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
                        var emailComposeTask = _platformTaskFactory.GetEmailTask(SUPPORT_EMAIL, "PinHolder " +
                  ApplicationVersion);
                        emailComposeTask.Show();
                    });
            }
        }

    }
}
