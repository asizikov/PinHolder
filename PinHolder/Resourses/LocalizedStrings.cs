namespace PinHolder.Resourses
{
    public sealed class LocalizedStrings
    {
        public LocalizedStrings()
        {
        }

        private static Strings localizedResources = new Strings();

        public Strings Strings
        {
            get { return localizedResources; }
        }
    }
}
