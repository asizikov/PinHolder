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
        private readonly ISecondaryTileService _secondaryTileService;

        private readonly int _id;
        private CardViewModel _card;


        public ViewCardViewModel([NotNull] INavigationService navigation, [NotNull] BaseCardProvider cardProvider,
                                 [NotNull] ISecondaryTileService secondaryTileService, [NotNull] LockerViewModel locker, 
                                 int id)
        {
            if (navigation == null) throw new ArgumentNullException("navigation");
            if (cardProvider == null) throw new ArgumentNullException("cardProvider");
            if (secondaryTileService == null) throw new ArgumentNullException("secondaryTileService");
            if (locker == null) throw new ArgumentNullException("locker");
            _navigation = navigation;
            _cardProvider = cardProvider;
            _secondaryTileService = secondaryTileService;
            _id = id;

            Card = _cardProvider.GetById(_id).ToViewModel();
            Locker = locker;
        }

        [UsedImplicitly(ImplicitUseKindFlags.Access)]
        public CardViewModel Card
        {
            get {
                return _card;
            }
            private set {
                _card = value;
                if (_card == CardViewModel.Empty)
                {
                    _navigation.GoBack();
                }
            }
        }

        [UsedImplicitly(ImplicitUseKindFlags.Access)]
        public RelayCommand EditCommand
        {
            get {
                return new RelayCommand(()=> Locker
                    .Activate(
                    () => _navigation.Navigate(Pages.New,string.Format("?{0}={1}", Keys.Id, Card.Id)))
                    );
                
            }
        }

        [UsedImplicitly(ImplicitUseKindFlags.Access)]
        public RelayCommand CreatePinCommand
        {
            get
            {
                return new RelayCommand(
                    ()=> _secondaryTileService.TryCreate(Card.Name, Card.Description, Card.Id, () => { }),
                    ()=> _secondaryTileService.CanCreate(Card.Id));
            }
        }

        [NotNull]
        [UsedImplicitly(ImplicitUseKindFlags.Access)]
        public LockerViewModel Locker { get; private set; }
    }
}