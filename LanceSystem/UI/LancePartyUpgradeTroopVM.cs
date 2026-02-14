using System;
using TaleWorlds.CampaignSystem.ViewModelCollection.Party;
using TaleWorlds.CampaignSystem.ViewModelCollection.Party.PartyTroopManagerPopUp;
using TaleWorlds.Library;

namespace LanceSystem.UI
{
    public class LancePartyUpgradeTroopVM : PartyUpgradeTroopVM
    {
        readonly LancePartyVM _lancePartyVM;
        public LancePartyUpgradeTroopVM(LancePartyVM partyVM) : base(partyVM)
        {
            _lancePartyVM = partyVM;
        }
        public new void OnTroopUpgraded()
        {
            base.OnTroopUpgraded();
            foreach (var item in Troops)
            {
                item.PartyCharacter.InitializeUpgrades();
            }
        }
        public override void OpenPopUp()
        {
            base.OpenPopUp();
            PopulateTroops();
        }
        protected override void ConfirmCancel()
        {
            _lancePartyVM.ExecuteReset();
            base.ConfirmCancel();
        }
        private void PopulateTroops()
        {
            Troops = new MBBindingList<PartyTroopManagerItemVM>();
            var disabledTroopsStartIndex = 0;
            foreach (var lance in _lancePartyVM.PartyLances)
            {
                foreach (PartyCharacterVM partyCharacterVM in lance.LanceTroops)
                {
                    if (partyCharacterVM.IsTroopUpgradable)
                    {
                        Troops.Insert(disabledTroopsStartIndex, new PartyTroopManagerItemVM(partyCharacterVM, new Action<PartyTroopManagerItemVM>(SetFocusedCharacter)));
                        disabledTroopsStartIndex++;
                    }
                    else if (partyCharacterVM.NumOfReadyToUpgradeTroops > 0)
                    {
                        Troops.Add(new PartyTroopManagerItemVM(partyCharacterVM, new Action<PartyTroopManagerItemVM>(SetFocusedCharacter)));
                    }
                }
            }
        }
    }
}