using BloodAndBittersteel.Features.BlackfyreRebellion;
using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.SceneInformationPopupTypes;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.SaveSystem;

namespace BloodAndBittersteel.Features.NightsWatch
{
    public class NightsWatchCampaignBehavior : CampaignBehaviorBase
    {
        readonly Random _random = new();
        [SaveableField(1)]
        private Dictionary<string, CampaignTime> _lastRefusalTimes = new();
        public override void RegisterEvents()
        {
            CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, OnSessionLaunched);
            CampaignEvents.MapEventEnded.AddNonSerializedListener(this, OnMapEventEnded);
        }
        private float GetChanceForAIToSendToNightsWatch(Hero ruler, Hero prisoner)
        {
            var chance = NightsWatchConfig.BaseChanceForAIToSendToNightsWatch;
            chance += NightsWatchConfig.ChanceForRulerPerRelationPoint * ruler.GetBaseHeroRelation(prisoner);
            return chance;
        }
        private void OnMapEventEnded(MapEvent mapEvent)
        {
            foreach(var party in mapEvent.InvolvedParties)
            {
                if (party == PartyBase.MainParty) continue;
                var hero = party.LeaderHero;
                if (hero == null) continue;
                var validPrisoners = party.PrisonerHeroes.Where(p => CanAIForceToJoinNightsWatch(hero, p.HeroObject));
                foreach(var validPrisoner  in validPrisoners)
                {
                    if (validPrisoner != null)
                    {
                        if (GetChanceForAIToSendToNightsWatch(hero, validPrisoner.HeroObject) > _random.NextDouble())
                            JoinNightsWatch(hero, validPrisoner.HeroObject);
                    }

                }
            }
        }

