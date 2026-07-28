using BloodAndBittersteel.Features.BaBEvents;
using BloodAndBittersteel.Features.BaBEvents.PopUpEvents.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.SaveSystem;

namespace BloodAndBittersteel.Features.Jousting
{
    public class JoustingCampaignBehavior : CampaignBehaviorBase
    {

        [SaveableField(1)]
        private CampaignTime _lastJoustingTournamentDate = CampaignTime.Zero;
        [SaveableField(2)]
        private List<Town> _activeJoustingTournamentTowns = new();
        [SaveableField(2)]
        private Dictionary<CampaignTime, string> _recentlyWeddedGrooms = new();
        private static JoustingCampaignBehavior? _instance;
        public static JoustingCampaignBehavior Instance => _instance ??= Campaign.Current.GetCampaignBehavior<JoustingCampaignBehavior>();
        public List<Town> ActiveJoustingTournamentTown => _activeJoustingTournamentTowns;
        public bool IsJoustingTournamentActive => _activeJoustingTournamentTowns.Count > 0;

        public override void RegisterEvents()
        {
            CampaignEvents.DailyTickClanEvent.AddNonSerializedListener(this, DailyTickClan);
            CampaignEvents.RomanticStateChanged.AddNonSerializedListener(this, OnRomanticStateChanged);
            CampaignEvents.TournamentFinished.AddNonSerializedListener(this, OnTournamentFinished);
            CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, OnSessionLaunched);
        }

        public override void SyncData(IDataStore dataStore)
        {
            dataStore.SyncData("_lastJoustingTournamentDate", ref _lastJoustingTournamentDate);
            dataStore.SyncData("_activeJoustingTournamentTowns", ref _activeJoustingTournamentTowns);
            dataStore.SyncData("_recentWeddings", ref _recentlyWeddedGrooms);
        }

        public bool IsRecentlyWedGroom(Hero hero)
        {
            foreach (var value in _recentlyWeddedGrooms) 
            {
                var key = value.Key;
                if (key + CampaignTime.Days(JoustingConfig.GroomBoostDurationDays) < CampaignTime.Now)
                _recentlyWeddedGrooms.Remove(value.Key);
            }
            return _recentlyWeddedGrooms.Values.Any(w => w == hero.StringId);
        }

        private void DailyTickClan(Clan clan)
        {
            if (!MeetsClanConditions(clan))
                return;

            var rollChance = CalculateHostingChance(clan, false);
            if (MBRandom.RandomFloat >= rollChance)
                return;

            var bestTown = SelectBestTownForTournament(clan);
            if (bestTown == null)
                return;

            GameTexts.SetVariable("CLAN_OWNER", clan.Leader.Name);
            GameTexts.SetVariable("TOWN_NAME", bestTown.Name);
            StartJoustingTournament(bestTown, true, JoustingConfig.BaseReason);
        }

        private bool MeetsClanConditions(Clan clan)
        {
            if (JoustingConfig.CulturesThatCantHost.Contains(clan.Culture.StringId)
                || clan.Gold < JoustingConfig.MinimumTreasuryForHosting
                || clan.Fiefs.Any(f => f.IsUnderSiege))
                    return false;

            return true;
        }

        private float CalculateHostingChance(Clan clan, bool fromWedding)
        {
            if (!fromWedding && _activeJoustingTournamentTowns.Count > 0) return 0f;
            var treasuryBonus = GetTreasuryBonus(clan);
            var malusFromExistingTournaments = _activeJoustingTournamentTowns.Count * JoustingConfig.HostingMalusPerActiveTournament;
            return MathF.Clamp(JoustingConfig.DefaultJoustBaseChance + treasuryBonus + JoustingConfig.WeddingBoostChance - malusFromExistingTournaments, 0f, 1f);
        }

        private Town? SelectBestTownForTournament(Clan clan)
        {
            var eligibleTowns = clan.Fiefs.Where(f => f.IsTown).ToList();

            if (eligibleTowns.Count == 0)
            {
                return null;
            }

            return eligibleTowns
                .OrderByDescending(t => CountFriendlyLordsNearby(t))
                .First();
        }

