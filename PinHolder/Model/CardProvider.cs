using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;

namespace PinHolder.Model
{
    public sealed class CardsHolder
    {
        public List<Card> Cards;
    }


    public sealed class CardProvider: BaseCardProvider
    {
        public CardProvider()
        {
            LoadCardsFromStorage();
        }


        protected override void UpdateFile(List<Card> cards)
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
