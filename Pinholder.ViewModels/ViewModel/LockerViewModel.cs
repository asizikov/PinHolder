using System;
using PinHolder.Annotations;
using PinHolder.Model;

namespace PinHolder.ViewModel
{
    public class LockerViewModel: BaseViewModel
    {
        private readonly ApplicationSettingsProvider _settingsProvider;
        private bool _showLocker;
        private bool _passwordAccepted;

        private Action _onPasswordAccepted;

        public LockerViewModel([NotNull] ApplicationSettingsProvider settingsProvider)
        {
            if (settingsProvider == null) throw new ArgumentNullException("settingsProvider");
            _settingsProvider = settingsProvider;
        }

        public bool ShowLocker
        {
            get { return _showLocker; }
            private set
            {
                if (value.Equals(_showLocker)) return;
                _showLocker = value;
                OnPropertyChanged("ShowLocker");
            }
        }

        public void Activate([CanBeNull] Action onPasswordAccepted)
        {
            _onPasswordAccepted = onPasswordAccepted;
            if (_settingsProvider.AskPassword)
            {
                ShowLocker = true;
            }
            
        }

        [UsedImplicitly(ImplicitUseKindFlags.Default)]
        public bool PasswordAccepted
        {
            get
            {
                return _passwordAccepted;
            }
            set
            {
                _passwordAccepted = value;
                if (_passwordAccepted && _onPasswordAccepted != null)
                {
                    _onPasswordAccepted();
                }

                ShowLocker = !_passwordAccepted;
            }
        }

        public string StoredPassword { get { return _settingsProvider.Password; } }
    }
}
