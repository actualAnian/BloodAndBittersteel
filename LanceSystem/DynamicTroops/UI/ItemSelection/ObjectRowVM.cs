using TaleWorlds.Library;
using LanceSystem.DynamicTroops.UI.ItemSelection;

namespace LanceSystem.DynamicTroops.TroopCreation.ItemSelection
{
    public class ObjectRowVM : ViewModel
    {
        MBBindingList<ObjectCardVM> _cards = new();
        [DataSourceProperty] public MBBindingList<ObjectCardVM> Cards { get => _cards; set { if (value != _cards) { _cards = value; OnPropertyChangedWithValue(value, "Cards"); } } }
        public ObjectRowVM(MBBindingList<ObjectCardVM> cards) { Cards = cards; }
    }
}
