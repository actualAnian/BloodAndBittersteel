using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v2;
using MCM.Abstractions.Base.Global;
using MCM.Common;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;

namespace BloodAndBittersteel
{
    public class BaBSettings
    {
        static ICustomSettingsProvider? _provider;
        public static ICustomSettingsProvider Instance
        {
            get
            {
                if (_provider != null) return _provider;
                try
                {
                    if (GlobalSettings<CustomSettings>.Instance != null)
                    {
                        _provider = GlobalSettings<CustomSettings>.Instance;
                        return _provider;
                    }
                }
                catch
                {
                    string text = "no MCM module found, using default settings";
                    InformationManager.DisplayMessage(new InformationMessage(text, new Color(0, 0, 0)));
                }
                _provider = new HardcodedCustomSettings();
                return _provider;
            }
        }
    }
    public interface ICustomSettingsProvider
    {
        [SettingPropertyGroup("{=CustomSettings_General}General")]
        bool FemalePrejudice { get; set; }
        Dropdown<InputKey> HelmetTilting { get; set; }
    }
    public class HardcodedCustomSettings : ICustomSettingsProvider
    {
        public bool FemalePrejudice { get; set; } = true;
        public Dropdown<InputKey> HelmetTilting { get; set; } = new Dropdown<InputKey>(new InputKey[] { InputKey.H }, selectedIndex: 0);
    }
    public class CustomSettings : AttributeGlobalSettings<CustomSettings>, ICustomSettingsProvider
    {
        public override string DisplayName => "Blood and Bittersteel settings";
        public override string Id => "blood_and_bittersteel_id";

        [SettingPropertyGroup("General")]

        [SettingPropertyBool("Female prejudice", Order = 1, RequireRestart = false, HintText = "Female lords do not enter tournaments, traits required to enter as female character")]
        public bool FemalePrejudice { get; set; } = true;
        [SettingPropertyBool("Helmet Tilting", Order = 2, RequireRestart = false, HintText = "Button that allows helmet tilting")]
        public Dropdown<InputKey> HelmetTilting { get; set; } = new Dropdown<InputKey>(new InputKey[]
        {
            InputKey.A,
            InputKey.B,
            InputKey.C,
            InputKey.D,
            InputKey.E,
            InputKey.F,
            InputKey.G,
            InputKey.H,
            InputKey.I,
            InputKey.J,
            InputKey.K,
            InputKey.L,
            InputKey.M,
            InputKey.N,
            InputKey.O,
            InputKey.P,
            InputKey.Q,
            InputKey.R,
            InputKey.S,
            InputKey.T,
            InputKey.U,
            InputKey.V,
            InputKey.W,
            InputKey.X,
            InputKey.Y,
            InputKey.Z,
            InputKey.Numpad0,
            InputKey.Numpad1,
            InputKey.Numpad2,
            InputKey.Numpad3,
            InputKey.Numpad4,
            InputKey.Numpad5,
            InputKey.Numpad6,
            InputKey.Numpad7,
            InputKey.Numpad8,
            InputKey.Numpad9,
            InputKey.NumLock,
            InputKey.NumpadSlash,
            InputKey.NumpadMultiply,
            InputKey.NumpadMinus,
            InputKey.NumpadPlus,
            InputKey.F1,
            InputKey.F2,
            InputKey.F3,
            InputKey.F4,
            InputKey.F5,
            InputKey.F6,
            InputKey.F7,
            InputKey.F8,
            InputKey.F9,
            InputKey.F10,
            InputKey.F11,
            InputKey.F12,
        }, selectedIndex: 7);

    }
}