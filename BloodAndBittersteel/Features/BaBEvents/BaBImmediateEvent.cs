using System;
using TaleWorlds.CampaignSystem.GameState;

namespace BloodAndBittersteel.Features.BaBEvents
{
    public abstract class BaBImmediateEvent : IBaBEvent
    {
        public string StringId { get; }
        public BaBEventTypes EventType { get; }
        public float Chance { get; }
        public Action OnFire { get; }

        protected BaBImmediateEvent(string stringId, BaBEventTypes eventType, float chance, Action onFire)
        {
            StringId = stringId;
            EventType = eventType;
            Chance = chance;
            OnFire = onFire;
        }

        public abstract bool CheckCondition();

        public void Fire(MapState mapState)
        {
            OnFire?.Invoke();
            FireInternal(mapState);
        }

        protected abstract void FireInternal(MapState mapState);
    }
}
