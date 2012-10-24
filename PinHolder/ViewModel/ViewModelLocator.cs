using PinHolder.Model;
using PinHolder.Navigation;

namespace PinHolder.ViewModel
{
    public static class ViewModelLocator
    {

        public static NavigationService Navigation { get; set; }

        private static readonly CardProvider CardProvider = new CardProvider();

        public static MainViewModel GetMainViewModel()
        {
            return new MainViewModel(Navigation,CardProvider);
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
    }
}
