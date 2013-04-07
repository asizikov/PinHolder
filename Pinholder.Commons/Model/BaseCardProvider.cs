using System;
using System.Collections.Generic;
using System.Linq;

namespace PinHolder.Model
{
    public abstract class BaseCardProvider
    {
        protected const string FOLDER = "Cards";
        protected const string FILE = "cards.xml";

        protected readonly List<Card> _cards = new List<Card>();

        public void Save(Card newCardModel)
        {
            {
                if (newCardModel == null) throw new ArgumentNullException("newCardModel");

                var x = _cards.Any() ? _cards.Max(c => c.Id) : 0;
                newCardModel.Id = x + 1;
                _cards.Add(newCardModel);
                UpdateFile(_cards);
            }
        }

        public void Update(Card card)
        {
            var old = _cards.FirstOrDefault(c => c.Id == card.Id);
            if (old == null)
            {
                Save(card);
            }
            else
            {
                _cards.Remove(old);
                _cards.Add(card);
                UpdateFile(_cards);
            }
        }

        public void DeleteById(int id)
        {
            var old = _cards.FirstOrDefault(c => c.Id == id);
            if (old == null) return;

            _cards.Remove(old);
            UpdateFile(_cards);
        }

        public virtual IEnumerable<Card> LoadCards()
        {
            return _cards.ToList();
        }

        public Card GetById(int id)
        {
            return _cards.FirstOrDefault(c => c.Id == id);
        }


        protected abstract void UpdateFile(List<Card> cards);

        protected static string PathToFile
        {
            get { return FOLDER + "\\" + FILE; }
        }
    }
}