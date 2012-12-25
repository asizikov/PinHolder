using System.Collections.ObjectModel;
using PinHolder.Annotations;
using PinHolder.Command;
using PinHolder.Model;
using PinHolder.Navigation;

namespace PinHolder.ViewModel
{
    public sealed class MainViewModel :BaseViewModel
    {
        [NotNull] private readonly NavigationService _navigation;
        [NotNull] private readonly CardProvider _cardProvider;
        private CardViewModel _selected;

        public MainViewModel(NavigationService navigation, CardProvider cardProvider)
        {
            _navigation = navigation;
            _cardProvider = cardProvider;
            Cards = new ObservableCollection<CardViewModel>();
            InitCommands();
            LoadData();
        }

        private void LoadData()
        {
            var cards = _cardProvider.LoadCards();
            foreach (var card in cards)
            {
                Cards.Add(card);
            }
        }

        private void InitCommands()
        {
            AddNewCommand = new RelayCommand(AddNewCard);
        }


        public RelayCommand AddNewCommand { get; set; }

        private void AddNewCard()
        {
            _navigation.Navigate(Pages.New);
        }

        public ObservableCollection<CardViewModel> Cards { get; set; }

        public CardViewModel Selected
        {
            get { return _selected; }
            set {
                _selected = value;
                if (value != null)
                {
                    _navigation.Navigate(Pages.ViewPage, string.Format("?{0}={1}", Keys.Id, _selected.Id));                    
                }
            }
        }
    }
}