        private static int CountFriendlyLordsNearby(Town town)
        {
            var settlement = town.Settlement;
            var ownerFaction = town.OwnerClan?.MapFaction;

            var rangeSquared = JoustingConfig.NearByLordRange * JoustingConfig.NearByLordRange;
            return MobileParty.All
                .Count(p => p.IsLordParty
                    && p.LeaderHero != null
                    && p.GetPosition2D.DistanceSquared(settlement.GetPosition2D) < rangeSquared
                    && !p.MapFaction.IsAtWarWith(ownerFaction));
        }

        private static float GetTreasuryBonus(Clan clan)
        {
            if (clan.Gold < JoustingConfig.MinimumTreasuryForHosting)
                return 0f;
            var excessGold = (clan.Gold - JoustingConfig.MinimumTreasuryForHosting) / JoustingConfig.AmountOfGoldForMaxBonus;
            return MathF.Min(excessGold * 0.02f, JoustingConfig.MaxScoreFromTreasury);
        }
        private void FireJoustingPopUp(TextObject reason)
        {
            var baBEvent = BaBEventLoader.Instance.AllEvents.FirstOrDefault(e => e.StringId == JoustingEvent.StringId);

            if (baBEvent is null)
            {
                InformationManager.DisplayMessage(new($"Event 'jousting_tournament' not found."));
                return;
            }
            var mapState = GameStateManager.Current.LastOrDefault<MapState>();
            if (mapState == null) return;
            GameTexts.SetVariable("JOUSTING_DESCRIPTION", reason.ToString());
            BaBEventsCampaignBehavior.FireEvent(baBEvent, mapState);
        }
        private void StartJoustingTournament(Town town, bool showPopUp, TextObject reason)
        {
            if (showPopUp
            && BaBSettings.Instance.ShowJoustingPopUp 
            && (Clan.PlayerClan.MapFaction == null || !town.Owner.MapFaction.FactionsAtWarWith.Contains(Clan.PlayerClan.MapFaction)))
                FireJoustingPopUp(reason);
            _activeJoustingTournamentTowns.Add(town);
            _lastJoustingTournamentDate = CampaignTime.Now;
            Campaign.Current.TournamentManager.AddTournament(new JoustTournamentGame(town));
        }

        private void EndJoustingTournament(Town town)
        {
            if (Campaign.Current.TournamentManager.GetTournamentGame(town) != null)
                Campaign.Current.TournamentManager.ResolveTournament(Campaign.Current.TournamentManager.GetTournamentGame(town), town);
            _activeJoustingTournamentTowns.Remove(town);
        }

        private void OnRomanticStateChanged(Hero hero1, Hero hero2, Romance.RomanceLevelEnum level)
        {
            if (level != Romance.RomanceLevelEnum.Marriage)
                return;

            var groom = hero1.IsFemale ? hero2 : hero1;
            var bride = hero1.IsFemale ? hero1 : hero2;
            var weddingClan = hero1.Clan ?? hero2.Clan;
            if (weddingClan == null)
                return;

            if (!MeetsClanConditions(weddingClan)) return;
            var rollChance = CalculateHostingChance(weddingClan, true);
            if (MBRandom.RandomFloat >= rollChance)
                return;

            var bestTown = SelectBestTownForTournament(weddingClan);
            if (bestTown == null)
                return;
            GameTexts.SetVariable("GROOM", groom.Name);
            GameTexts.SetVariable("BRIDE", bride.Name);
            GameTexts.SetVariable("CLAN_NAME", weddingClan.Name);
            GameTexts.SetVariable("TOWN_NAME", bestTown.Name);

            StartJoustingTournament(bestTown, true, JoustingConfig.WeddingReason);
        }

        private void OnTournamentFinished(CharacterObject winner, MBReadOnlyList<CharacterObject> participants, Town town, ItemObject prize)
        {
            if (_activeJoustingTournamentTowns.Contains(town))
                EndJoustingTournament(town);
        }

