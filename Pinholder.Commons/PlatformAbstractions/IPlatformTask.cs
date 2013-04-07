namespace Pinholder.PlatformAbstractions
{
    public interface IPlatformTask
    {
        void Show();
    }

    public interface IPlatformTaskFactory
    {
        IPlatformTask GetRateTask();
        IPlatformTask GetEmailTask(string eMail, string subject);
    }
}
