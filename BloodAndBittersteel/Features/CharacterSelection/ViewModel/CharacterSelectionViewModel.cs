using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterCreationContent;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.GauntletUI.BodyGenerator;

namespace BloodAndBittersteel.Features.CharacterSelection.ViewModel;

public class CharacterSelectionViewModel : TaleWorlds.Library.ViewModel
{
    private MBBindingList<CharacterSelectionItemViewModel> _clanLeaders = new();
    private MBBindingList<CharacterSelectionItemViewModel> _clanMembers = new();
    private MBBindingList<CharacterSelectionItemViewModel> _wanderers = new();
    private CharacterSelectionItemViewModel? _lastSelectedItem;
    private readonly BodyGeneratorView _generatorView;
    private readonly CultureObject _culture;
    private static CharacterSelectionViewModel? _currentInstance;
    public static CharacterSelectionViewModel Instance { get => _currentInstance!; }

    private static string StartWithNewHero => new TextObject("{=bab_original_hero}Create Original Hero").ToString();
    private static string StartWithExistingHero => new TextObject("{=bab_existing_hero}Advance with Hero").ToString();


    [DataSourceProperty]
    public bool IsPreBuiltHero => PreBuildHero != null;
    Hero? _preBuiltHero;
    public Hero? PreBuildHero { get => _preBuiltHero; private set { _preBuiltHero = value; OnPropertyChanged(nameof(IsPreBuiltHero)); } }
    public CampaignTime OriginalHeroBirthday { get; private set; }

    [DataSourceProperty]
    public bool HasSelectedItem => _lastSelectedItem != null;

    [DataSourceProperty]
    public string CreateCharacterText => StartWithNewHero;

    [DataSourceProperty]
    public string ClanLeadersText => new TextObject("{=bab_clan_leaders}Clan Leaders").ToString();
    [DataSourceProperty]
    public string ClanMembersText => new TextObject("{=bab_clan_members}Clan Members").ToString();

    [DataSourceProperty]
    public string CompanionsText => new TextObject("{=bab_companions}Companions").ToString();

    [DataSourceProperty]
    public MBBindingList<CharacterSelectionItemViewModel> ClanMembers
    {
        get => _clanMembers;
        set
        {
            if (value == _clanMembers)
                return;
            _clanMembers = value;
            OnPropertyChanged(nameof(ClanMembers));
        }
    }

    [DataSourceProperty]
    public MBBindingList<CharacterSelectionItemViewModel> ClanLeaders
    {
        get => _clanLeaders;
        set
        {
            if (value == _clanLeaders)
                return;
            _clanLeaders = value;
            OnPropertyChanged(nameof(ClanLeaders));
        }
    }

    [DataSourceProperty]
    public MBBindingList<CharacterSelectionItemViewModel> Companions
    {
        get => _wanderers;
        set
        {
            if (value == _wanderers)
                return;
            _wanderers = value;
            OnPropertyChanged(nameof(Companions));
        }
    }

    public CharacterSelectionViewModel(BodyGeneratorView bodyGeneratorView)
    {
        _generatorView = bodyGeneratorView;
        _currentInstance = this;

        PreBuildHero = null;
        OriginalHeroBirthday = Hero.MainHero.BirthDay;
        _culture = (GameStateManager.Current.ActiveState as CharacterCreationState)!.CharacterCreationManager.CharacterCreationContent.SelectedCulture;

        _generatorView.DataSource.DoneBtnLbl = StartWithNewHero;
        InitializeLists();
        PopulateLists(_culture);
    }

    [DataSourceProperty]
    public void OnSelectedItem(CharacterSelectionItemViewModel item)
    {
        if (_lastSelectedItem != null)
        {
            _lastSelectedItem.IsSelected = false;
        }
        _lastSelectedItem = item;
        PreBuildHero = _lastSelectedItem.OriginalHero;

        PreMadeCharacterSelection.Instance.UpdateBodyProperties(PreBuildHero, _generatorView);
        OnPropertyChanged("HasSelectedItem");
        _generatorView.DataSource.DoneBtnLbl = StartWithExistingHero;
    }

    private void InitializeLists()
    {
        _clanMembers ??= new MBBindingList<CharacterSelectionItemViewModel>();
        _clanLeaders ??= new MBBindingList<CharacterSelectionItemViewModel>();
        _wanderers ??= new MBBindingList<CharacterSelectionItemViewModel>();

        _clanMembers.Clear();
        _clanLeaders.Clear();
        _wanderers.Clear();
    }

    private void PopulateLists(CultureObject culture)
    {
        var clanLeaders = PreMadeCharacterSelection.Instance.GetClanLeaders(culture);
        var clanMembers = PreMadeCharacterSelection.Instance.GetClanMembers(culture);
        var wanderers = PreMadeCharacterSelection.Instance.GetWanderers(culture);

        clanLeaders.ForEach(h => _clanLeaders.Add(new CharacterSelectionItemViewModel(h, OnSelectedItem)));
        clanMembers.ForEach(h => _clanMembers.Add(new CharacterSelectionItemViewModel(h, OnSelectedItem)));
        wanderers.ForEach(h => _wanderers.Add(new CharacterSelectionItemViewModel(h, OnSelectedItem)));
    }

    private void ResetInstance()
    {
        _lastSelectedItem = null;
    }

    public void Reset()
    {
        PreBuildHero = null;
        Instance.ResetInstance();
    }
    public static void RemoveInstance()
    {
        _currentInstance = null;
    }
    public void ExcuteActionForPrebuildHero()
    {
        if (PreBuildHero != null)
        {
            OriginalHeroBirthday = PreBuildHero.BirthDay;
            PreMadeCharacterSelection.Instance.ApplyPrebuildHeroConfiguration(PreBuildHero, OriginalHeroBirthday, _culture);
        }
    }
    public void OnResetCharacter()
    {
        if (_lastSelectedItem != null)
            _lastSelectedItem.IsSelected = false;
        Reset();
        _generatorView.DataSource.DoneBtnLbl = StartWithNewHero;
    }
}