using System.Collections.Generic;
using System.Linq;
using PinHolder.ViewModel;

namespace PinHolder.Model
{
    public static class CardExtentions
    {
        public static CardViewModel ToViewModel(this Card card)
        {
            return card != null ? new CardViewModel(card) : CardViewModel.Empty;
        }

        public static IEnumerable<CardViewModel> ToViewModelList(this IEnumerable<Card> cardsList)
        {
            return cardsList.Select(ToViewModel);
        }
    }
}