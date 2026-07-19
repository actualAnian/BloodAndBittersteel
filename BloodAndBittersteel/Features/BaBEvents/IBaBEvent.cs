using TaleWorlds.CampaignSystem;

namespace BloodAndBittersteel.Features.BaBEvents
{
    public interface IBaBEvent
    {
        string StringId { get; }
        BaBEventTypes EventType { get; }
        float Chance { get; }
        bool Condition();
        CampaignTime Cooldown { get; }
    }
}