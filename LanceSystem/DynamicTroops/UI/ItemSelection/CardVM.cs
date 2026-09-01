using System;
using TaleWorlds.CampaignSystem.ViewModelCollection.Inventory;
using TaleWorlds.Core.ViewModelCollection.ImageIdentifiers;
using TaleWorlds.Library;

namespace LanceSystem.DynamicTroops.UI.ItemSelection
{
    public abstract class CardVM : ViewModel
    {
        readonly Action _apply;
        readonly Action? _close;
        string _cardName = "";
        MBBindingList<ItemFlagVM> _itemFlagList = new();
        MBBindingList<ItemMenuTooltipPropertyVM> _objectProperties = new();
        ImageIdentifierVM _image;

        protected CardVM(Action apply, Action? close)
        {
            _apply = apply;
            _close = close;
            ItemFlagList = new MBBindingList<ItemFlagVM>();
            ObjectProperties = new MBBindingList<ItemMenuTooltipPropertyVM>();
        }

        [DataSourceProperty]
        public string CardName
        {
            get => _cardName;
            set
            {
                if (value == _cardName) return;
                _cardName = value;
                OnPropertyChanged("CardName");
            }
        }

        [DataSourceProperty]
        public ImageIdentifierVM Image
        {
            get => _image;
            set
            {
                if (value == _image) return;
                _image = value;
                OnPropertyChangedWithValue(value, "Image");
            }
        }

        [DataSourceProperty]
        public MBBindingList<ItemFlagVM> ItemFlagList
        {
            get => _itemFlagList;
            set
            {
                if (value == _itemFlagList) return;
                _itemFlagList = value;
                OnPropertyChangedWithValue(value, "ItemFlagList");
            }
        }

        [DataSourceProperty]
        public MBBindingList<ItemMenuTooltipPropertyVM> ObjectProperties
        {
            get => _objectProperties;
            set
            {
                if (value == _objectProperties) return;
                _objectProperties = value;
                OnPropertyChangedWithValue(value, "ObjectProperties");
            }
        }
        public void Apply()
        {
            _apply?.Invoke();
            _close?.Invoke();
        }
    }
}
