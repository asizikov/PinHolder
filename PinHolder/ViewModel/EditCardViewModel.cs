using System.Windows;
using PinHolder.Command;
using PinHolder.Model;
using PinHolder.Navigation;
using resx = PinHolder.Resourses;

namespace PinHolder.ViewModel
{
    public class EditCardViewModel: BaseViewModel
    {
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


        public string Title { get { return resx.Strings.Edit; } }

        public Visibility DeleteButtonVisible {get{return Visibility.Visible;}}

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
            _navigation.GoBack();
        }

        private void Save()
        {
            _cardProvider.Update(Card);
            _navigation.GoBack();
        }
    }
}