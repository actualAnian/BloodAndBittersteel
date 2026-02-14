using LanceSystem.CampaignBehaviors;
using LanceSystem.LanceDataClasses;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Party.PartyComponents;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;
using TaleWorlds.SaveSystem;

namespace LanceSystem
{
    public class DisbandedLancePartyComponent : PartyComponent
    {
        [SaveableField(0)]
        private TextObject _name;
        [SaveableField(1)]
        private Settlement _homeSettlement;
        [SaveableField(2)]
        private Hero _owner;
        [SaveableField(3)]
        private string _notableLanceBelongsTo;
        public string NotableLanceBelongsTo => _notableLanceBelongsTo;

        public override bool AvoidHostileActions
        {
            get
            {
                return true;
            }
        }
        public DisbandedLancePartyComponent(Settlement homeSettlement, TextObject name, Hero owner, string notableLanceBelongsTo)
        {
            _name = name;
            _homeSettlement = homeSettlement;
            _owner = owner;
            _notableLanceBelongsTo = notableLanceBelongsTo;
        }
        public static MobileParty CreateDisbandedLanceParty(NotableLanceData lanceToDisband, PartyBase previousOwner)
        {
            var settlement = MBObjectManager.Instance.GetObject<Settlement>(lanceToDisband.SettlementStringId);
            var notable = MBObjectManager.Instance.GetObject<CharacterObject>(lanceToDisband.NotableId).HeroObject;
            var lanceName = LancesCampaignBehavior.GetLanceName(notable, settlement, lanceToDisband.GetSettlementNotableLanceInfo().CurrentLance);
            MobileParty disbandedParty = MobileParty.CreateParty("disbanded_lance", new DisbandedLancePartyComponent(settlement, lanceName, settlement.Owner, lanceToDisband.NotableId));
            disbandedParty.ActualClan = settlement.OwnerClan;
            disbandedParty.InitializeMobilePartyAroundPosition(lanceToDisband.LanceRoster, TroopRoster.CreateDummyTroopRoster(), previousOwner.Position, 5);
            disbandedParty.Party.SetVisualAsDirty();
            disbandedParty.SetTargetSettlement(settlement, settlement.HasPort);

            return disbandedParty;
        }
        public override Hero PartyOwner
        {
            get
            {
                return _owner;
            }
        }
        public override TextObject Name
        {
            get
            {
                return _name;
            }
        }
        public override Settlement HomeSettlement
        {
            get
            {
                return _homeSettlement;
            }
        }

        public override Banner GetDefaultComponentBanner()
        {
            return HomeSettlement.OwnerClan.Banner;
        }
    }
}
