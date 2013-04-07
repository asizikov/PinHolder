using System.Collections.Generic;
using System.Collections.ObjectModel;
using PinHolder.PlatformAbstractions;

namespace PinHolder.PlatformSpecificFactories
{
    public class CollectionFactory : ICollectionFactory
    {
        public IList<T> GetCollection<T>()
        {
            return new ObservableCollection<T>();
        }
    }
}