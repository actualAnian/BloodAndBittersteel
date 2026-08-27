using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.ImageIdentifiers;
using TaleWorlds.Library;
namespace LanceSystem.DynamicTroops.TroopCreation
{
    public class UpgradeSlotVM : ViewModel
    {
        readonly CharacterObject _upgrade;
        readonly Action<CharacterObject> _onLink;
        readonly Action _onAdd;
        readonly Action<CharacterObject> _onRemove;
        ImageIdentifierVM _imageIdentifier;
        string _buttonText = "Add";
        public UpgradeSlotVM(CharacterObject upgrade, Action<CharacterObject> onLink, Action onAdd, Action<CharacterObject> onRemove)
        {
            _upgrade = upgrade;
            _onLink = onLink;
            _onAdd = onAdd;
            _onRemove = onRemove;
            _imageIdentifier = upgrade != null ? new CharacterImageIdentifierVM(CharacterCode.CreateFrom(upgrade)) : new ItemImageIdentifierVM(null);
            _buttonText = upgrade != null ? "Remove" : "Add";
        }
        [DataSourceProperty] public ImageIdentifierVM ImageIdentifier { get => _imageIdentifier; set { if (value != _imageIdentifier) { _imageIdentifier = value; OnPropertyChangedWithValue(value, "ImageIdentifier"); } } }
        [DataSourceProperty] public string ButtonText { get => _buttonText; set { if (value != _buttonText) { _buttonText = value; OnPropertyChangedWithValue(value, "ButtonText"); } } }
        public void ExecuteLink()
        {
            if (_upgrade == null) return;
            _onLink?.Invoke(_upgrade);
        }
        public void ExecuteRemove()
        {
            if (_upgrade == null) _onAdd?.Invoke();
            else _onRemove?.Invoke(_upgrade);
        }
        public void ExecuteBeginHint()
        {
            if (_upgrade != null) InformationManager.ShowTooltip(typeof(CharacterObject), new object[] { _upgrade });
        }
        public void ExecuteEndHint() => MBInformationManager.HideInformations();
    }
}
