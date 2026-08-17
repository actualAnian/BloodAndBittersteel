using LanceSystem.MCM;
using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v2;
using MCM.Abstractions.Base.Global;

namespace LanceSystemMCMIntegration
{
    public partial class LanceMCMSettings
    {
        public class CustomSettings : AttributeGlobalSettings<CustomSettings>, ICustomSettingsProvider
        {
            public override string DisplayName => "Lance system settings";
            public override string Id => "lances";

            [SettingPropertyGroup("General")]

            [SettingPropertyBool("Female prejudice", Order = 1, RequireRestart = false, HintText = "Less chance to recruit volunteers from notables, more stringent rules to recruit a lance as female character.")]
            public bool FemalePrejudice { get; set; } = true;
        }
    }
}