using System;
using PinHolder.Annotations;
using PinHolder.Command;
using PinHolder.Lifecycle;
using PinHolder.Model;
using PinHolder.Navigation;

namespace PinHolder.ViewModel
{
    public sealed class ViewCardViewModel: BaseViewModel
    {
        private readonly INavigationService _navigation;
        private readonly BaseCardProvider _cardProvider;
        private readonly int _id;
        private CardViewModel _card;

        [NotNull] private readonly ISecondaryTileService _secondaryTileService;

        public ViewCardViewModel([NotNull] INavigationService navigation, [NotNull] BaseCardProvider cardProvider,
                                 [NotNull] ISecondaryTileService secondaryTileService, int id)
        {
            if (navigation == null) throw new ArgumentNullException("navigation");
            if (cardProvider == null) throw new ArgumentNullException("cardProvider");
            if (secondaryTileService == null) throw new ArgumentNullException("secondaryTileService");
            _navigation = navigation;
            _cardProvider = cardProvider;
            _secondaryTileService = secondaryTileService;
            _id = id;
            Card = _cardProvider.GetById(_id).ToViewModel();
        }

        [UsedImplicitly]
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

        [UsedImplicitly]
        public RelayCommand EditCommand
        {
            get {
                return new RelayCommand(()=> _navigation.Navigate(Pages.New,
                    string.Format("?{0}={1}", Keys.Id, Card.Id)));
            }
        }

        [UsedImplicitly]
        public RelayCommand CreatePinCommand
        {
            get
            {
                return new RelayCommand(
                    ()=> _secondaryTileService.TryCreate(Card.Name, Card.Description, Card.Id, () => { }),
                    ()=> _secondaryTileService.CanCreate(Card.Id));
            }
        }
    }
}