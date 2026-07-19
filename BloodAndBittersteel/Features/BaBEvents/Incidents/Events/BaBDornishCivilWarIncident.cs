using BloodAndBittersteel.Features.BaBEvents.PopUpEvents.Events;
using BloodAndBittersteel.Features.BlackfyreRebellion;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.Incidents;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace BloodAndBittersteel.Features.BaBEvents.Incidents.Events
{
    public class BaBDornishCivilWarIncident
    {
        public static readonly string DorneKingdomId = "Dorne";
        private static IncidentEffect JoinLoyalistsEffect()
        {
            return BaBIncidentsBase.CreateCustomIncidentEffect(
                null!,
                () =>
                {
                    JoinLoyalistSide();
                    return new List<TextObject> { TextObject.GetEmpty() };
                },
                effect =>
                {
                    TextObject textObject = new("{=bab_dornish_loyalist}You will stand with {DORNE_KING}! The Vulture King must be stopped before his rebellion tears all of Dorne apart!");
                    var loyalKingdom = Kingdom.All.FirstOrDefault(k => k.StringId == DorneKingdomId);
                    GameTexts.SetVariable("DORNE_KING", loyalKingdom.Leader.Name);
                    return new List<TextObject> { textObject };
                });
        }

        private static void JoinLoyalistSide()
        {
            var loyalKingdom = Kingdom.All.FirstOrDefault(k => k.StringId == DorneKingdomId);

            if (loyalKingdom != null && !loyalKingdom.IsEliminated)
            {
                ChangeKingdomAction.ApplyByJoinToKingdom(Clan.PlayerClan, loyalKingdom, default, false);
                ChangeRelationAction.ApplyPlayerRelation(loyalKingdom.Leader, RebellionConfig.RelationGainOnJoinKingdom);
            }
        }

        private static IncidentEffect JoinRebelsEffect()
        {
            return BaBIncidentsBase.CreateCustomIncidentEffect(
                null!,
                () =>
                {
                    JoinRebelSide();
                    return new List<TextObject> { TextObject.GetEmpty() };
                },
                effect =>
                {
                    TextObject textObject = new("{=bab_dornish_rebels}You swear your sword and fealty to the Vulture King's cause, that Dorne shall be free of the Iron Throne's yoke!");
                    return new List<TextObject> { textObject };
                });
        }

        private static void JoinRebelSide()
        {
            var rebelKingdom = Kingdom.All.FirstOrDefault(k => k.StringId == VultureKingEvent.NewDornishKingdomId);

            if (rebelKingdom != null && !rebelKingdom.IsEliminated)
            {
                ChangeKingdomAction.ApplyByJoinToKingdom(Clan.PlayerClan, rebelKingdom, default, false);
                ChangeRelationAction.ApplyPlayerRelation(rebelKingdom.Leader, RebellionConfig.RelationGainOnJoinKingdom);
            }
        }

        [BaBEvent]
        public static BaBIncident CreateDornishSuccessionIncident()
        {
            BaBIncident incident = new("dornish_succession", BaBEventTypes.OnTick, 1f);

            incident.Initialize(
                "{=bab_dornish_succession_title}The Prince of Dorne Calls Upon You",
                "",
                IncidentsCampaignBehaviour.IncidentTrigger.LeavingVillage,
                IncidentsCampaignBehaviour.IncidentType.HardTravel,
                CampaignTime.Never,
                condition: description =>
                {
                    if (BaBEventsCampaignBehavior.Instance.ForceInvokeIncidentNextTick) return true;
                    if (BaBEventsCampaignBehavior.Instance.HasEventFired(VultureKingEvent.StringId))
                        return false;
                    var playerClan = Clan.PlayerClan;
                    if (playerClan.Kingdom == null)
                        return false;
                    return playerClan.Kingdom?.StringId == DorneKingdomId;
                });

            List<IncidentEffect> joinLoyalists = new()
            {
                JoinLoyalistsEffect(),
            };

            List<IncidentEffect> joinRebels = new()
            {
                JoinRebelsEffect(),
            };

            string joinLoyalistText = new TextObject("{=bab_join_loyalist_txt}The Prince of Dorne sends you word. With the Vulture King rising in rebellion, he needs loyal men and women to take up arms alongside him against his rebellious lords.").ToString();
            string joinRebelText = new TextObject("{=bab_join_rebel_txt}You receive whispers that the Vulture King's cause could mean freedom for all of Dorne from Westeros. His rebel host could use your sword, your steel. That you could break the Prince of Dorne's weakness forever, ending any prince or lord in Dorne who bows to outsiders, never again forced into submission.").ToString();

            incident.AddOption(joinLoyalistText, joinLoyalists, null, null);
            incident.AddOption(joinRebelText, joinRebels, null, null);
            return incident;
        }
    }
}
