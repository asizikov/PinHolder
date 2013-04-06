using System;
using System.Collections.Generic;
using System.Linq;
using PinHolder.ViewModel;

namespace PinHolder.Model
{
    public abstract class BaseCardProvider
    {
        protected readonly List<Card> _cards = new List<Card>();

        public void Save(CardViewModel newCardViewModel)
        {
            {
                if (newCardViewModel == null) throw new ArgumentNullException("newCardViewModel");

                var x = _cards.Any() ? _cards.Max(c => c.Id) : 0;
                newCardViewModel.Id = x + 1;
                _cards.Add(newCardViewModel.GetModel());
                UpdateFile(_cards);
            }
        }

        public void Update(CardViewModel card)
        {
            var old = _cards.FirstOrDefault(c => c.Id == card.Id);
            if (old == null)
            {
                Save(card);
            }
            else
            {
                _cards.Remove(old);
                _cards.Add(card.GetModel());
                UpdateFile(_cards);
            }
        }

        public void Delete(CardViewModel card)
        {
            var old = _cards.FirstOrDefault(c => c.Id == card.Id);
            if (old == null) return;

            _cards.Remove(old);
            UpdateFile(_cards);
        }

        public virtual IEnumerable<CardViewModel> LoadCards()
        {
            return _cards.Select(m => new CardViewModel(m)).ToList();
        }

        public CardViewModel GetById(int id)
        {
            var model = _cards.FirstOrDefault(c => c.Id == id);

            return model != null ? new CardViewModel(model) : CardViewModel.Empty;
        }


        protected abstract void UpdateFile(List<Card> cards);
    }
}