namespace LanceSystem.DynamicTroops.TroopCreation.ItemSelection.Filters
{
    public interface IFilterContext
    {
        string Title { get; }
        string DisplayText { get; }
        void OnEventClicked();
        void Reset();
    }
}
