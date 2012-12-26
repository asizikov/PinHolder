using System;

namespace PinHolder.Model
{
    public class SettingsProvider
    {
        public ApplicationSettings LoadSettings()
        {
            return new ApplicationSettings
                {
                    MasterPassword = string.Empty,
                    UseMasterPassword = false
                };
        }

        public void SaveSettings(ApplicationSettings settings)
        {
            throw new NotImplementedException();
        }
    }
}
