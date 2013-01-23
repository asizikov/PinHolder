using System.Collections.Generic;
using System.Globalization;
using NUnit.Framework;
using PinHolder.Model;
using PinHolder.ViewModel;

namespace PinHolder.Tests
{
    [TestFixture]
    public class CardViewModelTestFixture
    {

        private const int PINS_NUMBER = 20;

        [Test]
        public void DefaultConstructorBuildsEmptyCardViewModel()
        {
            var target = new CardViewModel();
            Assert.NotNull(target.PinItems);
            Assert.IsTrue(string.IsNullOrEmpty(target.Description));
            Assert.IsTrue(string.IsNullOrEmpty(target.Name));

            foreach (var t in target.PinItems)
            {
                Assert.IsTrue(string.IsNullOrEmpty(t.Pin));
            }
        }

        [Test]
        public void CardPropertiesAreStoredInCardViewModelProperties()
        {
            var card = new Card
                           {
                               Description = "description",
                               Name = "name",
                               Id = 1,
                               Pins = new List<string>()
                           };
            for (var i = 0; i < PINS_NUMBER; i++)
            {
                card.Pins.Add(i.ToString(CultureInfo.InvariantCulture));
            }

            var target = new CardViewModel(card);

            Assert.AreEqual(card.Name, target.Name);
            Assert.AreEqual(card.Description, target.Description);
            Assert.AreEqual(card.Id, target.Id);

            var index = 0;
            foreach (var pinItem in target.PinItems)
            {
                Assert.AreEqual(card.Pins[index], pinItem.Pin);
                index++;
            }
        }

        [Test]
        public void CardViewModelFillsAllPinItemsAfterUserAddedFourDigits()
        {
            var target = new CardViewModel();
            target.PinItems[0].Pin = "1";
            target.PinItems[1].Pin = "1";
            target.PinItems[2].Pin = "1";
            target.PinItems[3].Pin = "1";

            foreach (var pinItem in target.PinItems)
            {
                Assert.IsFalse(string.IsNullOrEmpty(pinItem.Pin));
            }
        }

        [Test]
        public void DoesNotFillAllPinItemsAfterUserChangedTheSamePinItemFourTimes()
        {
            var target = new CardViewModel();
            for (int i = 0; i < 4; i++)
            {
                target.PinItems[0].Pin = i.ToString(CultureInfo.InvariantCulture);
            }

            for (int index = 1; index < PINS_NUMBER; index++)
            {
                Assert.IsTrue(string.IsNullOrEmpty(target.PinItems[index].Pin));
            }
        }

        [Test]
        public void RaisesReadyToSaveAfterUserAddedAllDigits()
        {
            bool raised = false;
            var target = new CardViewModel();
            target.ReadyToSave += () => raised = true;
            target.PinItems[0].Pin = "1";
            target.PinItems[1].Pin = "1";
            target.PinItems[2].Pin = "1";
            target.PinItems[3].Pin = "1";

            Assert.IsTrue(raised);
            
        }
    }
}
