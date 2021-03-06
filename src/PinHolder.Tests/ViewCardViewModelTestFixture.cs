﻿using NUnit.Framework;
using PinHolder.Lifecycle;
using PinHolder.Model;
using PinHolder.Navigation;
using PinHolder.PlatformAbstractions;
using PinHolder.ViewModel;
using Moq;

namespace PinHolder.Tests
{
    [TestFixture]
    public class ViewCardViewModelTestFixture
    {
        private Mock<INavigationService> _navigation;
        private Mock<BaseCardProvider> _cardProvider;
        private Mock<ISecondaryTileService> _tile;
        private Mock<StatisticsService> _statistics;
        private ApplicationSettingsProvider _applicationSettingsProvider;
        private const int ID = 1;

        [TestFixtureSetUp]
        public void SetUp()
        {
            _navigation = new Mock<INavigationService>();
            _cardProvider = new Mock<BaseCardProvider>();
            _tile = new Mock<ISecondaryTileService>();
            _statistics = new Mock<StatisticsService>();
            var settingsLoader = new Mock<ISettingsLoader>();
            settingsLoader.Setup(s => s.GetSettings())
                .Returns(new ApplicationSettings {Password = "1234", AskPassword = true});
            _applicationSettingsProvider = new ApplicationSettingsProvider(settingsLoader.Object);
        }

        [Test]
        public void EnsuresThatTileDoesNotExistsBeforeCreateTile()
        {
            _tile.Setup(t => t.CanCreate(ID))
                .Returns(false);

            var target = new ViewCardViewModel(_navigation.Object, _cardProvider.Object, _tile.Object, 
                _statistics.Object, From.MainPage, ID);

            Assert.False(target.CreatePinCommand.CanExecute(null));
        }


        [Test]
        public void CantPinEmptyCard()
        {
            _cardProvider.Setup(c => c.GetById(It.IsAny<int>()))
                .Returns<Card>(null);

            var target = new ViewCardViewModel(_navigation.Object, _cardProvider.Object, _tile.Object, 
                _statistics.Object, From.MainPage, ID);

            Assert.IsFalse(target.CreatePinCommand.CanExecute(null));
        }

        [Test]
        public void CantEditEmptyCard()
        {
            _cardProvider.Setup(c => c.GetById(It.IsAny<int>()))
                .Returns<Card>(null);

            var target = new ViewCardViewModel(_navigation.Object, _cardProvider.Object, _tile.Object,
                _statistics.Object, From.MainPage, ID);

            Assert.IsFalse(target.EditCommand.CanExecute(null));
        }
    }
}