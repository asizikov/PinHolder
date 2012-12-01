using PinHolder.Command;
using PinHolder.Lifecycle;
using PinHolder.Model;
using PinHolder.Navigation;

namespace PinHolder.ViewModel
{
    public sealed class ViewCardViewModel: BaseViewModel
    {
        private readonly NavigationService _navigation;
        private readonly CardProvider _cardProvider;
        private readonly int _id;
        private CardViewModel _card;
        
        private readonly ISecondaryTileService _secondaryTileService = new SecondaryTileService();

        public ViewCardViewModel(NavigationService navigation, CardProvider cardProvider, int id)
        {
            _navigation = navigation;
            _cardProvider = cardProvider;
            _id = id;
            Card = _cardProvider.GetById(_id);
        }

        public CardViewModel Card
        {
            get {
                return _card;
            }
            set {
                _card = value;
                if (_card == CardViewModel.Empty)
                {
                    _navigation.GoBack();
                }
            }
        }

        public RelayCommand EditCommand
        {
            get {
                return new RelayCommand(()=> _navigation.Navigate(Pages.New,
                    string.Format("?{0}={1}", Keys.Id, Card.Id)));
            }
        }

        public RelayCommand CreatePinCommand
        {
            get
            {
                return new RelayCommand(
                    ()=> _secondaryTileService.TryCreate(Card.Name, Card.Id, () => { }),
                    ()=> _secondaryTileService.CanCreate(Card.Id));
            }
        }
    }
}