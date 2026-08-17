namespace BloodAndBittersteel.MCM
{
    public class BaBSettings
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