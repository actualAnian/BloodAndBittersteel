using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem.ViewModelCollection.Party;
using TaleWorlds.CampaignSystem.ViewModelCollection.Party.PartyTroopManagerPopUp;

namespace LanceSystem.UI
{
    internal class LancePartyRecruitTroopVM : PartyRecruitTroopVM
    {
        LancePartyVM _lancePartyVM;
        public LancePartyRecruitTroopVM(LancePartyVM partyVM) : base(partyVM)
        {
            _lancePartyVM = partyVM;
        }
        public override void ExecuteItemPrimaryAction()
        {
            if (FocusedTroop == null)
                return;
            if (FocusedTroop.PartyCharacter.IsTroopRecruitable)
                _lancePartyVM.ExecuteRecruit(FocusedTroop.PartyCharacter, false);
        }
        public new void ExecuteRecruitAll()
        {
            for (int i = Troops.Count - 1; i >= 0; i--)
            {
                PartyCharacterVM partyCharacter = Troops[i].PartyCharacter;
                partyCharacter.RecruitAll();
                if (partyCharacter.IsTroopRecruitable)
                    _lancePartyVM.ExecuteRecruit(partyCharacter, true);
            }
        }
        protected override void ConfirmCancel()
        {
            _lancePartyVM.ExecuteReset();
            base.ConfirmCancel();
        }
    }
}
