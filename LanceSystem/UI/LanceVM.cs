using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.ViewModelCollection.Party;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace LanceSystem.UI
{
    public class LanceVM : ViewModel
    {
        string _name;
        readonly string _text;
        readonly int _maxTroops; 
        public int LanceNumber { get; private set; }
        MBBindingList<PartyCharacterVM> _lanceTroops;
        BasicTooltipViewModel _disbandLanceHint;
        LancePartyVM _partyVM;
        public LanceVM(LancePartyVM partyVM, int lanceNumber, string name, MBBindingList<PartyCharacterVM> mainPartyTroops, string text, int maxTroops)
        {
            _partyVM = partyVM;
            LanceNumber = lanceNumber;
            _name = name;
            _lanceTroops = mainPartyTroops;
            _disbandLanceHint = new BasicTooltipViewModel(delegate()
            {
                GameTexts.SetVariable("TEXT", new TextObject("Disband Lance", null));
                GameTexts.SetVariable("HOTKEY", "");
                return GameTexts.FindText("str_hotkey_with_hint", null).ToString();
            });
            _text = text;
            _maxTroops = maxTroops;
        }
        public void ExecuteDisbandLance()
        {
            if (IsNotDisbanded) //to allow for debugging with breakpoints
            {
                _partyVM.OnLanceDisbanded(this);
                LanceTroops = new();
            }
            IsNotDisbanded = false;
        }
        public override void RefreshValues()
        {
            base.RefreshValues();
            _lanceTroops?.ApplyActionOnAllItems(delegate (PartyCharacterVM x)
            {
                x.RefreshValues();
            });
        }

        [DataSourceProperty]
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                if (value != _name)
                {
                    _name = value;
                    OnPropertyChangedWithValue(value, "Name");
                }
            }
        }
        [DataSourceProperty]
        public MBBindingList<PartyCharacterVM> LanceTroops
        {
            get
            {
                return _lanceTroops;
            }
            set
            {
                if (value != _lanceTroops)
                {
                    _lanceTroops = value;
                    OnPropertyChangedWithValue(value, "LanceTroops");
                }
            }
        }
        
        [DataSourceProperty]
        public string Text
        {
            get
            {
                int currentTroops = 0;
                foreach (var troop in LanceTroops)
                    currentTroops += troop.Number;
                var fullText = new TextObject(_text);
                GameTexts.SetVariable("CURRENT_TROOPS", currentTroops);
                GameTexts.SetVariable("MAX_TROOPS", _maxTroops);
                GameTexts.SetVariable("CURRENT_LANCES", PartyBase.MainParty.Lances().Count);
                GameTexts.SetVariable("MAX_LANCES", Campaign.Current.Models.LanceModel().MaxLancesForParty(PartyBase.MainParty).RoundedResultNumber);
                return fullText.ToString();
            }
        }
        [DataSourceProperty]
        public BasicTooltipViewModel DisbandLanceHint
        {
            get
            {
                return _disbandLanceHint;
            }
        }
        bool _isNotDisbanded = true;
        [DataSourceProperty]
        public bool IsNotDisbanded
        {
            get
            {
                return _isNotDisbanded;
            }
            set
            {
                if (value != _isNotDisbanded)
                {
                    _isNotDisbanded = value;
                    OnPropertyChangedWithValue(value, "IsNotDisbanded");
                }
            }
        }
        public bool CanDisbandLance
        {
            get
            {
                return LanceNumber != 0;
            }
        }
    }
}
