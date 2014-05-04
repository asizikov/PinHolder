using System;
using PinHolder.Annotations;
using PinHolder.PlatformAbstractions;

namespace PinHolder.Model
{
    public class ApplicationSettingsProvider
    {
        private readonly ISettingsLoader _settingsLoader;
        private readonly ApplicationSettings _settings;

        public ApplicationSettingsProvider([NotNull] ISettingsLoader settingsLoader)
        {
            if (settingsLoader == null) throw new ArgumentNullException("settingsLoader");
            _settingsLoader = settingsLoader;
            _settings =  _settingsLoader.GetSettings();

        }


        public string Password
        {
            get { return _settings.Password; }
        }

        public bool AskPassword
        {
            get { return _settings.AskPassword; }
        }
    }
}
