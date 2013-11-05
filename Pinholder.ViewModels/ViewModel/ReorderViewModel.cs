using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using PinHolder.Annotations;
using PinHolder.Command;
using PinHolder.Model;
using PinHolder.PlatformAbstractions;

namespace PinHolder.ViewModel
{
    public class ReorderViewModel : BaseViewModel
    {
        private readonly BaseCardProvider _cardProvider;
        private readonly StatisticsService _statistics;


        public ReorderViewModel([NotNull] BaseCardProvider cardProvider,
            [NotNull] ICollectionFactory collectionFactory, [NotNull] StatisticsService statistics)
        {
            if (cardProvider == null) throw new ArgumentNullException("cardProvider");
            if (collectionFactory == null) throw new ArgumentNullException("collectionFactory");
            if (statistics == null) throw new ArgumentNullException("statistics");

            _cardProvider = cardProvider;
            _statistics = statistics;

            Cards = collectionFactory.GetCollection<CardViewModel>();
            LoadData();
            ApplyChangesCommand = new RelayCommand(SaveChanges);
            _statistics.PublishReorderPageLoaded();
        }

        private void SaveChanges()
        {
           _cardProvider.UpdateList(Cards.Select(cvm => cvm.GetModel()));
        }

        private void LoadData()
        {
            var cards = _cardProvider.LoadCards().ToViewModelList();
            foreach (var card in cards)
            {
                Cards.Add(card);
            }
        }

        [NotNull]
        public IList<CardViewModel> Cards { get; private set; }

        public ICommand ApplyChangesCommand { get; private set; }
    }
}