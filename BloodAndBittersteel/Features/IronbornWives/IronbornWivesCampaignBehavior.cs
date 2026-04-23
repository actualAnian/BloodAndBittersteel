using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.SceneInformationPopupTypes;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.SaveSystem;

namespace BloodAndBittersteel.Features.IronbornWives
{
    public class IronbornWivesCampaignBehavior : CampaignBehaviorBase
    {
        const int MaxSpousesForAiLords = 2;
        const float BaseAcceptanceChance = 0.8f;
        readonly Random _random = new();
        [SaveableField(1)]
        private Dictionary<string, CampaignTime> _lastRefusalTimes = new();
        public override void RegisterEvents()
        {
            CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, OnSessionLaunched);
            CampaignEvents.MapEventEnded.AddNonSerializedListener(this, OnMapEventEnded);
        }

        private void OnMapEventEnded(MapEvent mapEvent)
        {
            var lolol = MobileParty.All.First(p => p.LeaderHero != null && p.LeaderHero.StringId == "IRON_22m_03");
            foreach(var party in mapEvent.InvolvedParties)
            {
                if (party == PartyBase.MainParty) continue;
                if (party.LeaderHero == null || party.LeaderHero.Culture.StringId != Globals.IronbornCultureId) return;
                var hero = party.LeaderHero;
                var validPrisoner = party.PrisonerHeroes.FirstOrDefault(p => CanTakeWife(hero, p.HeroObject));
                if (validPrisoner != null)
                {
                    var spousesAmount = 0;
                    if (hero.Spouse != null) spousesAmount++;
                    foreach (var exSpouse in hero.ExSpouses)
                        if (exSpouse.IsAlive) spousesAmount++;
                    if (spousesAmount > MaxSpousesForAiLords) continue;
                    var prisoner = validPrisoner.HeroObject;
                        TakeWife(hero, prisoner);
                }
            }
        }

