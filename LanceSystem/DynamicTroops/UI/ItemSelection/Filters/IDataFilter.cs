using System.Collections.Generic;
namespace LanceSystem.DynamicTroops.TroopCreation.ItemSelection.Filters
{
    public interface IDataFilter<T>
    {
        IList<T> Filter(IList<T> data);
    }
}