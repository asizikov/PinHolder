using System;
using System.Collections.Generic;
using System.Linq;
using PinHolder.Annotations;

namespace PinHolder.Model
{
    public abstract class BaseCardProvider
    {
        protected const string Folder = "Cards";
        protected const string File = "cards.xml";

        protected readonly List<Card> Cards = new List<Card>();

        public void Save([NotNull] Card newCardModel)
        {
            if (newCardModel == null) throw new ArgumentNullException("newCardModel");

            var x = Cards.Any() ? Cards.Max(c => c.Id) : 0;
            newCardModel.Id = x + 1;
            Cards.Add(newCardModel);
            UpdateFile(Cards);
        }

        public void Update([NotNull] Card card)
        {
            if (card == null) throw new ArgumentNullException("card", "Null was passed to BaseCardProvider.Update");

            var old = Cards.FirstOrDefault(c => c.Id == card.Id);
            if (old == null)
            {
                Save(card);
            }
            else
            {
                Cards.Remove(old);
                Cards.Add(card);
                UpdateFile(Cards);
            }
        }

        public void DeleteById(int id)
        {
            var old = Cards.FirstOrDefault(c => c.Id == id);
            if (old == null) return;

            Cards.Remove(old);
            UpdateFile(Cards);
        }

        public virtual IEnumerable<Card> LoadCards()
        {
            return Cards.ToList();
        }

        [CanBeNull]
        public Card GetById(int id)
        {
            return Cards.FirstOrDefault(c => c.Id == id);
        }


        protected abstract void UpdateFile(List<Card> cards);

        protected static string PathToFile
        {
            get { return Folder + "\\" + File; }
        }
    }
}