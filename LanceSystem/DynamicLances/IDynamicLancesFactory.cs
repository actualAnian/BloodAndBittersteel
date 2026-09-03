using LanceSystem.Deserialization;

namespace LanceSystem.DynamicLances
{
    public interface IDynamicLancesFactory
    {
        Lance SaveLance(string stringId, string name, string? cultureId, string? clanId, LanceTemplateOriginType originType, LanceTroopsTemplate troopsTemplate, int weight, string? bannerKey);
    }
}
