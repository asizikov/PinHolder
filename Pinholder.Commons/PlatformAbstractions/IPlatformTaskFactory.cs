namespace PinHolder.PlatformAbstractions
{
    public interface IPlatformTaskFactory
    {
        IPlatformTask GetRateTask();
        IPlatformTask GetEmailTask(string eMail, string subject);
    }
}