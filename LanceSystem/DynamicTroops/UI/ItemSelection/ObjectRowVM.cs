using TaleWorlds.Library;

namespace LanceSystem.DynamicTroops.UI.ItemSelection
{
    public class ObjectRowVM : ViewModel
    {
        MBBindingList<ObjectCardVM> _cards = new();
        [DataSourceProperty] public MBBindingList<ObjectCardVM> Cards { get => _cards; set { if (value != _cards) { _cards = value; OnPropertyChangedWithValue(value, "Cards"); } } }
        public ObjectRowVM(MBBindingList<ObjectCardVM> cards) { Cards = cards; }
    }
}
