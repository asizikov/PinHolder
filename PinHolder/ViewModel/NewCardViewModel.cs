using System.Windows;
using PinHolder.Command;
using PinHolder.Model;
using PinHolder.Navigation;

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
        }

        public string Title { get { return "new"; } }
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
                    ?? (_saveCommand = new RelayCommand(SaveCard, () => _canSave));
            }
        }

        private void SaveCard()
        {
            _cardProvider.Save(Card);
            _navigation.GoBack();
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
    }
}