        private void OnSessionLaunched(CampaignGameStarter campaignGameStarter)
        {
            AddDialogs(campaignGameStarter);
        }
        bool WillAcceptMarriage(Hero prisoner)
        {
            float chanceToAccept = GetChanceToAcceptMarriage(prisoner);
            return _random.NextDouble() < chanceToAccept;
        }
        float GetChanceToAcceptMarriage(Hero prisoner)
        {
            var chance = BaseAcceptanceChance;
            if (IsFighter(prisoner)) chance -= 0.3f;
            return chance;
        }
        bool IsFighter(Hero prisoner)
        {
            return prisoner.GetSkillValue(DefaultSkills.OneHanded) > 100
                    || prisoner.GetSkillValue(DefaultSkills.TwoHanded) > 100;
        }
        public void AddDialogs(CampaignGameStarter starter)
        {
            starter.AddPlayerLine(
                "ironborn_wife_start",
                "hero_main_options",
                "ironborn_wife_response",
                "{IRONBORN_WIFE_START}",
                () =>
                {
                    bool isValidDialog = ConditionToStartDialog();
                    if (!isValidDialog) return false;

                    if (Hero.MainHero.GetTraitLevel(DefaultTraits.Mercy) < 0
                        && Hero.MainHero.GetTraitLevel(DefaultTraits.Honor) < 0)
                        GameTexts.SetVariable("IRONBORN_WIFE_START", IronbornWivesDialogs.StartDialogSadistic);
                    else if (Hero.MainHero.GetTraitLevel(DefaultTraits.Mercy) > 0)
                        GameTexts.SetVariable("IRONBORN_WIFE_START", IronbornWivesDialogs.StartDialogMerciful);
                    else if (Hero.MainHero.GetTraitLevel(DefaultTraits.Calculating) > 0)
                        GameTexts.SetVariable("IRONBORN_WIFE_START", IronbornWivesDialogs.StartDialogCalculating);
                    else if (Hero.MainHero.GetTraitLevel(DefaultTraits.Honor) < 0)
                        GameTexts.SetVariable("IRONBORN_WIFE_START", IronbornWivesDialogs.StartDialogDevious);
                    else if (Hero.MainHero.GetTraitLevel(DefaultTraits.Calculating) < 0)
                        GameTexts.SetVariable("IRONBORN_WIFE_START", IronbornWivesDialogs.StartDialogImpulsive);
                    else
                        GameTexts.SetVariable("IRONBORN_WIFE_START", IronbornWivesDialogs.StartDialogNeutral);
                    return true;
                },
                null
            );
            starter.AddDialogLine(
                "ironborn_wife_response_accept",
                "ironborn_wife_response",
                "ironborn_wife_finalize",
                "{IRONBORN_WIFE_OFFER}",
                () =>
                {
                    var prisoner = Hero.OneToOneConversationHero;
                    if (prisoner == null) return false;
                    if (!WillAcceptMarriage(prisoner)) return false;
                    string text = "";
                    if (IsFighter(prisoner)) text = IronbornWivesDialogs.NobleFighterAgree;
                    else text = prisoner.Clan.Tier switch
                    {
                        0 or 1 or 2 or 3 => IronbornWivesDialogs.LowNobleNonFighterAgree,
                        _ => IronbornWivesDialogs.HighNobleNonFighterAgree
                    };
                    GameTexts.SetVariable("IRONBORN_WIFE_OFFER", text);
                    return true;
                },
                null
            );

            starter.AddDialogLine(
                "ironborn_wife_response_refuse",
                "ironborn_wife_response",
                "ironborn_wife_after_refuse",
                "{IRONBORN_WIFE_OFFER}",
                () =>
                {
                    var prisoner = Hero.OneToOneConversationHero;
                    string text = "";
                    if (IsFighter(prisoner)) text = IronbornWivesDialogs.NobleFighterRefuse;
                    else text = prisoner.Clan.Tier switch
                    {
                        0 or 1 or 2 or 3 => IronbornWivesDialogs.LowNobleNonFighterRefuse,
                        _ => IronbornWivesDialogs.HighNobleNonFighterRefuse
                    };
                    GameTexts.SetVariable("IRONBORN_WIFE_OFFER", text);
                    _lastRefusalTimes[prisoner.StringId] = CampaignTime.Now;
                    return true;
                },
                null
            );
            starter.AddPlayerLine(
                "ironborn_wife_execute",
                "ironborn_wife_after_refuse",
                "close_window",
                "{IRONBORN_WIFE_EXECUTE}",
                () =>
                {
                    if (Hero.MainHero.GetTraitLevel(DefaultTraits.Mercy) < 0
                        && Hero.MainHero.GetTraitLevel(DefaultTraits.Honor) < 0)
                        GameTexts.SetVariable("IRONBORN_WIFE_EXECUTE", IronbornWivesDialogs.ExecutionSadistic);
                    else if (Hero.MainHero.GetTraitLevel(DefaultTraits.Honor) < 0)
                        GameTexts.SetVariable("IRONBORN_WIFE_EXECUTE", IronbornWivesDialogs.ExecutionDevious);
                    else if (Hero.MainHero.GetTraitLevel(DefaultTraits.Calculating) < 0)
                        GameTexts.SetVariable("IRONBORN_WIFE_EXECUTE", IronbornWivesDialogs.ExecutionImpulsive);
                    else
                        GameTexts.SetVariable("IRONBORN_WIFE_EXECUTE", IronbornWivesDialogs.ExecutionNeutral);
                    return true;
                },
                () => 
                {
                    MBInformationManager.ShowSceneNotification(HeroExecutionSceneNotificationData.CreateForPlayerExecutingHero(Hero.OneToOneConversationHero, delegate { }, SceneNotificationData.RelevantContextType.Any, false));
                    if (MobileParty.MainParty.MapEvent != null)
                        KillCharacterAction.ApplyByExecutionAfterMapEvent(Hero.OneToOneConversationHero, Hero.MainHero);
                    else  KillCharacterAction.ApplyByExecution(Hero.OneToOneConversationHero, Hero.MainHero);
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
                "ironborn_wife_spare",
                "ironborn_wife_after_refuse",
                "close_window",
                "{IRONBORN_WIFE_SPARE}",
                () =>
                {
                    if (Hero.MainHero.GetTraitLevel(DefaultTraits.Mercy) > 0)
                        GameTexts.SetVariable("IRONBORN_WIFE_SPARE", IronbornWivesDialogs.SpareMerciful);
                    else if (Hero.MainHero.GetTraitLevel(DefaultTraits.Calculating) > 0)
                        GameTexts.SetVariable("IRONBORN_WIFE_SPARE", IronbornWivesDialogs.SpareCalculating);
                    else
                        GameTexts.SetVariable("IRONBORN_WIFE_SPARE", IronbornWivesDialogs.SpareNeutral);
                    return true;
                },
                null, 100,
                (out TextObject exp) =>
                {
                    exp = new TextObject("");
                    if (Hero.MainHero.GetTraitLevel(DefaultTraits.Mercy) > 0) return true;
                    if (Hero.MainHero.GetTraitLevel(DefaultTraits.Honor) >= 0
                        && Hero.MainHero.GetTraitLevel(DefaultTraits.Mercy) >= 0)
                        return true;
                    exp = new TextObject("{=bab_ironborn_wife_not_neutral_merciful}You need to be neutral or merciful");
                    return false;
                });

            starter.AddPlayerLine(
                "ironborn_wife_finalize_confirm",
                "ironborn_wife_finalize",
                "close_window",
                "{IRONBORN_WIFE_CONFIRM}",
                () =>
                {
                    if (Hero.MainHero.GetTraitLevel(DefaultTraits.Mercy) < 0
                        || Hero.MainHero.GetTraitLevel(DefaultTraits.Honor) < 0)
                    {
                        if (IsFighter(Hero.OneToOneConversationHero) || Hero.OneToOneConversationHero.Clan.Tier > 3)
                            GameTexts.SetVariable("IRONBORN_WIFE_CONFIRM", IronbornWivesDialogs.ConfirmationDevious);
                    }
                    else
                        GameTexts.SetVariable("IRONBORN_WIFE_CONFIRM", IronbornWivesDialogs.ConfirmationNeutral);
                    return true;
                },
                () => { TakeWife(Hero.MainHero, Hero.OneToOneConversationHero); }
            );
            starter.AddPlayerLine(
                "ironborn_wife_finalize_retract",
                "ironborn_wife_finalize",
                "close_window",
                "{IRONBORN_WIFE_RETRACT}",
                () =>
                {
                    GameTexts.SetVariable("IRONBORN_WIFE_RETRACT", IronbornWivesDialogs.RetractionNeutral);
                    return true;
                },
                null
            );
        }

        private void TakeWife(Hero main, Hero prisoner)
        {
            var previousClanLeader = prisoner.Clan.Leader;
            MarriageAction.Apply(main, prisoner, true);
            if (prisoner.Clan != main.Clan) return; // marriage was not successful
            ChangeRelationAction.ApplyRelationChangeBetweenHeroes(main, previousClanLeader, -100);
            EndCaptivityAction.ApplyByReleasedAfterBattle(prisoner);
        }
        private bool CanTakeWife(Hero capturer, Hero prisoner)
        {
            if (capturer.Culture.StringId != Globals.IronbornCultureId) return false;
            if (prisoner == null) return false;
            if (!prisoner.IsFemale) return false;
            if (prisoner.Culture.StringId == Globals.IronbornCultureId) return false;
            return true;
        }
        private bool ConditionToStartDialog()
        {
            var capturer = Hero.MainHero;
            var prisoner = Hero.OneToOneConversationHero;
            CanTakeWife(capturer, prisoner);
            if (!prisoner.IsPrisoner) return false;
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
            dataStore.SyncData("IronbornWives_LastRefusalTimes", ref _lastRefusalTimes);
        }
    }
}