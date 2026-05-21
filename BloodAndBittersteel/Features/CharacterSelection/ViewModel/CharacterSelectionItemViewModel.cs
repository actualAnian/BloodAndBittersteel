using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection.ClanManagement;
using TaleWorlds.Library;

namespace BloodAndBittersteel.Features.CharacterSelection.ViewModel;

public class CharacterSelectionItemViewModel: ClanPartyMemberItemVM
{
    private bool _isSelected;

    public readonly Action<CharacterSelectionItemViewModel> _onItemSelectedEvent;

    public Hero OriginalHero { get; private set; } = null;

    public CharacterSelectionItemViewModel(Hero hero, Action<CharacterSelectionItemViewModel> itemnselected) : base(hero, Campaign.Current.MainParty)
    {
        _onItemSelectedEvent = itemnselected;
        OriginalHero = hero;
    }

    public void OnPreBuildCharacterSelected()
    {
        bool flag = !this.IsSelected;
        if (flag)
        {
            this.IsSelected = true;
            this._onItemSelectedEvent(this);
        }
    }

    [DataSourceProperty]
    public bool IsSelected
    {
        get
        {
            return this._isSelected;
        }
        set
        {
            bool flag = value != this._isSelected;
            if (flag)
            {
                this._isSelected = value;
                base.OnPropertyChanged("IsSelected");
            }
        }
    }
}