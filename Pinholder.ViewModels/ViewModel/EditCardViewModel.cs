using System;
using PinHolder.Annotations;
using PinHolder.Command;
using PinHolder.Lifecycle;
using PinHolder.Model;
using PinHolder.Navigation;
using PinHolder.PlatformAbstractions;


namespace PinHolder.ViewModel
{
    public sealed class EditCardViewModel: BaseViewModel
    {
        [NotNull] private readonly ISecondaryTileService _secondaryTileService;
        private readonly IUiStringsProvider _stringsProvider;
        private readonly INavigationService _navigation;
        private readonly BaseCardProvider _cardProvider;
        private readonly int _id;
        private CardViewModel _card;

        public EditCardViewModel([NotNull] INavigationService navigation, [NotNull] BaseCardProvider cardProvider,
                                 [NotNull] ISecondaryTileService secondaryTileService,
                                 [NotNull] IUiStringsProvider stringsProvider, int id)
        {
            if (navigation == null) throw new ArgumentNullException("navigation");
            if (cardProvider == null) throw new ArgumentNullException("cardProvider");
            if (secondaryTileService == null) throw new ArgumentNullException("secondaryTileService");
            if (stringsProvider == null) throw new ArgumentNullException("stringsProvider");
            _navigation = navigation;
            _cardProvider = cardProvider;
            _secondaryTileService = secondaryTileService;
            _stringsProvider = stringsProvider;
            _id = id;
            Card = _cardProvider.GetById(_id).ToViewModel();
        }


        [UsedImplicitly(ImplicitUseKindFlags.Access)]
        public string Title { get { return _stringsProvider.Edit; } }

        [UsedImplicitly(ImplicitUseKindFlags.Access)]
        public bool DeleteButtonVisible { get { return true; } }

        [UsedImplicitly(ImplicitUseKindFlags.Access)]
        public CardViewModel Card
        {
            get { return _card; }
            private set
            {
                if (Equals(value, _card)) return;
                _card = value;
                OnPropertyChanged("Card");
            }
        }

        [UsedImplicitly(ImplicitUseKindFlags.Access)]
        public RelayCommand SaveCommand
        {
            get 
            {
                return new RelayCommand(Save);
            }
        }

        [UsedImplicitly(ImplicitUseKindFlags.Access)]
        public RelayCommand DeleteCommand
        {
            get 
            {
                return new RelayCommand(Delete);
            }
        }

        private void Delete()
        {
            _cardProvider.DeleteById(Card.Id);
            _secondaryTileService.DeleteTile(Card.Id);
            _navigation.GoBack();
        }

        private void Save()
        {
            _cardProvider.Update(Card.GetModel());
            _navigation.GoBack();
        }
    }
}