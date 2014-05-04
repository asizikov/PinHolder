using System;
using Curacao.Mvvm.Commands;
using Curacao.Mvvm.ViewModel;
using PinHolder.Annotations;
using PinHolder.Lifecycle;
using PinHolder.Model;
using PinHolder.Navigation;
using PinHolder.PlatformAbstractions;

namespace PinHolder.ViewModel
{
    public sealed class ViewCardViewModel : UnsafeBaseViewModel
    {
        private readonly INavigationService _navigation;
        private readonly ISecondaryTileService _secondaryTileService;
        private readonly StatisticsService _statistics;

        private CardViewModel _card;

        public ViewCardViewModel([NotNull] INavigationService navigation, [NotNull] BaseCardProvider cardProvider,
            [NotNull] ISecondaryTileService secondaryTileService, [NotNull] LockerViewModel locker,
            [NotNull] StatisticsService statistics, From from, int id)
        {
            if (navigation == null) throw new ArgumentNullException("navigation");
            if (cardProvider == null) throw new ArgumentNullException("cardProvider");
            if (secondaryTileService == null) throw new ArgumentNullException("secondaryTileService");
            if (locker == null) throw new ArgumentNullException("locker");
            if (statistics == null) throw new ArgumentNullException("statistics");
            _navigation = navigation;
            _secondaryTileService = secondaryTileService;
            _statistics = statistics;
            Card = cardProvider.GetById(id).ToViewModel();
            Locker = locker;
            _statistics.PublishViewCardPageLoaded(from == From.Tile);
        }

        [UsedImplicitly(ImplicitUseKindFlags.Access)]
        public CardViewModel Card
        {
            get { return _card; }
            private set
            {
                _card = value;
                if (_card == CardViewModel.Empty)
                {
                    _navigation.GoBack();
                }
            }
        }

        [UsedImplicitly(ImplicitUseKindFlags.Access)]
        public RelayCommand EditCommand
        {
            get
            {
                return new RelayCommand(() => Locker
                    .Activate(
                        () => _navigation.Navigate(Pages.New, string.Format("?{0}={1}", Keys.Id, Card.Id))),
                    _ => CanPerformCommand()
                    );
            }
        }

        [UsedImplicitly(ImplicitUseKindFlags.Access)]
        public RelayCommand CreatePinCommand
        {
            get
            {
                return new RelayCommand(
                    () =>
                    {
                        _statistics.PublishViewCardPinButtonClick();
                        _secondaryTileService.TryCreate(Card.Name, Card.Description, Card.Id, () => { });
                    },
                    _ => CanPerformCommand() && _secondaryTileService.CanCreate(Card.Id));
            }
        }

        [NotNull]
        [UsedImplicitly(ImplicitUseKindFlags.Access)]
        public LockerViewModel Locker { get; private set; }

        private bool CanPerformCommand()
        {
            return Card != CardViewModel.Empty;
        }
    }
}