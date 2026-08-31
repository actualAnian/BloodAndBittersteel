using LanceSystem.Deserialization;

namespace LanceSystem.DynamicLances
{
    public interface IDynamicLancesFactory
    {
        Lance CreateLanceFromData(string name, string? cultureId, string? clanId, LanceTemplateOriginType originType, LanceTroopsTemplate troopsTemplate, int weight, string? bannerKey);
        void UpdateLanceFromData(string stringId, string name, string? cultureId, string? clanId, LanceTemplateOriginType originType, LanceTroopsTemplate troopsTemplate, int weight, string? bannerKey);
    }
}
