using System.Collections.Generic;
namespace LanceSystem.UI.ItemSelection.Filters
{
    public interface IDataFilter<T>
    {
        IEnumerable<T> GetFilteredItems(IEnumerable<T> data);
    }
}