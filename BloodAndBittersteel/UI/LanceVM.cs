using SandBox.ViewModelCollection.Input;
using System;
using TaleWorlds.CampaignSystem.ViewModelCollection.Party;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace BloodAndBittersteel.UI
{
    public class LanceVM : ViewModel
    {
        string _name;
        string _text;
        string _listPanel;
        string _widgetToClose;
        MBBindingList<PartyCharacterVM> _mainPartyTroops;
        BasicTooltipViewModel _transferAllMainTroopsHint;
        public LanceVM(string name, MBBindingList<PartyCharacterVM> mainPartyTroops, string text)
        {
            _name = name;
            _mainPartyTroops = mainPartyTroops;
            _transferAllMainTroopsHint = new BasicTooltipViewModel(delegate ()
            {
                GameTexts.SetVariable("TEXT", new TextObject("{=9WrJP0hD}Transfer All Troops", null));
                GameTexts.SetVariable("HOTKEY", "TODO TEST");
                return GameTexts.FindText("str_hotkey_with_hint", null).ToString();
            }); ;
            _text = text;
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
                return _mainPartyTroops;
            }
            set
            {
                if (value != _mainPartyTroops)
                {
                    _mainPartyTroops = value;
                    base.OnPropertyChangedWithValue<MBBindingList<PartyCharacterVM>>(value, "LanceTroops");
                }
            }
        }
        [DataSourceProperty]
        public string Text
        {
            get
            {
                return _text;
            }
        }
        [DataSourceProperty]
        public string ListPanel
        {
            get
            {
                return _listPanel;
            }
        }
        [DataSourceProperty]
        public string WidgetToClose
        {
            get
            {
                return _widgetToClose;
            }
        }
        
        // TODO
        [DataSourceProperty]
        public InputKeyItemVM DismissAllTroopsInputKey
        {
            get
            {
                return null;
            }
        }
        [DataSourceProperty]
        public BasicTooltipViewModel TransferAllMainTroopsHint
        {
            get
            {
                return _transferAllMainTroopsHint;
            }
        }
    }
}
