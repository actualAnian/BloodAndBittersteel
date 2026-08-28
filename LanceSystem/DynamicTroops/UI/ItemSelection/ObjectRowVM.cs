using TaleWorlds.Library;

namespace LanceSystem.DynamicTroops.UI.ItemSelection
{
    public class ObjectRowVM : ViewModel
    {
        MBBindingList<CardVM> _cards = new();
        [DataSourceProperty] public MBBindingList<CardVM> Cards { get => _cards; set { if (value != _cards) { _cards = value; OnPropertyChangedWithValue(value, "Cards"); } } }
        public ObjectRowVM(MBBindingList<CardVM> cards) { Cards = cards; }
    }
}
