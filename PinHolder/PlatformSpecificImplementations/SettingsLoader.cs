using System;
using PinHolder.Model;
using PinHolder.PlatformAbstractions;

namespace PinHolder.PlatformSpecificImplementations
{
    public class SettingsLoader: ISettingsLoader
    {
        public ApplicationSettings GetSettings()
        {
            return new ApplicationSettings {Password = "12345", AskPassword = true};
        }
    }
}
