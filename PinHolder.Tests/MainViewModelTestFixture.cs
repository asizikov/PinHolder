using System.Collections.Generic;
using System.Globalization;
using Moq;
using NUnit.Framework;
using PinHolder.Lifecycle;
using PinHolder.Model;
using PinHolder.Navigation;
using PinHolder.ViewModel;

namespace PinHolder.Tests
{
    [TestFixture]
    class MainViewModelTestFixture
    {
        [Test]
        public void DoesNotShowLockerByDefault()
        {
            var settings = new Mock<ISettingsProvider>();
            var navigation = new Mock<INavigationService>();
            var cardProvider = new Mock<BaseCardProvider>();

            var target = new MainViewModel(navigation.Object, cardProvider.Object, settings.Object);

            Assert.IsFalse(target.ShowLocker);
        }

        [Test]
        public void LoadsCardsOnStart()
        {

            var settings = new Mock<ISettingsProvider>();
            var navigation = new Mock<INavigationService>();
            var cardProvider = new Mock<BaseCardProvider>();

            cardProvider.Setup(x => x.LoadCards())
                .Returns(() => new List<CardViewModel> {
                    new CardViewModel(GetCard())});

            var target = new MainViewModel(navigation.Object, cardProvider.Object, settings.Object);
            cardProvider.Verify(x => x.LoadCards(), Times.Once());
            Assert.AreEqual(1, target.Cards.Count);
            Assert.AreEqual(1, target.Cards[0].Id);

        }

        [Test]
        public void NavigatesToAboutPage()
        {
            var settings = new Mock<ISettingsProvider>();
            var navigation = new Mock<INavigationService>();
            var pageName = string.Empty;
            navigation.Setup(x => x.Navigate(It.IsAny<string>(), It.IsAny<string>()))
                      .Callback((string s, string q) => pageName = s);

            var cardProvider = new Mock<BaseCardProvider>();
            var target = new MainViewModel(navigation.Object, cardProvider.Object, settings.Object);
            target.AboutCommand.Execute(null);
            navigation.Verify(x => x.Navigate(It.IsAny<string>(), It.IsAny<string>()), Times.Once());
            Assert.AreEqual(Pages.About, pageName);


        }

        private Card GetCard()
        {
            var c = new Card
                        {
                            Id = 1,
                            Name = "name",
                            Description = "desc",
                            Pins = new List<string>()
                        };
            for (int i = 0; i < 20; i++)
            {
                c.Pins.Add(i.ToString(CultureInfo.InvariantCulture));
            }
            return c;
        }
    }
}
