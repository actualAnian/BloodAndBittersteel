using System;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
namespace LanceSystem.UI.TroopEditor
{
    public class ItemSlotVM : ViewModel
    {
        static readonly TextObject _noneText = new("{=lance_none}None");
        static readonly TextObject _separatorText = new("{=lance_separator} : ");
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
            _item = item;
            UpdateFrom(item, label);
        }
        public void UpdateFrom(ItemObject item, string label)
        {
            _item = item;
            HasItem = item != null;
            Type = item != null ? item.Type.ToString() : ItemObject.ItemTypeEnum.Invalid.ToString();
            string itemName = item?.Name?.ToString() ?? _noneText.ToString();
            if (string.IsNullOrEmpty(label))
            {
                DisplayName = itemName;
            }
            else
            {
                MBTextManager.SetTextVariable("LABEL", label);
                MBTextManager.SetTextVariable("ITEM_NAME", itemName);
                DisplayName = new TextObject("{=lance_slot_display}{LABEL}{SEPARATOR}{ITEM_NAME}")
                    .SetTextVariable("SEPARATOR", _separatorText).ToString();
            }
        }
        
        [DataSourceProperty] public string EditButtonText => UITexts.Edit.ToString();
        [DataSourceProperty] public string Type { get => _type; set { if (value != _type) { _type = value; OnPropertyChangedWithValue(value, "Type"); } } }
        [DataSourceProperty] public bool HasItem { get => _hasItem; set { if (value != _hasItem) { _hasItem = value; OnPropertyChangedWithValue(value, "HasItem"); } } }
        [DataSourceProperty] public string DisplayName { get => _displayName; set { if (value != _displayName) { _displayName = value; OnPropertyChangedWithValue(value, "DisplayName"); } } }
        public void ExecuteChange() => _onSelect?.Invoke(_slotKey);
        public void ExecuteBeginHint() { if (_item != null) InformationManager.ShowTooltip(typeof(ItemObject), new EquipmentElement(_item)); }
        public void ExecuteEndHint() => MBInformationManager.HideInformations();
    }
}
