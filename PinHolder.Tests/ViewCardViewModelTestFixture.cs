using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using PinHolder.Model;
using PinHolder.Navigation;
using PinHolder.ViewModel;
using Moq;

namespace PinHolder.Tests
{
    [TestFixture]
    public class ViewCardViewModelTestFixture
    {
        [Test, Ignore]
        public void Test()
        {
            const int id = 1;
            var navigation = new Mock<INavigationService>();
            var cardProvider = new Mock<ICardProvider>();
//            var target = new ViewCardViewModel(navigation.Object, cardProvider.Object, id);


        }

    }
}
