using System.IO.IsolatedStorage;
using PinHolder.Annotations;
using PinHolder.Model;

namespace PinHolder.Lifecycle
{
    public class SettingsProvider
    {
        private ApplicationSettings _cachedSettrings;
        [NotNull] private readonly IsolatedStorageSettings _storageSettings;

        public SettingsProvider()
        {
            _storageSettings =  IsolatedStorageSettings.ApplicationSettings;
        }


        public ApplicationSettings LoadSettings()
        {
            bool usePassword = false;
            _storageSettings.TryGetValue(SettingsKeys.UsePassword, out usePassword);
            string password = string.Empty;

            _storageSettings.TryGetValue(SettingsKeys.Password, out password);

            _cachedSettrings = new ApplicationSettings
                {
                    MasterPassword = password,
                    UseMasterPassword = usePassword
                };

            return _cachedSettrings;
        }

        public void SaveSettings(ApplicationSettings settings)
        {
            if (_cachedSettrings.MasterPassword != settings.MasterPassword ||
                _cachedSettrings.UseMasterPassword != settings.UseMasterPassword)
            {
                Save(settings);
            }
        }

        private void Save(ApplicationSettings settings)
        {
            SaveKeyValue(SettingsKeys.UsePassword, settings.UseMasterPassword);
            SaveKeyValue(SettingsKeys.Password, settings.MasterPassword);
        }

        private void SaveKeyValue(string key, object value)
        {
            if (!_storageSettings.Contains(key))
            {
                _storageSettings.Add(key, value);
            }
            else
            {
                _storageSettings[key] = value;
            }
        }
    }
}
