using HarmonyLib;
using LanceSystem.CampaignBehaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.ViewModelCollection.Party;
using TaleWorlds.Library;
using static TaleWorlds.CampaignSystem.Party.PartyScreenLogic;

namespace LanceSystem.UI
{
    public class LancePartyVM : PartyVM
    {
        public static LancePartyVM? Instance { get; private set; }
        List<TroopRoster> _lancesTroopRosters;
        private readonly TroopRoster _originalOtherSideTroopRoster = TroopRoster.CreateDummyTroopRoster();
        private readonly TroopRoster _originalPlayerSideTroopRoster = TroopRoster.CreateDummyTroopRoster();
        readonly LancePartyUpgradeTroopVM _lanceUpgradePopUp;
        readonly LancePartyRecruitTroopVM _lancePartyRecruitTroopVM;

        public LancePartyVM(PartyScreenLogic partyScreenLogic) : base(partyScreenLogic)
        {
            PartyCharacterVM.OnTransfer += new Action<PartyCharacterVM, int, int, PartyRosterSide>(OnTransferTroop);
            PartyCharacterVM.SetSelected += new Action<PartyCharacterVM>(ExecuteSelectLanceCharacterTuple);
            _lancesTroopRosters = CreateDisplayPartyLances();
            _partyLances = CreatePartyLances();
            Instance = this; // needs to happen after CreatePartyLances 
            ResetRosterData(_originalOtherSideTroopRoster, PartyScreenLogic.MemberRosters[0]);
            ResetRosterData(_originalPlayerSideTroopRoster, PartyScreenLogic.MemberRosters[1]);
            _lanceUpgradePopUp = new LancePartyUpgradeTroopVM(this);
            UpgradePopUp = _lanceUpgradePopUp;
            _lancePartyRecruitTroopVM = new LancePartyRecruitTroopVM(this);
            RecruitPopUp = _lancePartyRecruitTroopVM;
        }
        private void ResetRosterData(TroopRoster cloneTo, TroopRoster clonefrom)
        {
            int stopAt;
            if (cloneTo.Count != 0 && cloneTo.GetCharacterAtIndex(0).IsHero) stopAt = 1;
            else stopAt = 0;
            for (int num = cloneTo.Count - 1; num >= stopAt; num--)
            {
                cloneTo.AddToCountsAtIndex(num, -cloneTo.GetElementNumber(num), -cloneTo.GetElementWoundedNumber(num));
            }
            for (int startAt = stopAt; startAt < clonefrom.Count; startAt++)
            {
                cloneTo.AddToCounts(clonefrom.GetCharacterAtIndex(startAt), clonefrom.GetElementNumber(startAt), false, clonefrom.GetElementWoundedNumber(startAt), clonefrom.GetElementXp(startAt));
            }
        }
        private List<TroopRoster> CreateDisplayPartyLances()
        {
            List<TroopRoster> lancesTroopRoster = new();
            var otherTroops = TroopRoster.CreateDummyTroopRoster();
            if (PartyBase.MainParty.Lances().Count == 0)
            {
                otherTroops.Add(PartyBase.MainParty.MemberRoster);
                lancesTroopRoster.Add(otherTroops);
                return lancesTroopRoster;
            }
            lancesTroopRoster.Add(otherTroops);
            var list = PartyBase.MainParty.Lances();
            for (int i = 0; i < list.Count; i++)
                lancesTroopRoster.Add(TroopRoster.CreateDummyTroopRoster());

            foreach (var troop in PartyBase.MainParty.MemberRoster.GetTroopRoster())
            {
                var character = troop.Character;
                int total = troop.Number;
                int woundedRemaining = troop.WoundedNumber;
                var savedLances = PartyBase.MainParty.Lances();
                for (int i = 0; i < savedLances.Count; i++)
                {
                    var savedLance = savedLances[i];
                    var lanceTroops = savedLance.LanceRoster;
                    if (total <= 0)
                        break;
                    int inLance = lanceTroops.GetTroopCount(character);
                    if (inLance <= 0)
                        continue;

                    var rosterForLanceVM = lancesTroopRoster[i + 1];
                    rosterForLanceVM.AddToCounts(character, inLance);
                    rosterForLanceVM.AddXpToTroop(character, troop.Xp);
                    int woundedHere = Math.Min(inLance, woundedRemaining);
                    if (woundedHere > 0)
                    {
                        rosterForLanceVM.WoundTroop(character, woundedHere);
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
            return lancesTroopRoster;
        }

        public new void ExecuteReset()
        {
            base.ExecuteReset();
            _disbandedRosters = new();
            _lancesTroopRosters = CreateDisplayPartyLances();
            PartyLances = CreatePartyLances();
            for (int i = 0; i < PartyLances.Count; i++)
            {
                LanceVM? lance = PartyLances[i];
                foreach (var troop in lance.LanceTroops)
                    if (!troop.Character.IsHero)
                        InitializePartyCharacterVM(troop, _lancesTroopRosters[i], _lancesTroopRosters[i].GetElementNumber(_lancesTroopRosters[i].FindIndexOfTroop(troop.Character)), false);
            }
            var otherTroopRoster = PartyScreenLogic.MemberRosters[0];
            foreach (var troop in OtherPartyTroops)
                if (!troop.Character.IsHero)
                    InitializePartyCharacterVM(troop, otherTroopRoster, otherTroopRoster.GetElementNumber(otherTroopRoster.FindIndexOfTroop(troop.Character)), true);
            ResetRosterData(PartyScreenLogic.MemberRosters[0], _originalOtherSideTroopRoster);
            ResetRosterData(PartyScreenLogic.MemberRosters[1], _originalPlayerSideTroopRoster);
        }
        public new void ExecuteRecruit(PartyCharacterVM troop, bool recruitAll = false)
        {
            base.ExecuteRecruit(troop, recruitAll);
            int numberToUpgrade = 1;
            if (IsEntireStackModifierActive || recruitAll)
                numberToUpgrade = troop.Troop.Number;
            if (IsFiveStackModifierActive)
                numberToUpgrade = MathF.Min(troop.Troop.Number, 5);
            AddPlayerSideTroop(troop.Character, _lancesTroopRosters[0], PartyLances[0].LanceTroops, numberToUpgrade);
        }
        public new void ExecuteUpgrade(PartyCharacterVM troop, int upgradeTargetType, int maxUpgradeCount)
        {
            base.ExecuteUpgrade(troop, upgradeTargetType, maxUpgradeCount);
            int numberToUpgrade = 1;
            if (IsEntireStackModifierActive)
                numberToUpgrade = maxUpgradeCount;
            if (IsFiveStackModifierActive)
                numberToUpgrade = MathF.Min(maxUpgradeCount, 5);
            bool isUpgradingFromTopPanel = true;

            var upgradedCharacter = troop.Character.UpgradeTargets[upgradeTargetType];
            int xpUsed = troop.Character.GetUpgradeXpCost(PartyBase.MainParty, upgradeTargetType) * numberToUpgrade;
            for (int i = 0; i < PartyLances.Count; i++)
            {
                LanceVM? lance = PartyLances[i];
                foreach (var lanceTroop in lance.LanceTroops)
                {
                    if (lanceTroop == troop)
                    {
                        isUpgradingFromTopPanel = false;
                        RemovePlayerSideTroops(troop.Character, _lancesTroopRosters[i], PartyLances[i].LanceTroops, numberToUpgrade);
                        AddPlayerSideTroop(upgradedCharacter, _lancesTroopRosters[i], PartyLances[i].LanceTroops, numberToUpgrade);
                        break;
                    }
                    else if (lanceTroop.Character == troop.Character)
                    {
                        lanceTroop.NumOfUpgradeableTroops -= numberToUpgrade;
                        var newTroop = lanceTroop.Troop;
                        newTroop.Xp -= xpUsed;
                        lanceTroop.Troop = newTroop;
                        if (lanceTroop.NumOfUpgradeableTroops == 0) lanceTroop.IsTroopUpgradable = false;
                    }
                }
            }
            if (isUpgradingFromTopPanel)
            {
                for (int i = 0; i < _lancesTroopRosters.Count; i++)
                {
                    if (numberToUpgrade == 0) break;
                    TroopRoster? lance = _lancesTroopRosters[i];
                    var troopAmountInLance = lance.GetTroopCount(troop.Character);
                    if (troopAmountInLance != 0)
                    {
                        var toUpgrade = Math.Min(numberToUpgrade, troopAmountInLance);
                        RemovePlayerSideTroops(troop.Character, _lancesTroopRosters[i], PartyLances[i].LanceTroops, toUpgrade);
                        AddPlayerSideTroop(upgradedCharacter, _lancesTroopRosters[i], PartyLances[i].LanceTroops, toUpgrade);
                        numberToUpgrade -= toUpgrade;
                    }
                }
            }
            UpdateTroopManagerPopUpCounts();
        }
        private void UpdateTroopManagerPopUpCounts()
        {
            _lanceUpgradePopUp.OnTroopUpgraded();

            this.UpgradableTroopCount = 0;
            MainPartyTroops.ApplyActionOnAllItems(delegate (PartyCharacterVM x)
            {
                this.UpgradableTroopCount += x.NumOfUpgradeableTroops;
            });
            var upgradeableTroops = new Dictionary<string, int>();
            foreach (var lance in PartyLances)
                foreach (PartyCharacterVM troop in lance.LanceTroops)
                {
                    if (troop.NumOfUpgradeableTroops != 0)
                    {
                        if (upgradeableTroops.TryGetValue(troop.StringId, out int value))
                            upgradeableTroops[troop.StringId] = Math.Max(value, troop.NumOfUpgradeableTroops);
                        upgradeableTroops[troop.StringId] = troop.NumOfUpgradeableTroops;
                    }
                }
            UpgradableTroopCount = upgradeableTroops.Values.Sum();
            this.IsUpgradePopUpDisabled = (!this.AreMembersRelevantOnCurrentMode 
                || this.UpgradableTroopCount == 0 
                || this.PartyScreenLogic.IsTroopUpgradesDisabled);
            this.UpgradePopUp.UpdateOpenButtonHint(this.IsUpgradePopUpDisabled, 
                !this.AreMembersRelevantOnCurrentMode, 
                this.PartyScreenLogic.IsTroopUpgradesDisabled);
        }
        public new void ExecuteTransferAllOtherTroops()
        {
            //var savedRoster = PartyScreenLogic.MemberRosters[0].CloneRosterData();
            foreach (var troop in OtherPartyTroops)
                ExecuteTransferFromOtherToPlayerSide(troop, troop.Number);
            base.ExecuteTransferAllOtherTroops();
            //UpdateOtherTroop(troop.Character, _lancesTroopRosters[0], PartyLances[0].LanceTroops, troop.Number, true);
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
                    PartyCharacterVM partyCharacterVM = new(this.PartyScreenLogic, this, currentTroopRoster, currentTroopRoster.FindIndexOfTroop(troopRosterElement.Character), type, (PartyScreenLogic.PartyRosterSide)side, this.PartyScreenLogic.IsTroopTransferable(type, troopRosterElement.Character, side));
                    partyList.Add(partyCharacterVM);
                    partyCharacterVM.ThrowOnPropertyChanged();
                    partyCharacterVM.IsLocked = false;
                }
            }
        }
        private MBBindingList<LanceVM> CreatePartyLances()
        {
            var lanceData = PartyBase.MainParty.Lances();
            MBBindingList<LanceVM> partyLances = new() { new LanceVM(this, 0, "main_troops", new(), "Your Retinue ({CURRENT_TROOPS}/{MAX_TROOPS}), Lances ({CURRENT_LANCES}/{MAX_LANCES})", Campaign.Current.Models.LanceModel().GetRetinueSizeLimit(PartyBase.MainParty).RoundedResultNumber) };
            InitializePartyList(partyLances[0].LanceTroops, _lancesTroopRosters[0], PartyScreenLogic.TroopType.Member, 1);
            for (int i = 1; i < _lancesTroopRosters.Count; i++)
            {
                TroopRoster lance = _lancesTroopRosters[i];
                var lanceName = lanceData[i-1].Name;
                var lanceSize = lanceData[i - 1].TotalManCount;
                partyLances.Add(new(this, i, "lance_" + i, new(), lanceName + " ({CURRENT_TROOPS}/{MAX_TROOPS})", lanceSize));
                InitializePartyList(partyLances.Last().LanceTroops, lance, PartyScreenLogic.TroopType.Member, 1);
            }
            return partyLances;
        }
        MBBindingList<LanceVM> _partyLances;
        [DataSourceProperty]
        public MBBindingList<LanceVM> PartyLances
        {
            get
            {
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
        public void ExecuteSelectLanceCharacterTuple(PartyCharacterVM troop)
        {
            foreach(var lance in PartyLances)
            {
                foreach(PartyCharacterVM lanceTroop in lance.LanceTroops)
                {
                    if (lanceTroop == troop)
                        lanceTroop.IsSelected = true;
                    else
                        lanceTroop.IsSelected = false;
                }
            }
        }
        public void OnDone()
        {
            var playerLances = PartyBase.MainParty.Lances();
            for (int i = 0; i < playerLances.Count; i++)
            {
                var lance = playerLances[i];
                if (_disbandedRosters.TryGetValue(i+1, out var removedRoster))
                    lance.LanceRoster = removedRoster;
                else
                    lance.LanceRoster = _lancesTroopRosters[i+1];
                //lance.LanceRoster.Count
                //lance.LanceRoster.SetElementWoundedNumber
            }
            //for (int lanceNumber = 1; lanceNumber < _lancesTroopRosters.Count; lanceNumber++)
            //{
            //    TroopRoster lance = _lancesTroopRosters[lanceNumber];
            //    playerLances[lanceNumber -1].LanceRoster = lance;
            //}
            var behavior = Campaign.Current.GetCampaignBehavior<LancesCampaignBehavior>() ?? throw new InvalidOperationException("LancesCampaignBehavior not found in campaign behaviors.");
            for (int i = PartyLances.Count - 1; i >= 1; i--)
            {
                LanceVM? lance = PartyLances[i];
                if (!lance.IsNotDisbanded)
                    behavior.DisbandLanceInParty(PartyBase.MainParty, i - 1, false);
            }
        }
        public override void OnFinalize()
        {
            Instance = null;
            base.OnFinalize();
            //foreach (var lance in PartyBase.MainParty.Lances())
            //{
            //    for (int i = 0; i < lance.LanceRoster.Count; i++)
            //        lance.LanceRoster.SetElementWoundedNumber(i, 0);
            //}
        }
        readonly FieldInfo _thisStock = AccessTools.Field("PartyTradeVM:_thisStock");
        private void RefreshPartyCharacter(PartyCharacterVM troop, TroopRoster rosterBelongedTo)
        {
            var tradeData = troop.TradeData;
            rosterBelongedTo.SetElementNumber(rosterBelongedTo.FindIndexOfTroop(troop.Character), troop.TradeData.ThisStock);
            troop.OnPropertyChanged("TroopNum");
            tradeData.OnPropertyChanged("InitialThisStock");
            tradeData.OnPropertyChanged("ThisStock");
            tradeData.OnPropertyChanged("InitialOtherStock");
            tradeData.OnPropertyChanged("OtherStock");
        }
        private void AddPlayerSideTroop(CharacterObject troop, TroopRoster roster, MBBindingList<PartyCharacterVM> characterVMs, int addedAmount)
        {
            var troopVM = characterVMs.FirstOrDefault(vm => vm.Character == troop);
            roster.AddToCounts(troop, addedAmount);
            if (troopVM == null) 
            {
                PartyCharacterVM characterVM = new(PartyScreenLogic, this, roster, characterVMs.Count, TroopType.Member, PartyRosterSide.Right, true);
                InitializePartyCharacterVM(characterVM, roster, addedAmount, false);
                characterVMs.Add(characterVM);
            }
            else
            {
                var tradeData = troopVM.TradeData;
                var newAmount = tradeData.ThisStock + addedAmount;
                var rosterElement = troopVM.Troop;
                rosterElement.Number = newAmount;
                troopVM.Troop = rosterElement;
                _thisStock.SetValue(tradeData, newAmount);
                tradeData.InitialThisStock = tradeData.ThisStock;
                tradeData.TotalStock = tradeData.InitialOtherStock + tradeData.ThisStock;
                RefreshPartyCharacter(troopVM, roster);
            }
        }
        private void RemovePlayerSideTroops(CharacterObject troop, TroopRoster roster, MBBindingList<PartyCharacterVM> characterVMs, int removedAmount)
        {
            var troopVM = characterVMs.First(vm => vm.Character == troop);
            var index = roster.FindIndexOfTroop(troop);
            if (roster.GetElementCopyAtIndex(index).Number == removedAmount)
            {
                characterVMs.Remove(troopVM);
                roster.AddToCountsAtIndex(index, -removedAmount);
            }
            else
            {
                var tradeData = troopVM.TradeData;
                var newAmount = tradeData.ThisStock - removedAmount;
                var rosterElement = troopVM.Troop;
                rosterElement.Number = newAmount;
                troopVM.Troop = rosterElement;
                _thisStock.SetValue(tradeData, newAmount);
                tradeData.InitialThisStock = tradeData.ThisStock;
                tradeData.TotalStock = tradeData.InitialOtherStock + tradeData.ThisStock;
                RefreshPartyCharacter(troopVM, roster);
            }
        }
        private void UpdateOtherTroop(CharacterObject troop, TroopRoster roster, MBBindingList<PartyCharacterVM> characterVMs, int transferAmount, bool isAddition)
        {
            var index = roster.FindIndexOfTroop(troop);
            if (isAddition)
            {
                PartyCharacterVM? vm = characterVMs.FirstOrDefault(p => p.Character == troop);
                if (vm == null)
                {
                    vm = new(PartyScreenLogic, this, roster, roster.FindIndexOfTroop(troop), TroopType.Member, PartyRosterSide.Right, true);
                    characterVMs.Add(vm);
                }
                var tradeData = vm.TradeData;
                if (tradeData.ThisStock == -1)
                {
                    InitializePartyCharacterVM(vm, roster, transferAmount, true);
                }
                else
                {
                    if (tradeData.ThisStock != roster.GetElementNumber(index))
                    {
                        var newAmount = tradeData.ThisStock + transferAmount;
                        var rosterData = vm.Troop;
                        rosterData.Number = newAmount;
                        vm.Troop = rosterData;
                        _thisStock.SetValue(tradeData, newAmount);
                    }
                    tradeData.InitialThisStock = tradeData.ThisStock;
                    tradeData.InitialOtherStock -= transferAmount;
                    tradeData.OtherStock = tradeData.InitialOtherStock;
                    tradeData.TotalStock = tradeData.InitialOtherStock + tradeData.ThisStock;
                    RefreshPartyCharacter(vm, roster);
                }
            }
            else
            {
                PartyCharacterVM? vm = characterVMs.FirstOrDefault(p => p.Character == troop);
                if (vm != null)
                {
                    var tradeData = vm.TradeData;
                    var newAmount = tradeData.InitialThisStock - transferAmount;
                    var rosterData = vm.Troop;
                    rosterData.Number = newAmount;
                    vm.Troop = rosterData;
                    _thisStock.SetValue(tradeData, newAmount);
                    tradeData.InitialThisStock = tradeData.ThisStock;
                    tradeData.InitialOtherStock += transferAmount;
                    tradeData.OtherStock = tradeData.InitialOtherStock;
                    tradeData.TotalStock = tradeData.InitialOtherStock + tradeData.ThisStock;
                    RefreshPartyCharacter(vm, roster);
                }
            }
        }
        private void ExecuteTransferFromPlayerToOtherSide(PartyCharacterVM troop, int transferAmount)
        {
            bool clickedTroopIconFromPartyTroops = false;
            for (int lanceIndex = 0; lanceIndex < PartyLances.Count; lanceIndex++)
            {
                LanceVM lance = PartyLances[lanceIndex];
                for (int troopIndex = 0; troopIndex < lance.LanceTroops.Count; troopIndex++)
                {
                    PartyCharacterVM? lanceTroop = lance.LanceTroops[troopIndex];
                    if (lanceTroop.Character == troop.Character)
                    {
                        var tradeData = lanceTroop.TradeData;
                        if (lanceTroop == troop)
                        {
                            clickedTroopIconFromPartyTroops = true;
                            if (transferAmount == troop.Troop.Number)
                            {
                                _lancesTroopRosters[lanceIndex].RemoveTroop(lanceTroop.Character, transferAmount);
                                PartyLances[lanceIndex].LanceTroops.Remove(lanceTroop);
                            }
                            else
                            {
                                var newAmount = tradeData.InitialThisStock - transferAmount;
                                var rosterData = lanceTroop.Troop;
                                rosterData.Number = newAmount;
                                lanceTroop.Troop = rosterData;
                                _thisStock.SetValue(tradeData, newAmount);
                                tradeData.InitialThisStock = tradeData.ThisStock;
                                tradeData.InitialOtherStock += transferAmount;
                                tradeData.OtherStock = tradeData.InitialOtherStock;
                                tradeData.TotalStock = tradeData.InitialOtherStock + tradeData.ThisStock;
                                RefreshPartyCharacter(lanceTroop, _lancesTroopRosters[lanceIndex]);
                            }
                        }
                        else
                        {
                            tradeData.InitialOtherStock += transferAmount;
                            tradeData.OtherStock = tradeData.InitialOtherStock;
                            tradeData.TotalStock = tradeData.InitialOtherStock + tradeData.ThisStock;
                            RefreshPartyCharacter(lanceTroop, _lancesTroopRosters[lanceIndex]);
                        }
                    }
                }
            }
            if (!clickedTroopIconFromPartyTroops)
            {
                for (int i = 0; i < _lancesTroopRosters.Count; i++)
                {
                    if (transferAmount == 0) break;
                    TroopRoster? lance = _lancesTroopRosters[i];
                    var troopAmountInLance = lance.GetTroopCount(troop.Character);
                    if (troopAmountInLance != 0)
                    {
                        var toRemove = Math.Min(transferAmount, troopAmountInLance);
                        RemovePlayerSideTroops(troop.Character, _lancesTroopRosters[i], PartyLances[i].LanceTroops, toRemove);
                        transferAmount -= toRemove;
                    }
                }
            }
        }
        private void ExecuteTransferFromOtherToPlayerSide(PartyCharacterVM troop, int transferAmount)
        {
            bool retinueContainsTroop = false;
            for (int i = PartyLances[0].LanceTroops.Count - 1; i >= 0; i--)
            {
                PartyCharacterVM? retinueTroop = PartyLances[0].LanceTroops[i];
                if (retinueTroop.Character == troop.Character)
                {
                    var tradeData = retinueTroop.TradeData; 
                    var rosterData = retinueTroop.Troop;
                    rosterData.Number += transferAmount;
                    retinueTroop.Troop = rosterData;
                    if (troop != retinueTroop)
                    {
                        var newAmount = tradeData.ThisStock + transferAmount;
                        _thisStock.SetValue(tradeData, newAmount);
                    }
                    tradeData.InitialThisStock = tradeData.ThisStock;
                    tradeData.InitialOtherStock -= transferAmount;
                    tradeData.OtherStock = tradeData.InitialOtherStock;
                    tradeData.TotalStock = tradeData.InitialOtherStock + tradeData.ThisStock;

                    retinueContainsTroop = true;
                    RefreshPartyCharacter(retinueTroop, _lancesTroopRosters[0]);
                }
            }
            if (!retinueContainsTroop)
            {
                var iindex = _lancesTroopRosters[0].AddToCounts(troop.Character, transferAmount);
                var newVM = new PartyCharacterVM(PartyScreenLogic, this, _lancesTroopRosters[0], iindex, PartyScreenLogic.TroopType.Member, PartyRosterSide.Right, true);
                InitializePartyCharacterVM(newVM, _lancesTroopRosters[0], transferAmount, false);
                PartyLances[0].LanceTroops.Insert(PartyLances[0].LanceTroops.Count, newVM);
            }
            for (int i = 1; i < PartyLances.Count; i++)
            {
                foreach(var lanceTroop in PartyLances[i].LanceTroops)
                {
                    if (lanceTroop.Character == troop.Character)
                    {
                        var tradeData = lanceTroop.TradeData;
                        if (lanceTroop == troop)
                        {
                            var newAmount = tradeData.ThisStock - transferAmount;
                            _thisStock.SetValue(tradeData, newAmount);
                        }
                        tradeData.InitialOtherStock -= transferAmount;
                        tradeData.OtherStock = tradeData.InitialOtherStock;
                        tradeData.TotalStock = tradeData.InitialOtherStock + tradeData.ThisStock;
                        RefreshPartyCharacter(lanceTroop, _lancesTroopRosters[i]);
                    }
                }
            }
        }
        int transfers = 0;
        private void InitializePartyCharacterVM(PartyCharacterVM vm, TroopRoster roster, int newAmount, bool leftSide)
        {
            var tradeData = vm.TradeData;
            if (leftSide)
            {
                CharacterObject troop = vm.Character;
                var totalAmount = 0;
                foreach (var lance in PartyLances)
                {
                    foreach (var lanceTroop in lance.LanceTroops)
                    {
                        if (lanceTroop.Character == troop)
                        {
                            totalAmount += lanceTroop.Number;
                        }
                    }
                }
                tradeData.InitialOtherStock = totalAmount;
                tradeData.OtherStock = totalAmount;
            }
            else
            {
                var otherTroop = OtherPartyTroops?.FirstOrDefault(c => c.Character == vm.Character);
                var otherNumber = otherTroop == null? 0 : otherTroop.Number;
                tradeData.InitialOtherStock = otherNumber;
                tradeData.OtherStock = otherNumber;
            }
            _thisStock.SetValue(tradeData, newAmount);
            tradeData.InitialThisStock = newAmount;
            tradeData.TotalStock = tradeData.InitialOtherStock + tradeData.ThisStock;
            RefreshPartyCharacter(vm, roster);
        }
        private void OnTransferTroop(PartyCharacterVM troop, int newIndex, int transferAmount, PartyRosterSide fromSide)
        {
            if (troop.IsPrisoner) return;
            transfers++;
            InformationManager.DisplayMessage(new($"{transferAmount} from {fromSide}. transfers: {transfers}"));
            if (fromSide == PartyRosterSide.Left)
            {
                ExecuteTransferFromOtherToPlayerSide(troop, transferAmount);
                UpdateOtherTroop(troop.Character, PartyScreenLogic.MemberRosters[0], OtherPartyTroops, transferAmount, false);
            }
            else
            {
                ExecuteTransferFromPlayerToOtherSide(troop, transferAmount);
                UpdateOtherTroop(troop.Character, PartyScreenLogic.MemberRosters[0], OtherPartyTroops, transferAmount, true);
            }
            foreach (var lance in PartyLances)
                lance.OnPropertyChanged("Text");
        }
        private Dictionary<int, TroopRoster> _disbandedRosters = new();
        public void OnLanceDisbanded(LanceVM lanceVM)
        {
            for (int i = 0; i < PartyLances.Count; i++)
            {
                LanceVM? lance = PartyLances[i];
                if (lance == lanceVM)
                {
                    foreach (var troop in _lancesTroopRosters[i].GetTroopRoster())
                    {
                        var index = PartyScreenLogic.MemberRosters[1].FindIndexOfTroop(troop.Character);
                        PartyScreenLogic.MemberRosters[1].AddToCountsAtIndex(index, -troop.Number, -troop.WoundedNumber);
                    }
                    foreach (var troopVM in OtherPartyTroops)
                        InitializePartyCharacterVM(troopVM, PartyScreenLogic.MemberRosters[0], troopVM.Troop.Number, true);
                    _disbandedRosters[i] = _lancesTroopRosters[i];
                    _lancesTroopRosters[i] = TroopRoster.CreateDummyTroopRoster();
                }
            }
        }
    }
}