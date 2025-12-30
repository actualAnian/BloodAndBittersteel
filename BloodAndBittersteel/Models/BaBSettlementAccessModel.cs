using BloodAndBittersteel.Features.Tournaments;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Localization;

namespace BloodAndBittersteel.Models
{
    internal class BaBSettlementAccessModel : SettlementAccessModel
    {
        public SettlementAccessModel _baseModel;
        public BaBSettlementAccessModel(SettlementAccessModel baseModel)
        {
            _baseModel = baseModel;
        }

        public override bool CanMainHeroAccessLocation(Settlement settlement, string locationId, out bool disableOption, out TextObject disabledText)
        {
            return _baseModel.CanMainHeroAccessLocation(settlement, locationId, out disableOption, out disabledText);
        }

        public override bool CanMainHeroDoSettlementAction(Settlement settlement, SettlementAction settlementAction, out bool disableOption, out TextObject disabledText)
        {
            if (settlementAction != SettlementAction.JoinTournament) return _baseModel.CanMainHeroDoSettlementAction(settlement, settlementAction, out disableOption, out disabledText);
            return CanPlayerJoinTournament(settlement, out disableOption, out disabledText);

        }
        private bool CanPlayerJoinTournament(Settlement settlement, out bool disableOption, out TextObject disabledText)
        {
            if (!TournamentConfig.CanParticipate(Hero.MainHero.CharacterObject))
            {
                disableOption = true;
                disabledText = new TextObject("{=bab_tournament_female_blocked}This land does not allow females to join tournaments. Perhaps if you made a reputation for yourself as valorous, it would change.");
                return false;
            }
            return _baseModel.CanMainHeroDoSettlementAction(settlement, SettlementAction.JoinTournament, out disableOption, out disabledText);
        }

        public override void CanMainHeroEnterDungeon(Settlement settlement, out AccessDetails accessDetails)
        {
            _baseModel.CanMainHeroEnterDungeon(settlement, out accessDetails);
        }

        public override void CanMainHeroEnterLordsHall(Settlement settlement, out AccessDetails accessDetails)
        {
            _baseModel.CanMainHeroEnterLordsHall(settlement, out accessDetails);
        }

        public override void CanMainHeroEnterSettlement(Settlement settlement, out AccessDetails accessDetails)
        {
            _baseModel.CanMainHeroEnterSettlement(settlement, out accessDetails);
        }

        public override bool IsRequestMeetingOptionAvailable(Settlement settlement, out bool disableOption, out TextObject disabledText)
        {
            return _baseModel.IsRequestMeetingOptionAvailable(settlement, out disableOption, out disabledText);
        }
    }
}
