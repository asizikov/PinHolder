using NUnit.Framework;
using Moq;
using PinHolder.Model;
using PinHolder.PlatformAbstractions;
using PinHolder.ViewModel;

namespace PinHolder.Tests
{
    [TestFixture]
    public class LockerViewModelTestFixture
    {
        private  ApplicationSettings _settings;

        private Mock<ISettingsLoader> _settingsLoader;
        private ApplicationSettingsProvider _settingsProvider;

        [TestFixtureSetUp]
        public void SetUp()
        {
            _settings = new ApplicationSettings
            {
                Password = "1234",
                AskPassword = true
            };

           _settingsLoader = new Mock<ISettingsLoader>();
            _settingsLoader.Setup(s => s.GetSettings())
                         .Returns(_settings);
            _settingsProvider = new ApplicationSettingsProvider(_settingsLoader.Object);

        }

        [Test]
        public void DoesNotShowLockerAfterActivationIfPasswordIsDisabled()
        {
            var settingsLoader = new Mock<ISettingsLoader>();
            settingsLoader.Setup(s => s.GetSettings())
                         .Returns(new ApplicationSettings{Password = "12345", AskPassword = false});
            var settingsProvider = new ApplicationSettingsProvider(settingsLoader.Object);
            var target = new LockerViewModel(settingsProvider);
            target.Activate(null);

            Assert.IsFalse(target.ShowLocker);
        }


        [Test]
        public void ShowsLockerAfterActivation()
        {

            var target = new LockerViewModel(_settingsProvider);
            target.Activate(null);

            Assert.IsTrue(target.ShowLocker);
        }

        [Test]
        public void HidesLockerAfterPasswordAccepted()
        {
            var target = new LockerViewModel(_settingsProvider);
            target.Activate(null);

            target.PasswordAccepted = true;
            Assert.IsFalse(target.ShowLocker);
        }

        [Test]
        public void ExecutesCallbackAfterPasswordAccepted()
        {
            var target = new LockerViewModel(_settingsProvider);
            var executed = false;

            target.Activate(() => { executed = true; });
            target.PasswordAccepted = true;

            Assert.IsTrue(executed);
        }

        [Test]
        public void ReadsPasswordFromSettings()
        {
            var target = new LockerViewModel(_settingsProvider);

            Assert.AreEqual(_settings.Password, target.StoredPassword);

        }
    }
}
