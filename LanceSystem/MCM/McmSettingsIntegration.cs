namespace LanceSystem.MCM
{
    public interface IMCMSettingsIntegration
    {
        void TryInitialize();
        ICustomSettingsProvider? GetSettings();
    }
}
