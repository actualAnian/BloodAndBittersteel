namespace BloodAndBittersteel.MCM
{
    public interface IMCMSettingsIntegration
    {
        void TryInitialize();
        ICustomSettingsProvider? GetSettings();
    }
}
