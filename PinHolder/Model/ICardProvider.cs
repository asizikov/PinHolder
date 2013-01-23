using System.Collections.Generic;
using PinHolder.ViewModel;

namespace PinHolder.Model
{
    public interface ICardProvider
    {
        void Save(CardViewModel newCardViewModel);
        void Update(CardViewModel card);
        void Delete(CardViewModel card);
        IEnumerable<CardViewModel> LoadCards();
        CardViewModel GetById(int id);
    }
}