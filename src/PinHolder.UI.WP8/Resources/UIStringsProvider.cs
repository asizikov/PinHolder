using PinHolder.PlatformAbstractions;

namespace PinHolder.Resources
{
    public class UiStringsProvider : IUiStringsProvider
    {
        public string Edit
        {
            get
            {
                return Strings.Edit;
            }
        }

        public string New
        {
            get
            {
                return Strings.New;
            }
        }
    }
}
