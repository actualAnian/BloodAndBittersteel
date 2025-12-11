using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Localization;

namespace BloodAndBittersteel.Models
{
    internal class BaBKingdomDecisionPermissionModel : KingdomDecisionPermissionModel
    {
        KingdomDecisionPermissionModel _baseModel;
        public BaBKingdomDecisionPermissionModel(KingdomDecisionPermissionModel baseModel)
        {
            _baseModel = baseModel;
        }
        public override bool IsAnnexationDecisionAllowed(Settlement annexedSettlement)
        {
            return _baseModel.IsAnnexationDecisionAllowed(annexedSettlement);
        }

        public override bool IsExpulsionDecisionAllowed(Clan expelledClan)
        {
            return _baseModel.IsExpulsionDecisionAllowed(expelledClan);
        }

        public override bool IsKingSelectionDecisionAllowed(Kingdom kingdom)
        {
            return _baseModel.IsKingSelectionDecisionAllowed(kingdom);
        }

        public override bool IsPeaceDecisionAllowedBetweenKingdoms(Kingdom kingdom1, Kingdom kingdom2, out TextObject reason)
        {
            if (kingdom1.StringId == "khuzait" || kingdom2.StringId == "khuzait") return _baseModel.IsPeaceDecisionAllowedBetweenKingdoms(kingdom1, kingdom2, out reason);
            reason = new("{=bab_peace_denied}Peace cannot be made while a rebellion is ongoing.");
            return false;
        }

        public override bool IsPolicyDecisionAllowed(PolicyObject policy)
        {
            return _baseModel.IsPolicyDecisionAllowed(policy);
        }

        public override bool IsStartAllianceDecisionAllowedBetweenKingdoms(Kingdom kingdom1, Kingdom kingdom2, out TextObject reason)
        {
            return _baseModel.IsStartAllianceDecisionAllowedBetweenKingdoms(kingdom1, kingdom2, out reason);
        }

        public override bool IsWarDecisionAllowedBetweenKingdoms(Kingdom kingdom1, Kingdom kingdom2, out TextObject reason)
        {
            if (kingdom1.StringId == "khuzait" || kingdom2.StringId == "khuzait") return _baseModel.IsPeaceDecisionAllowedBetweenKingdoms(kingdom1, kingdom2, out reason);
            if (kingdom1.StringId.Contains("empire") && !kingdom2.StringId.Contains("empire"))
                reason = new("{=bab_declare_war_empire_vassal}Can not declare war on your vassal");
            else if (kingdom1.StringId.Contains("empire") && kingdom2.StringId.Contains("empire"))
                reason = new("{=bab_declare_war_empire_empire}Can not declare war on another part of your realm");
            else if (!kingdom1.StringId.Contains("empire") && kingdom2.StringId.Contains("empire"))
                reason = new("{=bab_declare_war_vassal_empire}Can not declare war on your liege");
            else 
                reason = new("{=bab_declare_war_vassal_vassal}Can not declare war on another crownland");
            return false;

        }
    }
}
