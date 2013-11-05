using System;
using System.Reflection;
using PinHolder.Annotations;
using PinHolder.Command;
using PinHolder.Lifecycle;
using PinHolder.PlatformAbstractions;

namespace PinHolder.ViewModel
{
    public class AboutViewModel : BaseViewModel
    {
        private readonly IPlatformTaskFactory _platformTaskFactory;
        private readonly StatisticsService _statistics;
        private const string SUPPORT_EMAIL = "pinholder@yandex.ru";


        public AboutViewModel([NotNull] IPlatformTaskFactory platformTaskFactory, [NotNull] StatisticsService statistics)
        {
            if (platformTaskFactory == null) throw new ArgumentNullException("platformTaskFactory");
            if (statistics == null) throw new ArgumentNullException("statistics");
            _platformTaskFactory = platformTaskFactory;
            _statistics = statistics;

            RateCommand = new RelayCommand(ShowRateTask);
            _statistics.PublishAboutLoaded();
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
            get { return string.Format("{0}.{1}", Configuration.MajorVersion, Configuration.MinorVersion); }
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