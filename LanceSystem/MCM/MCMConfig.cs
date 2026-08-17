namespace LanceSystem.MCM
{
    public class LanceSettings
    {
        private static ICustomSettingsProvider? _provider;

        public static ICustomSettingsProvider Instance
        {
            get
            {
                return _provider ??= CustomSettingsBootstrap.CreateProvider();
            }
        }

        internal static void SetProvider(ICustomSettingsProvider provider)
        {
            _provider = provider;
        }
    }
}