        private void OnSessionLaunched(CampaignGameStarter campaignGameStarter)
        {
            AddDialogs(campaignGameStarter);
        }
        bool WillJoinNightsWatch(Hero prisoner)
        {
            float chanceToAccept = GetChanceToJoinNightsWatch(prisoner);
            return _random.NextDouble() < chanceToAccept;
        }
        float GetChanceToJoinNightsWatch(Hero prisoner)
        {
            var chance = NightsWatchConfig.BaseChanceForAIToAcceptPlayerOfferToJoinNightsWatch;
            chance += 0.2f * prisoner.GetTraitLevel(DefaultTraits.Honor);
            return chance;
        }
        public void AddDialogs(CampaignGameStarter starter)
        {
            starter.AddPlayerLine(
                "send_to_nights_watch_start",
                "hero_main_options",
                "send_to_nights_watch_response",
                "{SEND_TO_NIGHTS_WATCH_START}",
                () =>
                {
                    bool isValidDialog = ConditionToStartDialog();
                    if (!isValidDialog) return false;

                    if (Hero.MainHero.GetTraitLevel(DefaultTraits.Honor) < 0)
                        GameTexts.SetVariable("SEND_TO_NIGHTS_WATCH_START", NightsWatchDialogs.SendToNightWatchDevious);
                    else 
                        GameTexts.SetVariable("SEND_TO_NIGHTS_WATCH_START", NightsWatchDialogs.SendToNightWatchNormal);
                    return true;
                },
                null
            );
            starter.AddDialogLine(
                "send_to_nights_watch_response_accept",
                "send_to_nights_watch_response",
                "close_window",
                "{SEND_TO_NIGHTS_WATCH_RESPONSE}",
                () =>
                {
                    var prisoner = Hero.OneToOneConversationHero;
                    if (prisoner == null) return false;
                    if (!WillJoinNightsWatch(prisoner)) return false;
                    string text = "";
                    if (prisoner.GetTraitLevel(DefaultTraits.Honor) < 0)
                        text = NightsWatchDialogs.PrisonerAgreesDevious;
                    else if (prisoner.GetTraitLevel(DefaultTraits.Honor) > 0)
                        text = NightsWatchDialogs.PrisonerAgreesHonourable;
                    else
                        text = NightsWatchDialogs.PrisonerAgreesNormal;
                    GameTexts.SetVariable("SEND_TO_NIGHTS_WATCH_RESPONSE", text);
                    return true;
                },
                () => { JoinNightsWatch(Hero.MainHero, Hero.OneToOneConversationHero); } 
            );

            starter.AddDialogLine(
                "send_to_nights_watch_response_refuse",
                "send_to_nights_watch_response",
                "send_to_nights_watch_after_refuse",
                "{SEND_TO_NIGHTS_WATCH_RESPONSE}",
                () =>
                {
                    var prisoner = Hero.OneToOneConversationHero;
                    string text = "";
                    if (RebellionCampaignBehavior.Instance.RebellionData.PlayerSide == RebellionSide.Rebel
                        && prisoner.GetRelation(Hero.MainHero) < -10
                        && prisoner.GetTraitLevel(DefaultTraits.Honor) > 0)
                            text = NightsWatchDialogs.PrisonerRefusesPlayerRebellionRelationNegativeLordHonourable;
                    else if (RebellionCampaignBehavior.Instance.RebellionData.PlayerSide == RebellionSide.Rebel
                        && prisoner.GetRelation(Hero.MainHero) < -10)
                            text = NightsWatchDialogs.PrisonerRefusesPlayerRebellionRelationNegative;
                    else if (RebellionCampaignBehavior.Instance.RebellionData.PlayerSide == RebellionSide.Rebel)
                        text = NightsWatchDialogs.PrisonerRefusesPlayerRebellion;
                    else if (RebellionCampaignBehavior.Instance.RebellionData.PlayerSide == RebellionSide.Loyalist
                        && prisoner.GetRelation(Hero.MainHero) < -10
                        && prisoner.GetTraitLevel(DefaultTraits.Honor) > 0)
                        text = NightsWatchDialogs.PrisonerRefusesPlayerLoyalistRelationNegativeLordHonourable;
                    else if (RebellionCampaignBehavior.Instance.RebellionData.PlayerSide == RebellionSide.Loyalist
                        && prisoner.GetTraitLevel(DefaultTraits.Honor) < 0)
                        text = NightsWatchDialogs.PrisonerRefusesPlayerLoyalistLordDevious;
                    else if (prisoner.GetTraitLevel(DefaultTraits.Honor) < 0)
                        text = NightsWatchDialogs.PrisonerRefusesDevious;
                    else if (prisoner.GetTraitLevel(DefaultTraits.Honor) > 0)
                        text = NightsWatchDialogs.PrisonerRefusesHonourable;
                    else
                        text = NightsWatchDialogs.PrisonerRefusesNormal;
                    GameTexts.SetVariable("SEND_TO_NIGHTS_WATCH_RESPONSE", text);

                    _lastRefusalTimes[prisoner.StringId] = CampaignTime.Now;
                    return true;
                },
                null
            );
            starter.AddPlayerLine(
                "send_to_nights_watch_execute",
                "send_to_nights_watch_after_refuse",
                "close_window",
                "{SEND_TO_NIGHTS_WATCH_EXECUTE}",
                () =>
                {
                    if (Hero.MainHero.GetTraitLevel(DefaultTraits.Honor) < 0)
                        GameTexts.SetVariable("SEND_TO_NIGHTS_WATCH_EXECUTE", NightsWatchDialogs.ExecuteAfterRefusalDevious);
                    else if (Hero.MainHero.GetTraitLevel(DefaultTraits.Honor) > 0)
                        GameTexts.SetVariable("SEND_TO_NIGHTS_WATCH_EXECUTE", NightsWatchDialogs.ExecuteAfterRefusalHonourable);
                    else
                        GameTexts.SetVariable("SEND_TO_NIGHTS_WATCH_EXECUTE", NightsWatchDialogs.ExecuteAfterRefusalNormal);
                    return true;
                },
                () =>
                {
                    MBInformationManager.ShowSceneNotification(HeroExecutionSceneNotificationData.CreateForPlayerExecutingHero(Hero.OneToOneConversationHero, delegate { }, SceneNotificationData.RelevantContextType.Any, false));
                    if (MobileParty.MainParty.MapEvent != null)
                        KillCharacterAction.ApplyByExecutionAfterMapEvent(Hero.OneToOneConversationHero, Hero.MainHero);
                    else KillCharacterAction.ApplyByExecution(Hero.OneToOneConversationHero, Hero.MainHero);
                }, 100,
                (out TextObject exp) =>
                {
                    exp = new TextObject("");
                    if (Hero.MainHero.GetTraitLevel(DefaultTraits.Mercy) > 0)
                    {
                        exp = new TextObject("{=bab_ironborn_wife_merciful}You are merciful");
                        return false;
                    }
                    return true;
                });
            starter.AddPlayerLine(
                "nights_watch_spare",
                "nights_watch_after_refuse",
                "close_window",
                "{NIGHTS_WATCH_SPARE}",
                () =>
                {
                    if (Hero.MainHero.GetTraitLevel(DefaultTraits.Honor) > 0
                    || Hero.MainHero.GetTraitLevel(DefaultTraits.Mercy) > 0)
                        GameTexts.SetVariable("NIGHTS_WATCH_SPARE", NightsWatchDialogs.SpareHonourable);
                    else
                        GameTexts.SetVariable("NIGHTS_WATCH_SPARE", NightsWatchDialogs.SpareNormal);
                    return true;
                },
                null, 100,
                (out TextObject exp) =>
                {
                    exp = new TextObject("");
                    if (Hero.MainHero.GetTraitLevel(DefaultTraits.Mercy) > 0) return true;
                    if (Hero.MainHero.GetTraitLevel(DefaultTraits.Honor) > 0)
                        return true;
                    exp = new TextObject("{=bab_nightwatch_not_honourable_merciful}You need to be honourable or merciful");
                    return false;
                }
            );
        }

