﻿using System;
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
        private readonly INavigationService _navigation;
        private readonly ICardProvider _cardProvider;
        private readonly ISettingsProvider _settingsProvider;

        private CardViewModel _selected;
        private bool _showLocker;

        public MainViewModel([NotNull] INavigationService navigation, [NotNull] ICardProvider cardProvider,
                             [NotNull] ISettingsProvider settingsProvider)
        {
            if (navigation == null) throw new ArgumentNullException("navigation");
            if (cardProvider == null) throw new ArgumentNullException("cardProvider");
            if (settingsProvider == null) throw new ArgumentNullException("settingsProvider");

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
            AddNewCommand = new RelayCommand(() => _navigation.Navigate(Pages.New));
            SettingsCommand = new RelayCommand(() => _navigation.Navigate(Pages.Settings));
            AboutCommand = new RelayCommand(() => _navigation.Navigate(Pages.About));
            HelpCommand = new RelayCommand(()=> _navigation.Navigate(Pages.HelpPage));
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
        public RelayCommand HelpCommand { get; private set; }


        [NotNull]
        public ObservableCollection<CardViewModel> Cards { get; private set; }

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
    }
}