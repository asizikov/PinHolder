using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using PinHolder.ViewModel;

namespace PinHolder.Model
{
    public class CardsHolder
    {
        public List<Card> Cards;
    }

    public class CardProvider
    {
        private readonly List<Card> _cards;
        private const string FOLDER = "Cards";
        private const string FILE = "cards.xml";

        public CardProvider()
        {
            _cards = new List<Card>();
            LoadCardsFromStorage();
        }

        protected string PathToFile
        {
            get { return FOLDER + "\\" + FILE; }
        }

       public void Save(CardViewModel newCardViewModel)
        {
            if (newCardViewModel == null) throw new ArgumentNullException("newCardViewModel");

            var x = _cards.Any() ? _cards.Max(c => c.Id) : 0;
            newCardViewModel.Id = x + 1;
            _cards.Add(newCardViewModel.GetModel());
            UpdateFile(_cards);
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

        private void UpdateFile(List<Card> cards)
        {
            using (var st = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (!CheckDir(st)) return;
                using (var writeFile = new StreamWriter(
                    new IsolatedStorageFileStream(PathToFile, FileMode.Create, st)))
                {
                    var holder = new CardsHolder
                        {
                            Cards = cards
                        };

                    var xml = new XmlSerializer(holder.GetType());
                    xml.Serialize(writeFile, holder);  
                } 
            }
        }


        public IEnumerable<CardViewModel> LoadCards()
        {
            return _cards.Select(m => new CardViewModel(m)).ToList();
        }

        public CardViewModel GetById(int id)
        {
            return new CardViewModel(_cards.FirstOrDefault(c => c.Id == id));
        }


        private void LoadCardsFromStorage()
        {
            using (var storage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (!CheckDir(storage)) return;
                if (!storage.FileExists(PathToFile)) return;

                using (var stream = storage.OpenFile(PathToFile, FileMode.Open))
                {
                    var xml = new XmlSerializer(typeof(CardsHolder));
                    var holder = xml.Deserialize(stream) as CardsHolder;
                    if (holder == null) return;
                    _cards.Clear();
                    _cards.AddRange(holder.Cards);
                }
            }
        }

        private static bool CheckDir(IsolatedStorageFile storage)
        {
            if (!storage.DirectoryExists(FOLDER))
            {
                try
                {
                    storage.CreateDirectory(FOLDER);
                }
                catch (Exception)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
