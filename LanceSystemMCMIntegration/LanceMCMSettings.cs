using LanceSystem.MCM;
using MCM.Abstractions.Base.Global;

namespace LanceSystemMCMIntegration
{
    public partial class LanceMCMSettings : IMCMSettingsIntegration
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