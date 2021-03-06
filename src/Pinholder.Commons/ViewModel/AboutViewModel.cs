﻿using System;
using Curacao.Mvvm.Commands;
using PinHolder.Annotations;
using PinHolder.Lifecycle;
using PinHolder.PlatformAbstractions;

namespace PinHolder.ViewModel
{
    public class AboutViewModel : UnsafeBaseViewModel
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

            RateCommand = new RelayCommand(_ => ShowRateTask());
            _statistics.PublishAboutLoaded();
        }

        private void ShowRateTask()
        {
            var task = _platformTaskFactory.GetRateTask();
            _statistics.PublishAboutRateButtonClicked();
            task.Show();
        }

        [UsedImplicitly]
        public RelayCommand RateCommand { get; private set; }

        [UsedImplicitly]
        public string ApplicationVersion
        {
            get { return Configuration.Version.ToString(); }
        }

        [UsedImplicitly]
        public RelayCommand SupportQuestionCommand
        {
            get
            {
                return new RelayCommand(_ =>
                {
                    var emailComposeTask = _platformTaskFactory.GetEmailTask(SUPPORT_EMAIL, "PinHolder " +
                                                                                            ApplicationVersion);
                    _statistics.PublishAboutSupportButtonClicked();
                    emailComposeTask.Show();
                });
            }
        }
    }
}