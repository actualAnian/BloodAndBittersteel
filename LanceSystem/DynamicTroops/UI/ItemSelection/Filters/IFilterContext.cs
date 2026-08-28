using System;

namespace LanceSystem.DynamicTroops.UI.ItemSelection.Filters
{
    public interface IFilterContext
    {
        string Title { get; }
        string DisplayText { get; }
        void OnEventClicked();
        void Reset();
        event Action? OnChanged;
        event Action? OnReset;
    }
}
