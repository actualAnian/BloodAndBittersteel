using BloodAndBittersteel.MCM;
using MCM.Abstractions.Base.Global;

namespace BaBMCMIntegration
{
    public partial class BaBMCMSettings : IMCMSettingsIntegration
    {
        private bool _initialized;

        public bool IsAvailable { get => GlobalSettings<CustomSettings>.Instance != null; }
        public void TryInitialize()
        {
            if (_initialized)
                return;
            var _ = GlobalSettings<CustomSettings>.Instance;
            _initialized = true;
        }
        public ICustomSettingsProvider? GetSettings()
        {
            if (!IsAvailable)
                return null;
            return GlobalSettings<CustomSettings>.Instance;
        }
    }
}