using BloodAndBittersteel.Features.BlackfyreRebellion;
using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.Incidents;
using TaleWorlds.Localization;
using static BloodAndBittersteel.Features.BaBIncidents.BaBIncidentsBase;
namespace BloodAndBittersteel.Features.BaBIncidents
{
    public class BaBBlackfyreRebellionIncidents : IIncidentProvider
    {
        public IEnumerable<Incident> InitializeEvents()
        {
            yield return CreateStartRebellionIncident();
        }

        public static void CreateRebellionKingdom()
        {
            var rebellionClan = Clan.FindFirst(c => c.StringId == RebellionConfig.RebellionLeader);
            Kingdom rebelKingdom = Kingdom.CreateKingdom(RebellionConfig.RebellionFactionStringId);
            rebelKingdom.InitializeKingdom(new(RebellionConfig.RebellionFactionName),
                                      new(RebellionConfig.RebellionFactionInformalName),
                                      rebellionClan.Culture,
                                      RebellionConfig.RebellionBanner,
                                      1,
                                      1,
                                      rebellionClan.HomeSettlement,
                                      new(RebellionConfig.RebellionFactionEncyclopediaText),
                                      new(RebellionConfig.RebellionFactionEncyclopediaTitle),
                                      new(RebellionConfig.RebellionFactionEncyclopediaRulerTitle));
            ChangeKingdomAction.ApplyByCreateKingdom(rebellionClan, rebelKingdom, false);
            
            var supporterClans =  Clan.FindAll(c => RebellionConfig.RebellionSupporterClans.Contains(c.StringId));
            foreach (var clan in supporterClans)
                ChangeKingdomAction.ApplyByJoinToKingdom(clan, rebelKingdom, default, false);
            var loyalistKingdoms = Kingdom.All.Where(k => k.IsMapFaction && RebellionConfig.LoyalistFactions.Contains(k.StringId));
            var behavior = Campaign.Current.GetCampaignBehavior<AllianceCampaignBehavior>();
            foreach (var kindom in loyalistKingdoms)
            {
                FactionManager.DeclareWar(rebelKingdom, kindom);
                foreach (var kindom2 in loyalistKingdoms)
                {
                    if (kindom == kindom2) continue;
                    behavior.StartAlliance(kindom, kindom2);
                }
            }
            Campaign.Current.GetCampaignBehavior<RebellionCampaignBehavior>().OnRebellionStart();
        }
        public static BaBIncident CreateStartRebellionIncident()
        {
            BaBIncident incident = new("start_rebellion", BaBIncidentTypes.OnDailyTick, 1);
            incident.Initialize("The Black Dragon Rises",
                                "",
                                IncidentsCampaignBehaviour.IncidentTrigger.LeavingVillage,
                                IncidentsCampaignBehaviour.IncidentType.Siege,
                                CampaignTime.Never,
                                (TextObject description) => { return true; });
            List<IncidentEffect> joinRebellion = new()
            {
                StartTheRebellionEffect(),
                JoinTheRebellionEffect(),
            };
            List<IncidentEffect> fightRebellion = new()
            {
                StartTheRebellionEffect(),
                FightTheRebellionEffect(),
            };
            List<IncidentEffect> stayNeutral = new()
            {
                StartTheRebellionEffect(),
            };
            var joinRebellionText = "{=bab_join_rebellion}You gather your men and march to the Blackwater Rush to join the rebellion. The time is nigh, now is the time for sword not quill";
            var fightRebellionText = "{=bab_fight_rebellion}Convinced by your captains and sergeants, you order the men to pack up camp to aid the Crown against the rabble of pretenders at their doorsteps.";
            var stayNeutralText = "{=bab_stay_neutral}Unsure of what to make of these news, you heed advice from both camps that have sprung up amongst your men, some shouted that the rebel cause was just while others half-drew their swords from their sheaths at the mention of the name Daemon. Not seeking a full out mutiny, you inform your men that your host shall not partake for the time being in this royal squabble. That your intent is to see which one promises more for fighting men.";
            incident.AddOption(joinRebellionText, joinRebellion, null, null);
            incident.AddOption(fightRebellionText, fightRebellion, null, null);
            incident.AddOption(stayNeutralText, stayNeutral, null, null);
            return incident;
        }
        public static IncidentEffect StartTheRebellionEffect()
        {
            return CreateCustomIncidentEffect(
                null!,
                () => { CreateRebellionKingdom(); return new List<TextObject> { TextObject.GetEmpty() }; },
                (IncidentEffect effect) =>
                {
                    TextObject textObject = new("The civil war engulfs the empire");
                    return new List<TextObject> { textObject };
                });
        }
        public static IncidentEffect JoinTheRebellionEffect()
        {
            return CreateCustomIncidentEffect(
                null!,
                () => { JoinRebellion(); return new List<TextObject> { TextObject.GetEmpty() }; },
                (IncidentEffect effect) =>
                {
                    TextObject textObject = new("{=bab_join_rebellion}You will join the rebels!");
                    return new List<TextObject> { textObject };
                });
        }
        private static void JoinRebellion()
        {
            var rebellion = Kingdom.All.First(k => k.StringId == RebellionConfig.RebellionFactionStringId);
            ChangeKingdomAction.ApplyByJoinToKingdom(Clan.PlayerClan, rebellion);
            ChangeRelationAction.ApplyPlayerRelation(rebellion.Leader, RebellionConfig.RelationGainOnJoinKingdom);
        }
        public static IncidentEffect FightTheRebellionEffect()
        {
            return CreateCustomIncidentEffect(
                null!,
                () => { FightRebellion(); return new List<TextObject> { TextObject.GetEmpty() }; },
                (IncidentEffect effect) =>
                {
                    TextObject textObject = new("{=bab_join_rebellion}You will join the loyalists!");
                    return new List<TextObject> { textObject };
                });
        }
        private static void FightRebellion()
        {
            var kingdom = Kingdom.All.First(k => k.StringId == "empire");
            ChangeKingdomAction.ApplyByJoinToKingdom(Clan.PlayerClan, kingdom);
            ChangeRelationAction.ApplyPlayerRelation(kingdom.Leader, RebellionConfig.RelationGainOnJoinKingdom);
        }
    }
}