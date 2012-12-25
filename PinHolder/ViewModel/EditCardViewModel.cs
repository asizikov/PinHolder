using System.Windows;
using PinHolder.Annotations;
using PinHolder.Command;
using PinHolder.Lifecycle;
using PinHolder.Model;
using PinHolder.Navigation;
using resx = PinHolder.Resourses;

namespace PinHolder.ViewModel
{
    public sealed class EditCardViewModel: BaseViewModel
    {
        [NotNull] private readonly ISecondaryTileService _secondaryTileService = new SecondaryTileService();
        private readonly NavigationService _navigation;
        private readonly CardProvider _cardProvider;
        private readonly int _id;
        private CardViewModel _card;

        public EditCardViewModel(NavigationService navigation, CardProvider cardProvider, int id)
        {
            _navigation = navigation;
            _cardProvider = cardProvider;
            _id = id;
            Card = _cardProvider.GetById(_id);
        }


        [UsedImplicitly]
        public string Title { get { return resx.Strings.Edit; } }

        [UsedImplicitly]
        public Visibility DeleteButtonVisible { get{ return Visibility.Visible;}}

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

        public RelayCommand SaveCommand
        {
            get 
            {
                return new RelayCommand(Save);
            }
        }

        public RelayCommand DeleteCommand
        {
            get 
            {
                return new RelayCommand(Delete);
            }
        }

        private void Delete()
        {
            _cardProvider.Delete(Card);
            _secondaryTileService.DeleteTile(Card.Id);
            _navigation.GoBack();
        }

        private void Save()
        {
            _cardProvider.Update(Card);
            _navigation.GoBack();
        }
    }
}