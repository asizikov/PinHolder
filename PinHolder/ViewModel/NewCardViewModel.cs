using System.Windows;
using PinHolder.Command;
using PinHolder.Model;
using PinHolder.Navigation;
using resx = PinHolder.Resourses;

namespace PinHolder.ViewModel
{
    public class NewCardViewModel : BaseViewModel
    {
        private readonly NavigationService _navigation;
        private readonly CardProvider _cardProvider;
        private CardViewModel _card;
        private bool _canSave;

        public NewCardViewModel(NavigationService navigation, CardProvider cardProvider)
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
                    if (args.PropertyName == "Description")
                    {
                        SaveCommand.RaiseCanExecuteChanged();
                    }
                };
        }

        public string Title { get { return resx.Strings.New; } }
        public Visibility DeleteButtonVisible
        {
            get
            {
                return Visibility.Collapsed;
            }
        }

        private RelayCommand _saveCommand;
        public RelayCommand SaveCommand
        {
            get
            {
                return _saveCommand
                    ?? (_saveCommand = new RelayCommand(SaveCard, CanSave));
            }
        }

        private RelayCommand _deleteCommand;
        public RelayCommand DeleteCommand
        {
            get
            {
                return _deleteCommand ?? (_deleteCommand = new RelayCommand(() => { }, () => false));
            }
        }

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