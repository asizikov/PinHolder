using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using PinHolder.Model;
using PinHolder.PlatformAbstractions;

namespace PinHolder.Tests.CommonsTestFixtures
{
    [TestFixture]
    public class ApplicationSettingsProviderTestFixture
    {

        [Test,ExpectedException(typeof(ArgumentNullException))]
        public void DoesNotAcceptNullLoader()
        {
            var target = new ApplicationSettingsProvider(null);
        }

        [Test]
        public void LoadsSettingsFromSource()
        {
            var settings = new ApplicationSettings
                {
                    Password = "1234"
                };
            var settingsLoader = new Mock<ISettingsLoader>();
            settingsLoader.Setup(s => s.GetSettings())
                          .Returns(settings);

            var target = new ApplicationSettingsProvider(settingsLoader.Object);

            settingsLoader.Verify(s => s.GetSettings(),Times.AtLeastOnce());
            Assert.AreEqual(settings.Password, target.Password);
        }
    }
}
