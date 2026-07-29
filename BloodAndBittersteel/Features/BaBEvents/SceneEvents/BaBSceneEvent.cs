using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace BloodAndBittersteel.Features.BaBEvents.SceneEvents
{
    public class BaBSceneEvent : BaBImmediateEvent
    {
        public SceneNotificationData NotificationData { get; }

        public BaBSceneEvent(
            string stringId,
            BaBEventTypes eventType,
            SceneNotificationData notificationData,
            CampaignTime cooldown,
            Func<bool> condition,
            Action onFire,
            float chance = 1f)
            : base(stringId, eventType, chance, condition, onFire, cooldown)
        {
            NotificationData = notificationData;
        }

        protected override void FireInternal()
        {
            MBInformationManager.ShowSceneNotification(NotificationData);
        }
    }
}
