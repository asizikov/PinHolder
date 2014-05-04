using System.Collections.Generic;

namespace PinHolder.PlatformAbstractions
{
    public interface ICollectionFactory
    {
        IList<T> GetCollection<T>();
    }
}