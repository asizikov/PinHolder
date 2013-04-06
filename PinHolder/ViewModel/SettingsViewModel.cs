using System;
using PinHolder.Annotations;
using PinHolder.Command;
using PinHolder.Lifecycle;
using PinHolder.Model;
using PinHolder.Navigation;

namespace PinHolder.ViewModel
{
    public sealed class SettingsViewModel : BaseViewModel
    {
        [NotNull]
        private readonly SettingsProvider _settingsProvider;

        [NotNull]
        private readonly INavigationService _navigationService;
        private readonly ApplicationSettings _settings;

        public SettingsViewModel(SettingsProvider settingsProvider, [NotNull] INavigationService navigationService)
        {
            if (navigationService == null) throw new ArgumentNullException("navigationService");
            _settingsProvider = settingsProvider;
            _navigationService = navigationService;
            _settings = _settingsProvider.LoadSettings();
            SaveSettingsCommand = new RelayCommand(Save, CanSave);
            UseMasterPassword = _settings.UseMasterPassword;
        }

        private bool _useMasterPassword;
        private string _password;

        [UsedImplicitly]
        public bool UseMasterPassword
        {
            get
            {
                return _useMasterPassword;
            }
            set
            {
                if (value.Equals(_useMasterPassword)) return;
                _useMasterPassword = value;
                OnPropertyChanged("UseMasterPassword");
                SaveSettingsCommand.RaiseCanExecuteChanged();
            }
        }

        [UsedImplicitly]
        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                OnPropertyChanged("Password");
                SaveSettingsCommand.RaiseCanExecuteChanged();
            }
        }

        [NotNull, UsedImplicitly]
        public RelayCommand SaveSettingsCommand { get; set; }

        private void Save()
        {
            _settingsProvider.SaveSettings(CurrentSettings);
            _navigationService.GoBack();
        }

        private ApplicationSettings CurrentSettings
        {
            get
            {
                return new ApplicationSettings
                {
                    UseMasterPassword = UseMasterPassword,
                    MasterPassword = Password
                };
            }
        }

        private bool CanSave()
        {
            return CurrentSettings != _settings;
        }


    }
}
