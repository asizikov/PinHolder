using System;
using Curacao.Mvvm.Abstractions.Services;
using Curacao.Mvvm.ViewModel;
using PinHolder.Annotations;
using PinHolder.PlatformAbstractions;

namespace PinHolder.ViewModel
{
    public class BackupViewModel : BaseViewModel
    {
        private readonly StatisticsService _statistics;

        public BackupViewModel([NotNull] ISystemDispatcher dispatcher, [NotNull] StatisticsService statistics) 
            : base(dispatcher)
        {
            if (statistics == null) throw new ArgumentNullException("statistics");
            _statistics = statistics;
        }
    }
}
