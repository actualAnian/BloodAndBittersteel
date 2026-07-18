using System;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SceneInformationPopupTypes;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Localization;
using static BloodAndBittersteel.Features.BaBEvents.BaBEventTypes;

using RebellionConfig = BloodAndBittersteel.Features.BlackfyreRebellion.RebellionConfig;

namespace BloodAndBittersteel.Features.BaBEvents.SceneEvents.Events
{
    public class DaemonBecomesKingEvent
    {
        public const string StringId = "daemon_becomes_king";
        public const string ImageStringId = "test";
        public static readonly TextObject TitleText = new("{=bab_daemon_king_title}Victor of the Rebellion");
        public static readonly TextObject Description = new(
            "{=bab_daemon_king_desc}" +
            "The banners of the Black Dragon have triumphed upon the field, as {REBELLION_LEADER} has shattered the armies raised in defense of King Daeron. " +
            "With the loyalist hosts scattered and the cause of the Iron Throne thrown into doubt, Daemon now claims the mantle of King of the Andals, the Rhoynar, and the First Men. " +
            "Yet even a string of victories does not end a war of brothers, though those who once swore fealty to {CROWNLANDS_KING} must now choose whether to kneel before the new king or continue the fight against the dragon that has risen.");

        private class DaemonBecomesKingSceneNotificationData : BecomeKingSceneNotificationItem
        {
            public DaemonBecomesKingSceneNotificationData(Hero newLeaderHero) : base(newLeaderHero) { }
            public override TextObject TitleText => new("{=bab_daemon_king_title}Victor of the Rebellion");
        }

        private static readonly Random _random = new();
        public static bool Condition()
        {
            var kingsLanding = Settlement.All.FirstOrDefault(s => s.StringId == "town_EN1");
            var ownerClan = kingsLanding.OwnerClan;
            return ownerClan.StringId == RebellionConfig.RebellionFactionStringId;
        }
        public static void Consequence()
        {
            var kingdomToRename = Kingdom.All.FirstOrDefault(k => k.StringId == RebellionConfig.RebellionFactionStringId);
            if (kingdomToRename == null)
                return;

            kingdomToRename.ChangeKingdomName(
                new TextObject("{=bab_new_crownlands_name}Realm of the Blackfyre"),
                new TextObject("{=bab_new_crownlands_informal}Realm of the Blackfyre"));
        }
        [BaBEvent]
        private static BaBSceneEvent CreateEvent()
        {
            var daemonClan = Clan.FindFirst(c => c.StringId == RebellionConfig.RebellionLeader);
            var hero = daemonClan?.Leader ?? Hero.MainHero;
            var notificationData = new DaemonBecomesKingSceneNotificationData(hero);

            return new(
                StringId,
                OnDailyTick,
                notificationData,
                CampaignTime.Never,
                Condition,
                Consequence);
        }

    }
}
