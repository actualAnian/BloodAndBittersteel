using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.Core;

namespace BloodAndBittersteel.Features.BaBEvents.SceneEvents
{
    public class BaBSceneEvent : BaBImmediateEvent
    {
        public SceneNotificationData NotificationData { get; }

        private Func<bool> _condition;

        public BaBSceneEvent(
            string stringId,
            BaBEventTypes eventType,
            SceneNotificationData notificationData,
            CampaignTime cooldown,
            Action onFire,
            Func<bool>? condition = null,
            float chance = 1f)
            : base(stringId, eventType, chance, onFire, cooldown)
        {
            NotificationData = notificationData;
            _condition = condition ?? (() => true);
        }

        public override bool CheckCondition() => _condition();

        protected override void FireInternal()
        {
            MBInformationManager.ShowSceneNotification(NotificationData);
        }
    }
}
