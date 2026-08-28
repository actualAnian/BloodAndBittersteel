using System.Collections.Generic;
namespace LanceSystem.DynamicTroops.UI.ItemSelection.Filters
{
    public interface IDataFilter<T>
    {
        IList<T> GetFilteredItems(IList<T> data);
    }
}