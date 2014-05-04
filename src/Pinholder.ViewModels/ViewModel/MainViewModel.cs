using System;
using System.Collections.Generic;
using Curacao.Mvvm.Commands;
using Curacao.Mvvm.ViewModel;
using PinHolder.Annotations;
using PinHolder.Model;
using PinHolder.Navigation;
using PinHolder.PlatformAbstractions;

namespace PinHolder.ViewModel
{
    public sealed class MainViewModel : UnsafeBaseViewModel
    {
        private readonly INavigationService _navigation;
        private readonly BaseCardProvider _cardProvider;
        private readonly StatisticsService _statistics;

        private CardViewModel _selected;

        public MainViewModel([NotNull] INavigationService navigation, [NotNull] BaseCardProvider cardProvider,
            [NotNull] ICollectionFactory collectionFactory, [NotNull] StatisticsService statistics)
        {
            if (navigation == null) throw new ArgumentNullException("navigation");
            if (cardProvider == null) throw new ArgumentNullException("cardProvider");
            if (collectionFactory == null) throw new ArgumentNullException("collectionFactory");
            if (statistics == null) throw new ArgumentNullException("statistics");


            _navigation = navigation;
            _cardProvider = cardProvider;
            _statistics = statistics;

            Cards = collectionFactory.GetCollection<CardViewModel>();
            InitCommands();
            LoadData();
            _statistics.PublishMainPageLoaded(Cards == null ? 0 : Cards.Count);
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
            AddNewCommand = new RelayCommand(_ =>
            {
                _statistics.PublishMainAddNewButtonClick();
                _navigation.Navigate(Pages.New);
            });
            AboutCommand = new RelayCommand(_ =>
            {
                _statistics.PublishMainAboutButtonClick();
                _navigation.Navigate(Pages.About);
            });
            HelpCommand = new RelayCommand(_ =>
            {
                _statistics.PublishMainHelpButtonClick();
                _navigation.Navigate(Pages.HelpPage);
            });
            ReorderCommand = new RelayCommand(_ =>
            {
                _statistics.PublishMainReorderButtonClick();
                _navigation.Navigate(Pages.Reorder);
            });
        }

        [UsedImplicitly(ImplicitUseKindFlags.Access)]
        public RelayCommand AddNewCommand { get; private set; }

        [UsedImplicitly(ImplicitUseKindFlags.Access)]
        public RelayCommand AboutCommand { get; private set; }

        [UsedImplicitly(ImplicitUseKindFlags.Access)]
        public RelayCommand HelpCommand { get; private set; }

        [UsedImplicitly(ImplicitUseKindFlags.Access)]
        public RelayCommand ReorderCommand { get; private set; }


        [NotNull]
        public IList<CardViewModel> Cards { get; private set; }

        [CanBeNull, UsedImplicitly(ImplicitUseKindFlags.Default)]
        public CardViewModel Selected
        {
            get { return _selected; }
            set
            {
                _selected = value;
                if (value != null)
                {
                    var uri = string.Format("?{0}={1}&{2}={3}", Keys.Id, _selected.Id, Keys.From, From.MainPage);
                    _navigation.Navigate(Pages.ViewPage, uri);
                }
            }
        }
    }
}