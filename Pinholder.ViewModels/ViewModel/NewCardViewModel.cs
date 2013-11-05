using System;
using PinHolder.Annotations;
using PinHolder.Command;
using PinHolder.Model;
using PinHolder.Navigation;
using PinHolder.PlatformAbstractions;

namespace PinHolder.ViewModel
{
    public sealed class NewCardViewModel : BaseViewModel
    {
        private readonly INavigationService _navigation;
        private readonly BaseCardProvider _cardProvider;
        private readonly IUiStringsProvider _stringsProvider;
        private readonly StatisticsService _statistics;
        private CardViewModel _card;
        private bool _canSave;

        public NewCardViewModel([NotNull] INavigationService navigation, [NotNull] BaseCardProvider cardProvider,
                                [NotNull] IUiStringsProvider stringsProvider, [NotNull] StatisticsService statistics)
        {
            if (stringsProvider == null) throw new ArgumentNullException("stringsProvider");
            if (statistics == null) throw new ArgumentNullException("statistics");
            _navigation = navigation;
            _cardProvider = cardProvider;
            _stringsProvider = stringsProvider;
            _statistics = statistics;
            Card = new CardViewModel();
            Card.ReadyToSave += () =>
                {
                    _canSave = true;
                    SaveCommand.RaiseCanExecuteChanged();
                };
            Card.NameOfDescriptionUpdated += () => SaveCommand.RaiseCanExecuteChanged();
            _statistics.PublishNewCardPageLoaded();
        }

        [UsedImplicitly]
        public string Title { get { return _stringsProvider.New; } }

        [UsedImplicitly]
        public bool DeleteButtonVisible
        {
            get
            {
                return false;
            }
        }

        private RelayCommand _saveCommand;

        [UsedImplicitly]
        public RelayCommand SaveCommand
        {
            get
            {
                return _saveCommand
                    ?? (_saveCommand = new RelayCommand(SaveCard, CanSave));
            }
        }

        private RelayCommand _deleteCommand;

        [UsedImplicitly]
        public RelayCommand DeleteCommand
        {
            get
            {
                return _deleteCommand ?? (_deleteCommand = new RelayCommand(() => { }, () => false));
            }
        }

        [NotNull, UsedImplicitly]
        public CardViewModel Card
        {
            get { return _card; }
            set
            {
                if (Equals(value, _card)) return;
                _card = value;
                OnPropertyChanged("Card");
            }
        }


        private bool CanSave()
        {
            return _canSave && !string.IsNullOrEmpty(Card.Name);
        }

        private void SaveCard()
        {
            _cardProvider.Save(Card.GetModel());
            _statistics.PublishNewCardSaveCardButtonClick();
            _navigation.GoBack();
        }
    }
}