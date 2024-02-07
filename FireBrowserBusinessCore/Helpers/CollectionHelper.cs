using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace FireBrowserBusinessCore.Helpers
{
    public static class CollectionHelper
    {
        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> enumerableList)
        {

            return enumerableList != null ? new ObservableCollection<T>(enumerableList) : null;

        }
    }
}
