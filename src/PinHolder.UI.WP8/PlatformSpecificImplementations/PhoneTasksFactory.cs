using Microsoft.Phone.Tasks;
using PinHolder.PlatformAbstractions;

namespace PinHolder.PlatformSpecificImplementations
{
    public class PhoneMarketplaceTaskWrapper : IPlatformTask
    {
        private readonly MarketplaceReviewTask _internalTask;
        public PhoneMarketplaceTaskWrapper()
        {
            _internalTask = new MarketplaceReviewTask();
        }

        public void Show()
        {
            _internalTask.Show();
        }
    }

    public class EmailTaskWraper : IPlatformTask
    {
        private readonly EmailComposeTask _emailComposeTask;

        public EmailTaskWraper(string eMail, string subject)
        {
            _emailComposeTask = new EmailComposeTask
            {
                To = eMail,
                Subject = subject
            };
        }
        public void Show()
        {
            _emailComposeTask.Show();
        }
    }

    public class PhoneTaskFactory : IPlatformTaskFactory
    {
        public IPlatformTask GetRateTask()
        {
            return new PhoneMarketplaceTaskWrapper();
        }

        public IPlatformTask GetEmailTask(string eMail, string subject)
        {
            return new EmailTaskWraper(eMail, subject);
        }
    }
}
