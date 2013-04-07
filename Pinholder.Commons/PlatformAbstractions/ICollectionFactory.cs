using System.Collections.Generic;

namespace Pinholder.PlatformAbstractions
{
    public interface ICollectionFactory
    {
        IList<T> GetCollection<T>();
    }
}