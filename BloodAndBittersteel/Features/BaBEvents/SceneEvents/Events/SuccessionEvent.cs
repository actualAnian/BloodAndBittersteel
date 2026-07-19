using BloodAndBittersteel.Features.BlackfyreRebellion;
using System;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.SceneInformationPopupTypes;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace BloodAndBittersteel.Features.BaBEvents.SceneEvents.Events
{
    public class SuccessionEvent
    {
        private const string StringId = "succession";
        public static readonly TextObject Description = new("{=bab_succession_desc} Succession. The rebellion has not yet failed! {NEW_LEADER} rises to lead the cause.");

        private class SuccessionSceneNotificationData : BecomeKingSceneNotificationItem
        {
            public SuccessionSceneNotificationData(Hero newLeaderHero) : base(newLeaderHero) { }
            public override TextObject TitleText 
            { 
                get
                { 
                    GameTexts.SetVariable("NEW_LEADER", Kingdom.All.FirstOrDefault(k => k.StringId == RebellionConfig.RebellionFactionStringId).Leader.Name);
                    return Description;
                }
            }
        }

        private static readonly Random _random = new();
        private static string[] SuccessionChain() => new[] { "BLACKFYRE_m_01", "BLACKFYRE_m_02", "BLACKFYRE_m_03", "BLACKFYRE_m_04" };
        private static Hero? GetNextSuccessionHeir()
        {
            var behavior = RebellionCampaignBehavior.Instance;
            var chain = SuccessionChain();
            var currentLeaderStringId = behavior.RebellionData.RebellionLeader;
            if (!string.IsNullOrEmpty(currentLeaderStringId))
            {
                var currentIndex = Array.IndexOf(chain, currentLeaderStringId);
                if (currentIndex >= 0 && currentIndex < chain.Length - 1)
                {
                    var nextInChain = Hero.Find(chain[currentIndex + 1]);
                    if (nextInChain != null && !nextInChain.IsDead)
                        return nextInChain;
                }
            }

            var blackfyreClan = Clan.FindFirst(c => c.StringId == RebellionConfig.RebellionLeader);
            if (blackfyreClan != null)
            {
                var adultMales = (from h in Hero.AllAliveHeroes where h.Clan?.StringId == "Blackfyre_1" && !h.IsFemale && !h.IsChild select h).ToList();
                if (adultMales.Any())
                    return adultMales[_random.Next(adultMales.Count)];
            }

            var rebellionKingdom = Kingdom.All.FirstOrDefault(k => k.StringId == RebellionConfig.RebellionFactionStringId);
            if (rebellionKingdom != null)
            {
                var clanHeads = (from c in rebellionKingdom.Clans let h = c.Leader where h != null && !h.IsFemale && !h.IsDead && !h.IsChild select h).ToList();
                if (clanHeads.Any())
                    return clanHeads[_random.Next(clanHeads.Count)];
            }
            return null;
        }

        public static bool Condition()
        {
            var behavior = RebellionCampaignBehavior.Instance;
            if (!behavior.RebellionData.IsRebellionActive)
                return false;

            var currentLeaderStringId = behavior.RebellionData.RebellionLeader;
            if (string.IsNullOrEmpty(currentLeaderStringId))
                return true;

            var currentLeader = Hero.Find(currentLeaderStringId);
            if (currentLeader != null && !currentLeader.IsDead)
                return false;
            return true;
        }

        public static void Consequence()
        {
            var behavior = RebellionCampaignBehavior.Instance;
            var oldLeaderStringId = behavior.RebellionData.RebellionLeader;
            var newHeir = GetNextSuccessionHeir();
            if (newHeir == null)
                return;

            behavior.RebellionData.RebellionLeader = newHeir.StringId;
            ChangeClanLeaderAction.ApplyWithSelectedNewLeader(newHeir.Clan, newHeir);
            var kingdom = Kingdom.All.FirstOrDefault(k => k.StringId == RebellionConfig.RebellionFactionStringId);
            kingdom.RulingClan = newHeir.Clan;
        }
        [BaBEvent]
        private static BaBSceneEvent CreateEvent()
        {
            var behavior = RebellionCampaignBehavior.Instance;
            var currentLeaderStringId = behavior.RebellionData.RebellionLeader;
            Hero? hero;

            if (string.IsNullOrEmpty(currentLeaderStringId))
            {
                hero = Hero.MainHero;
            }
            else
            {
                var currentLeader = Hero.Find(currentLeaderStringId);
                if (currentLeader != null && !currentLeader.IsDead)
                    hero = currentLeader;
                else
                    hero = GetNextSuccessionHeir();
            }
            var notificationData = new SuccessionSceneNotificationData(hero);
            return new(StringId, BaBEventTypes.OnDailyTick, notificationData, CampaignTime.Never, Condition, Consequence);
        }
    }
}
