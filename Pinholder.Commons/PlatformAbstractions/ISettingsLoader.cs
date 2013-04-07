using PinHolder.Annotations;
using PinHolder.Model;

namespace PinHolder.PlatformAbstractions
{
    public interface ISettingsLoader
    {
        [NotNull]
        ApplicationSettings GetSettings();
    }
}
