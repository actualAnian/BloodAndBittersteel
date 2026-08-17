using TaleWorlds.InputSystem;

namespace BloodAndBittersteel.MCM
{
    public class HardcodedCustomSettings : ICustomSettingsProvider
    {
        public bool FemalePrejudice { get; set; } = true;
        public InputKey HelmetTilting { get; set; } = InputKey.H;
        // public Dropdown<InputKey> HelmetTilting { get; set; } = new Dropdown<InputKey>(new InputKey[] { InputKey.H }, selectedIndex: 0);
        public bool ShowJoustingPopUp { get; set; } = true;
    }
}
