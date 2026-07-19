using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameState;

namespace BloodAndBittersteel.Features.BaBEvents
{
    public abstract class BaBImmediateEvent : IBaBEvent
    {
        public string StringId { get; }
        public BaBEventTypes EventType { get; }
        public float Chance { get; }
        public Action OnFire { get; }
        public Func<bool> ConditionFunc { get; }
        public CampaignTime Cooldown { get; private set; }

        protected BaBImmediateEvent(string stringId, BaBEventTypes eventType, float chance, Func<bool> condition, Action onFire, CampaignTime cooldown)
        {
            StringId = stringId;
            EventType = eventType;
            Chance = chance;
            OnFire = onFire;
            ConditionFunc = condition;
            Cooldown = cooldown;
        }
        public bool Condition() => ConditionFunc.Invoke();

        public void Fire()
        {
            OnFire?.Invoke();
            FireInternal();
        }

        protected abstract void FireInternal();
    }
}
