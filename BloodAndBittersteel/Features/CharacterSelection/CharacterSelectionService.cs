using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterCreationContent;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.GauntletUI.BodyGenerator;
using TaleWorlds.CampaignSystem.Party;
using BloodAndBittersteel.Features.CharacterSelection.Helper;

namespace BloodAndBittersteel.Features.CharacterSelection;

public class CharacterSelectionService
{
    private static CharacterSelectionService? _instance;
    public static CharacterSelectionService Instance { get { _instance ??= new CharacterSelectionService(); return _instance; } }
    public List<Hero> GetKingdomMembers(CultureObject culture)
    {
        var kingdomLeader = Kingdom.All.FirstOrDefault(k => k.Culture.StringId == culture?.StringId && k.Leader != null)?.Leader;

        if (kingdomLeader == null)
            return new List<Hero>();

        var kingdomMembers = new List<Hero>(kingdomLeader.Children
            .Where(child => !child.IsChild)
            .Distinct());

        kingdomMembers.Insert(0, kingdomLeader);

        if (kingdomLeader.Spouse != null)
            kingdomMembers.Add(kingdomLeader.Spouse);

        kingdomMembers.RemoveAll(hero =>
            hero.Name.ToString().ToLower().Contains("place holder") ||
            hero.Name.ToString().ToLower().Contains("placeholder"));

        return kingdomMembers;
    }

    public List<Hero> GetClanLeaders(CultureObject culture, List<Hero> excludeHeroes)
    {
        if (excludeHeroes == null)
            throw new ArgumentNullException(nameof(excludeHeroes));

        var clanLeaders = Clan.All
            .Where(f => f.IsClan && f.Culture.StringId == culture?.StringId && f.Leader != null)
            .Select(f => f.Leader)
            .Distinct()
            .ToList();

        clanLeaders.RemoveAll(leader =>
            excludeHeroes.Contains(leader) ||
            leader.Name.Contains("Just To View"));

        return clanLeaders;
    }

    public List<Hero> GetWanderers(CultureObject culture)
    {
        var activeWanderers = Campaign.Current.AliveHeroes
            .Where(h => h.Culture.StringId == culture?.StringId && h.IsWanderer)
            .Distinct()
            .ToList();

        var inactiveWanderers = Campaign.Current.DeadOrDisabledHeroes
            .Where(h => h.Culture.StringId == culture?.StringId && h.IsWanderer)
            .Distinct()
            .ToList();

        return activeWanderers.Union(inactiveWanderers).ToList();
    }

    public void UpdateBodyProperties(Hero selectedHero, object generatorView)
    {
        if (selectedHero == null || generatorView == null)
            return;

        var bodyGeneratorView = generatorView as BodyGeneratorView;
        if (bodyGeneratorView == null)
            return;


        BodyProperties props;
        BodyProperties.FromString(selectedHero.BodyProperties.ToString(), out props);

        if (selectedHero.BattleEquipment != null || selectedHero.CivilianEquipment != null)
        {
            var equipment = selectedHero.BattleEquipment ?? selectedHero.CivilianEquipment;
            EquipmentElement[] itemSlots = ReflectionHelper.GetFieldValue<Equipment, EquipmentElement[]>(equipment, "_itemSlots");
            var bannerIndex = itemSlots.FindIndex(item => item.Item != null && item.Item.HasBannerComponent);

            if (bannerIndex > -1)
            {
                itemSlots[bannerIndex].Clear();
            }

            ReflectionHelper.SetFieldValue(bodyGeneratorView, "_dressedEquipment", equipment);
        }

        try
        {
            bodyGeneratorView.DataSource.SetBodyProperties(props, false, selectedHero.CharacterObject.Race);
        }
        catch (ArgumentOutOfRangeException)
        {
            Debug.PrintWarning("Voice could not be loaded for prebuild character");
        }

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
        //var settlement = kingdom?.InitialHomeLand;

        //ReflectionHelper.SetFieldValue(state, "_furthestStageIndex", nextIndex); //@TODO check

        if (prebuildHero.Clan != null)
        {
            //_campaignConfigurationAdapter.SetPlayerDefaultFaction(prebuildHero.Clan);
            ReflectionHelper.SetPropertyValue(Campaign.Current, "PlayerDefaultFaction", prebuildHero.Clan);
        }
        else
        {
            var mainParty = MobileParty.MainParty;
            var defaultClan = mainParty?.LeaderHero?.Clan;

            if (defaultClan != null)
            {
                var underlyingClan = defaultClan;

                if (culture != null)
                {
                    underlyingClan.Culture = culture;
                }
                ReflectionHelper.SetFieldValue(underlyingClan, "_banner", Hero.MainHero.ClanBanner);

                prebuildHero.Clan = underlyingClan;

                //if (settlement != null)
                //{
                //    // ADR-007: Unwrap immediately before accessing game API
                //    var underlyingSettlement = (settlement).GetUnderlyingSettlement();
                //    underlyingClan.InitialPosition = underlyingSettlement.GetPosition2D;
                //}
                ReflectionHelper.SetPropertyValue(Campaign.Current, "PlayerDefaultFaction", underlyingClan);
            }
        }
        JumpToNextStage(state, nextIndex, prebuildHero);
    }

    public void ResetHeroSkills(Hero hero)
    {
        //hero?.ResetSkillsWithBonus(); @TODO
    }

    public void CheckHeroSkillIntegrity(Hero hero)
    {
        //hero?.CheckSkillIntegrity(); @TODO
    }

    private void JumpToNextStage(CharacterCreationState state, int nextIndex, Hero prebuildHero)
    {
        BodyProperties props;
        BodyProperties.FromString(prebuildHero.BodyProperties.ToString(), out props);
        ReflectionHelper.SetPropertyValue(Hero.MainHero, "StaticBodyProperties", props.StaticProperties);
        ReflectionHelper.SetPropertyValue(Hero.MainHero, "_battleEquipment", prebuildHero.BattleEquipment.Clone());
        //ReflectionHelper.SetFieldValue(state, "_furthestStageIndex", nextIndex); //@TODO check
        ReflectionHelper.SetFieldValue(state.CharacterCreationManager, "_furthestStageIndex", nextIndex);
        state.CharacterCreationManager.GoToStage(nextIndex);
        //state.GoToStage(nextIndex); @TODO
    }
}