        private void JoinNightsWatch(Hero main, Hero prisoner)
        {
            ChangeRelationAction.ApplyRelationChangeBetweenHeroes(main, prisoner.Clan.Leader, -100);
            var joinedNightsWatchLogEntry = new JoinedNightsWatchLogEntry(prisoner, main);
            LogEntry.AddLogEntry(joinedNightsWatchLogEntry);
            prisoner.Clan = NightsWatchConfig.NightsWatchClanToJoin;
            EndCaptivityAction.ApplyByReleasedAfterBattle(prisoner);
        }

        private bool IsRulerOfRegion(Hero hero)
        {
            return hero.MapFaction != null && NightsWatchConfig.KingdomsWhoCanForceToNightsWatch.Contains(hero.MapFaction.StringId) && hero.IsKingdomLeader;
        }
        private bool CanAIForceToJoinNightsWatch(Hero capturer, Hero prisoner)
        {
            if (prisoner.IsFemale) return false;
            if (!IsRulerOfRegion(capturer)) return false;
            return true;
        }
        private bool CanPlayerForceToJoinNightsWatch(Hero prisoner)
        {
            if (prisoner.IsFemale) return false;
            return Hero.MainHero.Clan.Fiefs.Count > 0;
        }
        private bool ConditionToStartDialog()
        {
            var prisoner = Hero.OneToOneConversationHero;
            if (!prisoner.IsPrisoner) return false;
            if (!CanPlayerForceToJoinNightsWatch(prisoner)) return false;
            if (Campaign.Current.CurrentConversationContext == ConversationContext.CapturedLord) return false;
            if (Campaign.Current.CurrentConversationContext == ConversationContext.FreeOrCapturePrisonerHero) return false;
            var belongsToPlayer = (prisoner.PartyBelongedToAsPrisoner != null && prisoner.PartyBelongedToAsPrisoner.Owner.Clan == Clan.PlayerClan)
                || (prisoner.CurrentSettlement != null && prisoner.CurrentSettlement.OwnerClan == Clan.PlayerClan);
            if (!belongsToPlayer) return false;
            if (_lastRefusalTimes != null && _lastRefusalTimes.TryGetValue(prisoner.StringId, out var last))
            {
                if (CampaignTime.Now - last < CampaignTime.Days(1)) return false;
                _lastRefusalTimes.Remove(prisoner.StringId);
            }
            return true;
        }
        public override void SyncData(IDataStore dataStore)
        {
            dataStore.SyncData("NightsWatch_LastRefusalTimes", ref _lastRefusalTimes);
        }
    }
}