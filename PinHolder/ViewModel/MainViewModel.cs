using System.Collections.ObjectModel;
using PinHolder.Annotations;
using PinHolder.Command;
using PinHolder.Lifecycle;
using PinHolder.Model;
using PinHolder.Navigation;

namespace PinHolder.ViewModel
{
    public sealed class MainViewModel :BaseViewModel
    {
        [NotNull] private readonly INavigationService _navigation;
        [NotNull] private readonly ICardProvider _cardProvider;
        [NotNull] private readonly ISettingsProvider _settingsProvider;

        private CardViewModel _selected;
        private bool _showLocker;

        public MainViewModel(INavigationService navigation, ICardProvider cardProvider, ISettingsProvider settingsProvider)
        {
            _navigation = navigation;
            _cardProvider = cardProvider;
            _settingsProvider = settingsProvider;
            Cards = new ObservableCollection<CardViewModel>();
            InitCommands();
            ApplySettings();
            LoadData();
        }

        private void ApplySettings()
        {
            ShowLocker = false;
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
            SettingsCommand = new RelayCommand(GoToSettings);
            AboutCommand = new RelayCommand(GoToAbout);
        }

        

        [UsedImplicitly]
        public bool ShowLocker
        {
            get { return _showLocker; }
            set
            {
                if (value.Equals(_showLocker)) return;
                _showLocker = value;
                OnPropertyChanged("ShowLocker");
            }
        }

        public RelayCommand AddNewCommand { get; private set; }
        public RelayCommand SettingsCommand { get; private set; }
        public RelayCommand AboutCommand { get; private set; }


        [NotNull]
        public ObservableCollection<CardViewModel> Cards { get; set; }

        [CanBeNull]
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

        private void AddNewCard()
        {
            _navigation.Navigate(Pages.New);
        }

        private void GoToSettings()
        {
            _navigation.Navigate(Pages.Settings);
        }

        private void GoToAbout()
        {
           _navigation.Navigate(Pages.About);
        }
    }
}