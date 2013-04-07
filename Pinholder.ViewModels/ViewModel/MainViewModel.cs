using System;
using System.Collections.Generic;
using PinHolder.Annotations;
using PinHolder.Command;
using PinHolder.Model;
using PinHolder.Navigation;
using PinHolder.PlatformAbstractions;

namespace PinHolder.ViewModel
{
    public sealed class MainViewModel :BaseViewModel
    {
        private readonly INavigationService _navigation;
        private readonly BaseCardProvider _cardProvider;
        private readonly ICollectionFactory _collectionFactory;

        private CardViewModel _selected;

        public MainViewModel([NotNull] INavigationService navigation, [NotNull] BaseCardProvider cardProvider,
                             [NotNull] ICollectionFactory collectionFactory)
        {
            if (navigation == null) throw new ArgumentNullException("navigation");
            if (cardProvider == null) throw new ArgumentNullException("cardProvider");
            if (collectionFactory == null) throw new ArgumentNullException("collectionFactory");


            _navigation = navigation;
            _cardProvider = cardProvider;
            _collectionFactory = collectionFactory;

            Cards = _collectionFactory.GetCollection<CardViewModel>();
            InitCommands();
            LoadData();
        }


        private void LoadData()
        {
            var cards = _cardProvider.LoadCards().ToViewModelList();
            foreach (var card in cards)
            {
                Cards.Add(card);
            }
        }

        private void InitCommands()
        {
            AddNewCommand = new RelayCommand (() => _navigation.Navigate(Pages.New));
            AboutCommand = new RelayCommand(() => _navigation.Navigate(Pages.About));
            HelpCommand = new RelayCommand(() => _navigation.Navigate(Pages.HelpPage));
        }


        public RelayCommand AddNewCommand { get; private set; }
        public RelayCommand AboutCommand { get; private set; }
        public RelayCommand HelpCommand { get; private set; }


        [NotNull]
        public IList<CardViewModel> Cards { get; private set; }

        [CanBeNull,UsedImplicitly(ImplicitUseKindFlags.Default)]
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