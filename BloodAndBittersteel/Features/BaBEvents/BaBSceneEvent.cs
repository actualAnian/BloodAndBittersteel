using System;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.Core;

namespace BloodAndBittersteel.Features.BaBEvents
{
    public class BaBSceneEvent : BaBImmediateEvent
    {
        public SceneNotificationData NotificationData { get; }

        private Func<bool> _condition;

        public BaBSceneEvent(
            string stringId,
            BaBEventTypes eventType,
            SceneNotificationData notificationData,
            Action onFire,
            Func<bool>? condition = null,
            float chance = 1f)
            : base(stringId, eventType, chance, onFire)
        {
            NotificationData = notificationData;
            _condition = condition ?? (() => true);
        }

        public override bool CheckCondition() => _condition();

        protected override void FireInternal(MapState mapState)
        {
            MBInformationManager.ShowSceneNotification(NotificationData);
        }
    }
}
