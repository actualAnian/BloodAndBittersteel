using BloodAndBittersteel.Features.LanceSystem;
using System;
using System.Linq;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.ViewModelCollection.Party;
using TaleWorlds.Library;

namespace BloodAndBittersteel.UI
{
    internal class BaBPartyVM : PartyVM
    {
        public BaBPartyVM(PartyScreenLogic partyScreenLogic) : base(partyScreenLogic)
        {
        }
        private void InitializePartyList(MBBindingList<PartyCharacterVM> partyList, TroopRoster currentTroopRoster, PartyScreenLogic.TroopType type, int side)
        {
            partyList.Clear();
            MBList<TroopRosterElement> troopRoster = currentTroopRoster.GetTroopRoster();
            for (int i = 0; i < troopRoster.Count; i++)
            {
                TroopRosterElement troopRosterElement = troopRoster[i];
                if (troopRosterElement.Character == null)
                {
                    Debug.FailedAssert("Invalid TroopRosterElement found in InitializePartyList!", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem.ViewModelCollection\\Party\\PartyVM.cs", "InitializePartyList", 497);
                }
                else
                {
                    PartyCharacterVM partyCharacterVM = new PartyCharacterVM(this.PartyScreenLogic, this, currentTroopRoster, currentTroopRoster.FindIndexOfTroop(troopRosterElement.Character), type, (PartyScreenLogic.PartyRosterSide)side, this.PartyScreenLogic.IsTroopTransferable(type, troopRosterElement.Character, side));
                    partyList.Add(partyCharacterVM);
                    partyCharacterVM.ThrowOnPropertyChanged();
                    partyCharacterVM.IsLocked = true;
                }
            }
        }

        MBBindingList<PartyCharacterVM> _mainPartyTroops;
        [DataSourceProperty]
        new public MBBindingList<PartyCharacterVM> MainPartyTroops
        {
            get
            {
                return base.MainPartyTroops;
            }
            set
            {
                if (value != _mainPartyTroops)
                {
                    _mainPartyTroops = value;
                    OnPropertyChangedWithValue(value, "MainPartyTroops");
                }
            }
        }
        MBBindingList<LanceVM> _partyLances;
        [DataSourceProperty]
        public MBBindingList<LanceVM> PartyLances
        {
            get
            {
                var otherTroops = TroopRoster.CreateDummyTroopRoster();

                var allLances = PartyBase.MainParty.Lances();
                var displayLances = allLances
                    .Select(l => l.LanceRoster)
                    .ToList();

                bool hasLances = displayLances.Count > 0;

                foreach (var troop in PartyBase.MainParty.MemberRoster.GetTroopRoster())
                {
                    var character = troop.Character;
                    int total = troop.Number;
                    int woundedRemaining = troop.WoundedNumber;
                    foreach (var lance in displayLances)
                    {
                        if (total <= 0)
                            break;

                        int inLance = lance.GetTroopCount(character);
                        if (inLance <= 0)
                            continue;

                        int woundedHere = Math.Min(inLance, woundedRemaining);
                        if (woundedHere > 0)
                        {
                            lance.WoundTroop(character, woundedHere);
                            woundedRemaining -= woundedHere;
                        }

                        total -= inLance;
                    }
                    if (total > 0)
                    {
                        int woundedInOther = Math.Min(total, woundedRemaining);
                        otherTroops.AddToCounts(character, total, false, woundedInOther);
                    }
                }
                _partyLances = new() { new LanceVM("main_troops", new(), "Main Party Troops") };
                InitializePartyList(_partyLances[0].LanceTroops, otherTroops, PartyScreenLogic.TroopType.Member, 1);
                for (int i = 0; i < displayLances.Count; i++)
                {
                    TroopRoster lance = displayLances[i];
                    _partyLances.Add(new("lance_" + i, new(), "Temp Lance Name"));
                    InitializePartyList(_partyLances.Last().LanceTroops, lance, PartyScreenLogic.TroopType.Member, 1);
                }
                return _partyLances;
            }
            set
            {
                if (value != _partyLances)
                {
                    _partyLances = value;
                    OnPropertyChangedWithValue(value, "PartyLances");
                }
            }
        }
    }
}
