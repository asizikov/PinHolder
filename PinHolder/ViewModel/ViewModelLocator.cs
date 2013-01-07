using PinHolder.Annotations;
using PinHolder.Lifecycle;
using PinHolder.Model;
using PinHolder.Navigation;

namespace PinHolder.ViewModel
{
    public static class ViewModelLocator
    {
        [NotNull]
        public static NavigationService Navigation { get; set; }

        private static readonly CardProvider CardProvider = new CardProvider();

        public static MainViewModel GetMainViewModel()
        {
            return new MainViewModel(Navigation, CardProvider, new SettingsProvider());
        }

        public static NewCardViewModel GetNewCardViewModel()
        {
            return new NewCardViewModel(Navigation,CardProvider);
        }

        public static ViewCardViewModel GetViewCardViewModel(int id)
        {
            return new ViewCardViewModel(Navigation,CardProvider,id);
        }

        public static EditCardViewModel GetEditCardViewModel(int id)
        {
            return new EditCardViewModel(Navigation, CardProvider, id);
        }

        public static SettingsViewModel GetSettingsViewModel()
        {
            return new SettingsViewModel(new SettingsProvider(), Navigation);
        }
    }
}
