using System;
using TaleWorlds.Core;
using TaleWorlds.Library;
namespace LanceSystem.DynamicTroops.TroopCreation
{
    public class ItemSlotVM : ViewModel
    {
        readonly string _slotKey;
        readonly Action<string> _onSelect;
        ItemObject _item;
        string _type = "";
        bool _hasItem;
        string _displayName = "";
        public ItemSlotVM(string slotKey, string label, ItemObject item, Action<string> onSelect)
        {
            _slotKey = slotKey;
            _onSelect = onSelect;
            UpdateFrom(item, label);
        }
        public void UpdateFrom(ItemObject item, string label)
        {
            _item = item;
            HasItem = item != null;
            Type = item != null ? item.Type.ToString() : ItemObject.ItemTypeEnum.Invalid.ToString();
            DisplayName = label + " : " + (item?.Name?.ToString() ?? "None");
        }
        [DataSourceProperty] public string Type { get => _type; set { if (value != _type) { _type = value; OnPropertyChangedWithValue(value, "Type"); } } }
        [DataSourceProperty] public bool HasItem { get => _hasItem; set { if (value != _hasItem) { _hasItem = value; OnPropertyChangedWithValue(value, "HasItem"); } } }
        [DataSourceProperty] public string DisplayName { get => _displayName; set { if (value != _displayName) { _displayName = value; OnPropertyChangedWithValue(value, "DisplayName"); } } }
        public void ExecuteChange() => _onSelect?.Invoke(_slotKey);
        public void ExecuteBeginHint() { if (_item != null) InformationManager.ShowTooltip(typeof(ItemObject), new EquipmentElement(_item)); }
        public void ExecuteEndHint() => MBInformationManager.HideInformations();
    }
}
