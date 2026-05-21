using System;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterCreationContent;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.GauntletUI.BodyGenerator;

namespace BloodAndBittersteel.Features.CharacterSelection.ViewModel;

public class CharacterSelectionViewModel : TaleWorlds.Library.ViewModel
{
    private MBBindingList<CharacterSelectionItemViewModel> _kingdomMembers;
    private MBBindingList<CharacterSelectionItemViewModel> _clanMembers;
    private MBBindingList<CharacterSelectionItemViewModel> _wanderers;
    private BodyGeneratorView _generatorView;
    private CharacterSelectionItemViewModel _lastSelectedItem;
    private static Clan _defaultClan;
    private static CultureObject _culture;
    private static CharacterSelectionViewModel _currentInstance;

    public static bool IsPreBuildHero => PreBuildHero != null;
    public static Hero PreBuildHero { get; private set; } = null;
    public static CampaignTime OriginalHeroBirthday { get; private set; }

    [DataSourceProperty]
    public bool HasSelectedItem => _lastSelectedItem != null;

    [DataSourceProperty]
    public String StartWithCurrentText => "Advance with Hero";

    [DataSourceProperty]
    public string KingdomMembersText => new TextObject("Kingdom Members").ToString();

    [DataSourceProperty]
    public string ClanMembersText => new TextObject("Clan Leaders").ToString();

    [DataSourceProperty]
    public string CompanionsText => new TextObject("Companions").ToString();

    [DataSourceProperty]
    public MBBindingList<CharacterSelectionItemViewModel> KingdomMembers
    {
        get => _kingdomMembers;
        set
        {
            if (value == _kingdomMembers)
                return;
            _kingdomMembers = value;
            OnPropertyChanged(nameof(KingdomMembers));
        }
    }

    [DataSourceProperty]
    public MBBindingList<CharacterSelectionItemViewModel> ClanLeaders
    {
        get => _clanMembers;
        set
        {
            if (value == _clanMembers)
                return;
            _clanMembers = value;
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
        _culture = (GameStateManager.Current.ActiveState as CharacterCreationState).CharacterCreationManager.CharacterCreationContent.SelectedCulture;

        _defaultClan = Campaign.Current.MainParty.LeaderHero.Clan;
        _defaultClan.Culture = _culture;

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

        CharacterSelectionService.Instance.UpdateBodyProperties(PreBuildHero, _generatorView);
        OnPropertyChanged("HasSelectedItem");
    }

    private void InitializeLists()
    {
        _kingdomMembers ??= new MBBindingList<CharacterSelectionItemViewModel>();
        _clanMembers ??= new MBBindingList<CharacterSelectionItemViewModel>();
        _wanderers ??= new MBBindingList<CharacterSelectionItemViewModel>();

        _kingdomMembers.Clear();
        _clanMembers.Clear();
        _wanderers.Clear();
    }

    private void PopulateLists(CultureObject culture)
    {
        var kingdomMembers = CharacterSelectionService.Instance.GetKingdomMembers(culture);
        var clanLeaders = CharacterSelectionService.Instance.GetClanLeaders(culture, kingdomMembers);
        var wanderers = CharacterSelectionService.Instance.GetWanderers(culture);

        kingdomMembers.ForEach(h => _kingdomMembers.Add(new CharacterSelectionItemViewModel(h, OnSelectedItem)));

        clanLeaders.ForEach(h => _clanMembers.Add(new CharacterSelectionItemViewModel(h, OnSelectedItem)));

        wanderers.ForEach(h => _wanderers.Add(new CharacterSelectionItemViewModel(h, OnSelectedItem)));
    }

    private void ResetInstance()
    {
        _lastSelectedItem = null;
        if (_generatorView != null)
            _generatorView.DataSource.CanChangeRace = true;
    }

    public static void Reset()
    {
        PreBuildHero = null;
        if (_currentInstance != null)
        {
            _currentInstance.ResetInstance();
            ClearHelperStatics();
        }
    }

    public static void ClearHelperStatics()
    {
        _currentInstance = null;
    }

    public static void ExcuteActionForPrebuildHero()
    {
        if (PreBuildHero != null)
        {
            OriginalHeroBirthday = PreBuildHero.BirthDay;
            CharacterSelectionService.Instance.ApplyPrebuildHeroConfiguration(PreBuildHero, OriginalHeroBirthday, _culture);
        }
    }

    public static void CheckSkillReset()
    {
        var service = CharacterSelectionService.Instance;

        InformationManager.ShowInquiry(new InquiryData(
            new TextObject("Skill reset decision").ToString(),
            "Do you wanna set your skills to default?",
            true,
            true,
            GameTexts.FindText("str_ok").ToString(),
            GameTexts.FindText("str_cancel").ToString(),
            () => service.ResetHeroSkills(Hero.MainHero),
            () => service.CheckHeroSkillIntegrity(Hero.MainHero)
        ), true, true);
    }
}
