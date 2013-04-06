using System.Windows;
using PinHolder.Annotations;
using PinHolder.Command;
using PinHolder.Model;
using PinHolder.Navigation;
using resx = PinHolder.Resourses;

namespace PinHolder.ViewModel
{
    public sealed class NewCardViewModel : BaseViewModel
    {
        private readonly INavigationService _navigation;
        private readonly BaseCardProvider _cardProvider;
        private CardViewModel _card;
        private bool _canSave;

        public NewCardViewModel([NotNull] INavigationService navigation, [NotNull] BaseCardProvider cardProvider)
        {
            _navigation = navigation;
            _cardProvider = cardProvider;
            Card = new CardViewModel();
            Card.ReadyToSave += () =>
                {
                    _canSave = true;
                    SaveCommand.RaiseCanExecuteChanged();
                };
            Card.PropertyChanged += (sender, args) =>
                {
                    if (args.PropertyName == "Description" || args.PropertyName == "Name")
                    {
                        SaveCommand.RaiseCanExecuteChanged();
                    }
                };
        }

        [UsedImplicitly]
        public string Title { get { return resx.Strings.New; } }

        [UsedImplicitly]
        public Visibility DeleteButtonVisible
        {
            get
            {
                return Visibility.Collapsed;
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
            return _canSave && !string.IsNullOrWhiteSpace(Card.Name);
        }

        private void SaveCard()
        {
            _cardProvider.Save(Card);
            _navigation.GoBack();
        }
    }
}