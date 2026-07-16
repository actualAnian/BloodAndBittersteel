using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SceneInformationPopupTypes;
using TaleWorlds.Localization;

namespace BloodAndBittersteel.Features.BaBEvents.SceneEvents
{
    public class DaemonBecomesKingSceneNotificationData : BecomeKingSceneNotificationItem
    {
        public DaemonBecomesKingSceneNotificationData(Hero newLeaderHero) : base(newLeaderHero) { }

        public override TextObject TitleText => new("my test");
        public override TextObject AffirmativeText => new("I replaced click to continue, with an epic text, I am sure noone will notice that this was originally meant to just say, click to exit, and will be immersed in the setting and cool epic scene");
        public override TextObject AffirmativeDescriptionText => new("I replaced click to continue, with an epic description, I am sure noone will notice that this was originally meant to just say, click to exit, and will be immersed in the setting and cool epic scene");
    }

    public class BaBDefaultSceneEventProvider : IEventProvider
    {
        public IEnumerable<IBaBEvent> InitializeEvents()
        {
            yield return new BaBSceneEvent(
                "daemon_becomes_king",
                BaBEventTypes.OnDailyTick,
                new DaemonBecomesKingSceneNotificationData(Hero.MainHero),
                CampaignTime.Never,
                () => { },
                () => { return true; }
            );
        }
    }
}
