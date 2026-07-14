namespace BloodAndBittersteel.Features.BaBEvents
{
    public interface IBaBEvent
    {
        string StringId { get; }
        BaBEventTypes EventType { get; }
        float Chance { get; }
        bool CheckCondition();
    }
}
