using PinHolder.Model;

namespace PinHolder.Lifecycle
{
    public interface ISettingsProvider
    {
        ApplicationSettings LoadSettings();
        void SaveSettings(ApplicationSettings settings);
    }
}