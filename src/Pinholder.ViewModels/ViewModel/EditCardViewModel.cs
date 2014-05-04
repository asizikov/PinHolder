using System;
using Curacao.Mvvm.Commands;
using Curacao.Mvvm.ViewModel;
using PinHolder.Annotations;
using PinHolder.Lifecycle;
using PinHolder.Model;
using PinHolder.Navigation;
using PinHolder.PlatformAbstractions;


namespace PinHolder.ViewModel
{
    public sealed class EditCardViewModel : UnsafeBaseViewModel
    {
        [NotNull] private readonly ISecondaryTileService _secondaryTileService;

        private readonly StatisticsService _statistics;
        private readonly IUiStringsProvider _stringsProvider;
        private readonly INavigationService _navigation;
        private readonly BaseCardProvider _cardProvider;
        private CardViewModel _card;

        public EditCardViewModel([NotNull] INavigationService navigation, [NotNull] BaseCardProvider cardProvider,
            [NotNull] ISecondaryTileService secondaryTileService,
            [NotNull] StatisticsService statistics,
            [NotNull] IUiStringsProvider stringsProvider, int id)
        {
            if (navigation == null) throw new ArgumentNullException("navigation");
            if (cardProvider == null) throw new ArgumentNullException("cardProvider");
            if (secondaryTileService == null) throw new ArgumentNullException("secondaryTileService");
            if (statistics == null) throw new ArgumentNullException("statistics");
            if (stringsProvider == null) throw new ArgumentNullException("stringsProvider");
            _navigation = navigation;
            _cardProvider = cardProvider;
            _secondaryTileService = secondaryTileService;
            _statistics = statistics;
            _stringsProvider = stringsProvider;
            Card = _cardProvider.GetById(id).ToViewModel();
            _statistics.PublishEditPageLoaded();
        }


        [UsedImplicitly(ImplicitUseKindFlags.Access)]
        public string Title
        {
            get { return _stringsProvider.Edit; }
        }

        [UsedImplicitly(ImplicitUseKindFlags.Access)]
        public bool DeleteButtonVisible
        {
            get { return true; }
        }

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
            get { return new RelayCommand(Save, _ => CanPerformCommands()); }
        }

        [UsedImplicitly(ImplicitUseKindFlags.Access)]
        public RelayCommand DeleteCommand
        {
            get { return new RelayCommand(Delete); }
        }

        private void Delete()
        {
            _cardProvider.DeleteById(Card.Id);
            _secondaryTileService.DeleteTile(Card.Id);
            _statistics.PublishEditCardDeleted();
            _navigation.GoBack();
        }

        private void Save()
        {
            var model = Card.GetModel();
            if (model != null) _cardProvider.Update(model);
            _statistics.PublishEditCardSaved();
            _navigation.GoBack();
        }

        private bool CanPerformCommands()
        {
            return Card != CardViewModel.Empty;
        }
    }
}