using TaleWorlds.InputSystem;

namespace BloodAndBittersteel.MCM
{
    public interface ICustomSettingsProvider
    {
        bool FemalePrejudice { get; set; }
        InputKey HelmetTilting { get; set; }
        public bool ShowJoustingPopUp { get; set; }

    }
}