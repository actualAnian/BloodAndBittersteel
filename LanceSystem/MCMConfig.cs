using MCM.Abstractions.Attributes;
using MCM.Abstractions.Base.Global;
using TaleWorlds.Library;

namespace LanceSystem
{
    public class LanceSettings
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
                    string text = "Lance system, no MCM module found, using default settings";
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
    }
    public class HardcodedCustomSettings : ICustomSettingsProvider
    {
        public bool FemalePrejudice { get; set; } = true;
    }
    public class CustomSettings : AttributeGlobalSettings<CustomSettings>, ICustomSettingsProvider
    {
        public override string DisplayName => "Lance system settings";
        public override string Id => "lances";
        public bool FemalePrejudice { get; set; } = true;
    }
}