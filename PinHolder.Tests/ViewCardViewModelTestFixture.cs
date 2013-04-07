using NUnit.Framework;
using PinHolder.Lifecycle;
using PinHolder.Model;
using PinHolder.Navigation;
using PinHolder.ViewModel;
using Moq;

namespace PinHolder.Tests
{
    [TestFixture]
    public class ViewCardViewModelTestFixture
    {
        [Test]
        public void EnsuresThatTileDoesNotExistsBeforeCreateTile()
        {
            const int id = 1;
            var navigation = new Mock<INavigationService>();
            var cardProvider = new Mock<BaseCardProvider>();
            var tile = new Mock<ISecondaryTileService>();

            tile.Setup(t => t.CanCreate(id))
                .Returns(false);

            var target = new ViewCardViewModel(navigation.Object, cardProvider.Object, tile.Object, id);

            Assert.False(target.CreatePinCommand.CanExecute(null));
        }

    }
}