        private void OnSessionLaunched(CampaignGameStarter campaignGameStarter)
        {
            AddDialogs(campaignGameStarter);
        }

        private void AddDialogs(CampaignGameStarter starter)
        {
            starter.AddPlayerLine(
                "bab_jousting_host_start",
                "hero_main_options",
                "bab_jousting_host_response",
                "{=bab_joust_host_start}I wish to host a jousting tournament.",
                () =>
                {
                    var governor = Hero.OneToOneConversationHero;
                    if (governor?.GovernorOf == null) return false;
                    return governor.GovernorOf.Owner.Owner == Hero.MainHero;
                },
                null
            );

            starter.AddDialogLine(
                "bab_jousting_host_fail",
                "bab_jousting_host_response",
                "lord_pretalk",
                "{BAB_JOUST_EXPLANATION}",
                () =>
                {
                    var reason = GetHostingFailureReason();
                    if (reason == null) return false;
                    GameTexts.SetVariable("BAB_JOUST_EXPLANATION", reason);
                    return true;
                },
                null
            );

            starter.AddDialogLine(
                "bab_jousting_host_success",
                "bab_jousting_host_response",
                "bab_jousting_host_confirm",
                "{BAB_JOUST_EXPLANATION}",
                () =>
                {
                    if (GetHostingFailureReason() != null) return false;
                    GameTexts.SetVariable("GOLD_REQUIRED", JoustingConfig.PlayerHostedJoustCost);
                    GameTexts.SetVariable("BAB_JOUST_EXPLANATION",
                        new TextObject("{=bab_joust_cost_proposal}Very well, my lord. Hosting a jousting tournament will cost {GOLD_REQUIRED} denars from the treasury. Shall I make the arrangements?"));
                    return true;
                },
                null,
                100
            );

            starter.AddPlayerLine(
                "bab_jousting_host_confirm_yes",
                "bab_jousting_host_confirm",
                "close_window",
                "{=bab_joust_confirm_yes}Yes, proceed with the arrangements.",
                null,
                () =>
                {
                    GiveGoldAction.ApplyBetweenCharacters(Hero.MainHero, null, JoustingConfig.PlayerHostedJoustCost);
                    var settlement = Settlement.CurrentSettlement;
                    if (settlement?.Town != null)
                    {
                        GameTexts.SetVariable("TOWN_NAME", settlement.Name);
                        StartJoustingTournament(settlement.Town, false,
                            new TextObject("{=bab_joust_player_reason}You decided to host a jousting tournament in {TOWN_NAME}"));
                    }
                }
            );

            starter.AddPlayerLine(
                "bab_jousting_host_confirm_no",
                "bab_jousting_host_confirm",
                "lord_pretalk",
                "{=bab_joust_confirm_no}Never mind, perhaps another time.",
                null,
                null
            );
        }

        private TextObject? GetHostingFailureReason()
        {
            if (Hero.MainHero.Gold < JoustingConfig.PlayerHostedJoustCost)
            {
                GameTexts.SetVariable("GOLD_REQUIRED", JoustingConfig.PlayerHostedJoustCost);
                return new TextObject("{=bab_joust_no_gold}My lord, we don't have enough gold in the treasury. We need at least {GOLD_REQUIRED} denars.");
            }

            if (_activeJoustingTournamentTowns.Count >= JoustingConfig.MaxActiveJoustingTournamentsForPlayerToHostHisOwn)
            {
                GameTexts.SetVariable("MAX_TOURNAMENTS", JoustingConfig.MaxActiveJoustingTournamentsForPlayerToHostHisOwn);
                return new TextObject("{=bab_joust_too_many}My lord, there are already {MAX_TOURNAMENTS} tournaments being held across the realm. We must wait until some conclude.");
            }

            var playerTownWithActiveTournament = Clan.PlayerClan.Fiefs.FirstOrDefault(t => Campaign.Current.TournamentManager.GetTournamentGame(t) != null);
            if (playerTownWithActiveTournament != null)
            {
                return new TextObject("{=bab_joust_town_tournament}My lord, one of our towns already has an ongoing tournament.");
            }

            return null;
        }
    }
}