using BloodAndBittersteel.Features.CharacterSelection.Helper;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterCreationContent;
using TaleWorlds.CampaignSystem.Extensions;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.GauntletUI.BodyGenerator;

namespace BloodAndBittersteel.Features.CharacterSelection;

public class PreMadeCharacterSelection
{
    private static PreMadeCharacterSelection? _instance;
    public static PreMadeCharacterSelection Instance { get { _instance ??= new PreMadeCharacterSelection(); return _instance; } }
    public List<Hero> GetClanMembers(CultureObject culture)
    {
        var kingdoms = Kingdom.All.Where(k => k.Culture.StringId == culture?.StringId && k.Leader != null);
        if (kingdoms == null || kingdoms.Count() == 0)
            return new List<Hero>();

        var clanMembers = kingdoms
            .SelectMany(kingdom => kingdom.Clans)
            .SelectMany(clan => clan.Heroes)
            .Where(hero => !hero.IsClanLeader && !hero.IsChild)
            .Distinct()
            .ToList();
        return clanMembers;
    }

    public List<Hero> GetClanLeaders(CultureObject culture)
    {
        var clanLeaders = Clan.All
            .Where(f => f.IsClan && f.Culture.StringId == culture?.StringId && f.Leader != null)
            .Select(f => f.Leader)
            .Distinct()
            .ToList();
        return clanLeaders;
    }

    public List<Hero> GetWanderers(CultureObject culture)
    {
        var activeWanderers = Campaign.Current.AliveHeroes
            .Where(h => h.Culture.StringId == culture?.StringId && h.IsWanderer && !h.IsChild)
            .Distinct()
            .ToList();

        var inactiveWanderers = Campaign.Current.DeadOrDisabledHeroes
            .Where(h => h.Culture.StringId == culture?.StringId && h.IsWanderer && !h.IsChild)
            .Distinct()
            .ToList();

        return activeWanderers.Union(inactiveWanderers).ToList();
    }

    public void UpdateBodyProperties(Hero selectedHero, object generatorView)
    {
        if (selectedHero == null || generatorView == null)
            return;

        if (generatorView is not BodyGeneratorView bodyGeneratorView)
            return;

        BodyProperties.FromString(selectedHero.BodyProperties.ToString(), out BodyProperties props);

        if (selectedHero.BattleEquipment != null || selectedHero.CivilianEquipment != null)
        {
            var equipment = selectedHero.BattleEquipment ?? selectedHero.CivilianEquipment;
            EquipmentElement[] itemSlots = ReflectionHelper.GetFieldValue<Equipment, EquipmentElement[]>(equipment, "_itemSlots");
            var bannerIndex = itemSlots.FindIndex(item => item.Item != null && item.Item.HasBannerComponent);

            if (bannerIndex > -1)
                itemSlots[bannerIndex].Clear();

            ReflectionHelper.SetFieldValue(bodyGeneratorView, "_dressedEquipment", equipment);
        }

        bodyGeneratorView.DataSource.SetBodyProperties(props, false, selectedHero.CharacterObject.Race);
        bodyGeneratorView.DataSource.SelectedGender = selectedHero.IsFemale ? 1 : 0;
        bodyGeneratorView.DataSource.CanChangeRace = false;
        bodyGeneratorView.IsDressed = true;
    }

    public void ApplyPrebuildHeroConfiguration(Hero prebuildHero, CampaignTime originalBirthday, CultureObject culture)
    {
        if (prebuildHero == null)
            return;
        var stages = CharacterCreationStagesHelper.GetStages();
        var state = (CharacterCreationState)GameStateManager.Current.ActiveState;
        int nextIndex = prebuildHero.Clan == null
            ? stages.FindIndex(s => s is CharacterCreationClanNamingStage) - 1
            : stages.FindIndex(s => s is CharacterCreationOptionsStage);

        var playerClan = MobileParty.MainParty?.LeaderHero?.Clan;
        var kingdom = playerClan != null
            ? Kingdom.All.FirstOrDefault(k => k.Culture.StringId == playerClan.Culture.StringId)
            : null;

        if (prebuildHero.Clan != null)
            ReflectionHelper.SetPropertyValue(Campaign.Current, "PlayerDefaultFaction", prebuildHero.Clan);
        JumpToNextStage(state, nextIndex, prebuildHero);
    }

    private static readonly int FocusPointsBonus = 4;
    private static readonly int AttributePointsBonus = 2;
    private static readonly int NewCharFocusPoints = 16;
    private static readonly int NewCharAttributePoints = 8;
    public void ResetHeroSkills(Hero hero)
    {

        if (hero?.HeroDeveloper == null)
            return;
        hero.HeroDeveloper.ClearHero();
        hero.HeroDeveloper.UnspentAttributePoints = NewCharAttributePoints + AttributePointsBonus;
        hero.HeroDeveloper.UnspentFocusPoints = NewCharFocusPoints + FocusPointsBonus;
        hero.HeroDeveloper.SetInitialLevel(1);

        //hero.SetTraitLevel(DefaultTraits.Generosity, traits.Generosity);
        //hero.SetTraitLevel(DefaultTraits.Valor, traits.Valor);
        //hero.SetTraitLevel(DefaultTraits.Calculating, traits.Calculating);
        //hero.SetTraitLevel(DefaultTraits.Mercy, traits.Mercy);
        //hero.SetTraitLevel(DefaultTraits.Honor, traits.Honor);

        foreach (var attribute in Attributes.All)
            hero.HeroDeveloper.AddAttribute(attribute, 2, false);
        foreach (var skill in Skills.All)
            hero.SetSkillValue(skill, MBRandom.RandomInt(0, 10));
    }
    private void JumpToNextStage(CharacterCreationState state, int nextIndex, Hero prebuildHero)
    {
        BodyProperties.FromString(prebuildHero.BodyProperties.ToString(), out BodyProperties props);
        ReflectionHelper.SetPropertyValue(Hero.MainHero, "StaticBodyProperties", props.StaticProperties);
        ReflectionHelper.SetPropertyValue(Hero.MainHero, "_battleEquipment", prebuildHero.BattleEquipment.Clone());
        ReflectionHelper.SetFieldValue(state.CharacterCreationManager, "_furthestStageIndex", nextIndex);
        state.CharacterCreationManager.GoToStage(nextIndex);
    }
}
