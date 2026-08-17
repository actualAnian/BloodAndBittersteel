using System;
using System.Linq;

namespace LanceSystem.MCM
{
    internal static class CustomSettingsBootstrap
    {
        private static bool _mcmInitialized;
        private static IMCMSettingsIntegration? _mcmSettingsIntegration;

        public static void Initialize()
        {
            if (_mcmInitialized)
                return;

            if (!IsMcmAvailable())
                return;

            _mcmSettingsIntegration = CreateMcmIntegration();
            if (_mcmSettingsIntegration == null) return;
            _mcmSettingsIntegration.TryInitialize();
            _mcmInitialized = true;
        }
        public static ICustomSettingsProvider CreateProvider()
        {
            if (_mcmInitialized)
            {
                var settings = _mcmSettingsIntegration!.GetSettings();
                if (settings != null)
                    return settings;
            }

            return new HardcodedCustomSettings();
        }
        private static IMCMSettingsIntegration? CreateMcmIntegration()
        {
            var assembly = System.Reflection.Assembly.Load("LanceSystemMCMIntegration");
            var type = assembly.GetType("LanceSystemMCMIntegration.LanceMCMSettings");
            return Activator.CreateInstance(type) as IMCMSettingsIntegration;
        }
        public static bool IsMcmAvailable()
        {
            return AppDomain.CurrentDomain
                .GetAssemblies()
                .Any(x => x.GetName().Name == "MCMv5");
        }
    }
}
