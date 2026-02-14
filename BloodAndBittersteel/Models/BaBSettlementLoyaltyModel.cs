using BloodAndBittersteel.Features.CampaignCheats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Settlements;

namespace BloodAndBittersteel.Models
{
    internal class BaBSettlementLoyaltyModel : SettlementLoyaltyModel
    {
        SettlementLoyaltyModel _previousModel;

        public BaBSettlementLoyaltyModel(SettlementLoyaltyModel previousModel)
        {
            _previousModel = previousModel;
        }

        public override int SettlementLoyaltyChangeDueToSecurityThreshold => _previousModel.SettlementLoyaltyChangeDueToSecurityThreshold;

        public override int MaximumLoyaltyInSettlement => _previousModel.MaximumLoyaltyInSettlement;

        public override int LoyaltyDriftMedium => _previousModel.LoyaltyDriftMedium;

        public override float HighLoyaltyProsperityEffect => _previousModel.HighLoyaltyProsperityEffect;

        public override int LowLoyaltyProsperityEffect => _previousModel.LowLoyaltyProsperityEffect;

        public override int MilitiaBoostPercentage => _previousModel.MilitiaBoostPercentage;

        public override float HighSecurityLoyaltyEffect => _previousModel.HighSecurityLoyaltyEffect;

        public override float LowSecurityLoyaltyEffect => _previousModel.LowSecurityLoyaltyEffect;

        public override float GovernorSameCultureLoyaltyEffect => _previousModel.GovernorSameCultureLoyaltyEffect;

        public override float GovernorDifferentCultureLoyaltyEffect => _previousModel.GovernorDifferentCultureLoyaltyEffect;

        public override float SettlementOwnerDifferentCultureLoyaltyEffect => _previousModel.SettlementOwnerDifferentCultureLoyaltyEffect;

        public override int ThresholdForTaxBoost => _previousModel.ThresholdForTaxBoost;

        public override int RebellionStartLoyaltyThreshold 
        {
            get
            {
                if (CampaignCheatsGlobalConfig.Instance.RebellionsEnabled)
                    return _previousModel.RebellionStartLoyaltyThreshold;
                else 
                    return -1;
            } 
        }

        public override int ThresholdForTaxCorruption => _previousModel.ThresholdForTaxCorruption;

        public override int ThresholdForHigherTaxCorruption => _previousModel.ThresholdForHigherTaxCorruption;

        public override int ThresholdForProsperityBoost => _previousModel.ThresholdForProsperityBoost;

        public override int ThresholdForProsperityPenalty => _previousModel.ThresholdForProsperityPenalty;

        public override int AdditionalStarvationPenaltyStartDay => _previousModel.AdditionalStarvationPenaltyStartDay;

        public override int AdditionalStarvationLoyaltyEffect => _previousModel.AdditionalStarvationLoyaltyEffect;

        public override int RebelliousStateStartLoyaltyThreshold => _previousModel.RebelliousStateStartLoyaltyThreshold;

        public override int LoyaltyBoostAfterRebellionStartValue => _previousModel.LoyaltyBoostAfterRebellionStartValue;

        public override float ThresholdForNotableRelationBonus => _previousModel.ThresholdForNotableRelationBonus;

        public override int DailyNotableRelationBonus => _previousModel.DailyNotableRelationBonus;

        public override void CalculateGoldCutDueToLowLoyalty(Town town, ref ExplainedNumber explainedNumber)
        {
            _previousModel.CalculateGoldCutDueToLowLoyalty(town, ref explainedNumber);
        }

        public override void CalculateGoldGainDueToHighLoyalty(Town town, ref ExplainedNumber explainedNumber)
        {
            _previousModel.CalculateGoldGainDueToHighLoyalty(town, ref explainedNumber);
        }

        public override ExplainedNumber CalculateLoyaltyChange(Town town, bool includeDescriptions = false)
        {
            return _previousModel.CalculateLoyaltyChange(town, includeDescriptions);
        }
    }
}
