using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using PinHolder.Annotations;
using PinHolder.Model;

namespace PinHolder.DeveloperTools
{
    public class TestDataGenerator
    {
        private readonly BaseCardProvider _cardProvider;

        public TestDataGenerator([NotNull] BaseCardProvider cardProvider)
        {
            if (cardProvider == null) throw new ArgumentNullException("cardProvider");
            _cardProvider = cardProvider;
        }

        public int CreateTestData()
        {
            var token = DateTime.Now.Millisecond;
            var cards =  _cardProvider.LoadCards().ToList();
            foreach (var index in Enumerable.Range(0,5))
            {
                var card =new Card
                {
                    Description = "test bank description",
                    Name = "ABN AMRO"+ index,
                    Id = index + token,
                    Pins = new List<string>(20)

                };
                for (var i = 0; i < 20; i++)
                {
                    card.Pins.Add(i.ToString(CultureInfo.InvariantCulture));
                }
                cards.Add(card);
            }
            _cardProvider.UpdateList(cards);
            return cards.Count;
        }

        public int DeleteAllCards()
        {
           _cardProvider.UpdateList(new List<Card>());
            return 0;
        }
    }
}
