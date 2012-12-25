using PinHolder.Annotations;
using PinHolder.Model;

namespace PinHolder.ViewModel
{
    public sealed class SettingsViewModel :BaseViewModel
    {
        [NotNull] private readonly SettingsProvider _settingsProvider;
        [NotNull] private readonly ApplicationSettings _settings;

        public SettingsViewModel(SettingsProvider settingsProvider )
        {
            _settingsProvider = settingsProvider;
            _settings = _settingsProvider.LoadSettings();
        }


        private void Save()
        {
            _settingsProvider.SaveSettings(_settings);
        }

    }
}
