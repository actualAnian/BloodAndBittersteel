using System;
using System.Collections.Generic;
using HarmonyLib;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party.PartyComponents;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.CharacterCreationContent;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.ViewModelCollection.CharacterCreation.OptionsStage;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using BloodAndBittersteel.Features.CharacterSelection.ViewModel;
using BloodAndBittersteel.Features.CharacterSelection.Helper;

namespace BloodAndBittersteel.Features.CharacterSelection.Patches;

[HarmonyPatch]
public static class CharacterCreationOptionsStageVMPatch
{
    private static Kingdom _kingdom;

    [HarmonyPostfix]
    [HarmonyPatch(typeof (CharacterCreationOptionsStageVM), "OnNextStage")]
    public static void Post_OnNextStage()
    {
        // Track if the selected prebuild hero was already in a kingdom (skip join prompt if so)
        bool prebuildHeroAlreadyInKingdom = false;

        if (CharacterSelectionViewModel.IsPreBuildHero)
        {
            CharacterSelectionViewModel.CheckSkillReset();
            Hero hero = CharacterSelectionViewModel.PreBuildHero;
            CharacterCreationOptionsStageVMPatch._kingdom = ((IEnumerable<Kingdom>) Campaign.Current.Kingdoms).FirstOrDefault<Kingdom>((Func<Kingdom, bool>) (item => item.Culture?.StringId == hero.Culture?.StringId));

            // Check if hero is already in a kingdom before making any changes
            prebuildHeroAlreadyInKingdom = hero.Clan?.Kingdom != null;

            // Store reference to original MainHero before any changes
            Hero originalMainHero = Hero.MainHero;
            Clan originalClan = originalMainHero?.Clan;

            if (hero.Clan == null || hero.IsWanderer)
            {
                hero.Clan = Hero.MainHero.Clan;
                hero.Clan.Culture = hero.Culture;
                hero.Gold = hero.Gold > 0 ? hero.Gold : Hero.MainHero.Gold;
                ReflectionHelper.SetPropertyValue(hero, "Occupation", 3);
                KillCharacterAction.ApplyByRemove(Hero.MainHero, false, true);
                ReflectionHelper.SetPropertyValue(CharacterObject.PlayerCharacter, "HeroObject", hero);
                Hero.MainHero.Clan.SetLeader(hero);
                Campaign.Current.MainParty.ChangePartyLeader(hero);
                ChangePlayerCharacterAction.Apply(hero);
            }
            else
            {
                // Hero already has a clan (existing kingdom member like Dain/Theoden)
                // Apply the selected hero as the player character
                ChangePlayerCharacterAction.Apply(hero);

                // CRITICAL: Set the player's default faction to the hero's clan
                // Clan.PlayerClan reads from Campaign.Current.PlayerDefaultFaction
                // but ChangePlayerCharacterAction.Apply does NOT update it.
                // Without this, CharacterDeveloperVM crashes because it enumerates
                // Clan.PlayerClan.Heroes which points to the wrong (empty) clan.
                if (hero.Clan != null)
                {
                    ReflectionHelper.SetPropertyValue(Campaign.Current, "PlayerDefaultFaction", hero.Clan);
                }

                if (originalMainHero != null && originalMainHero != hero)
                {
                    KillCharacterAction.ApplyByRemove(originalMainHero, false, true);
                }
            }

            if (!CampaignTime.Equals(Hero.MainHero.BirthDay, CharacterSelectionViewModel.OriginalHeroBirthday))
                Hero.MainHero.SetBirthDay(CharacterSelectionViewModel.OriginalHeroBirthday);

            MBInformationManager.Clear();
            InformationManager.ClearAllMessages();
            ((IEnumerable<WarPartyComponent>)Clan.PlayerClan.WarPartyComponents).Where<WarPartyComponent>((Func<WarPartyComponent, bool>)(item => item.Leader == null && item.MobileParty != null || item.MobileParty != Hero.MainHero.PartyBelongedTo)).ToList<WarPartyComponent>().ForEach((Action<WarPartyComponent>)(warparty =>
            {
                PartyBase.MainParty.ItemRoster.Add(warparty.MobileParty.ItemRoster);
                PartyBase.MainParty.MemberRoster.Add(warparty.MobileParty.MemberRoster);
                DisbandPartyAction.StartDisband(warparty.MobileParty);
            }));
            CharacterSelectionViewModel.Reset();
        }
        // Skip kingdom join prompt if prebuild hero was already in a kingdom
        if (prebuildHeroAlreadyInKingdom)
          return;

        CharacterCreationOptionsStageVMPatch._kingdom = Campaign.Current.Kingdoms.FirstOrDefault<Kingdom>((Func<Kingdom, bool>)(item => item.Culture?.StringId == Hero.MainHero.Culture?.StringId));
        if (CharacterCreationOptionsStageVMPatch._kingdom != null && !Campaign.Current.MainParty.MapFaction.IsKingdomFaction && Clan.PlayerClan.Kingdom == null)
        InformationManager.ShowInquiry(new InquiryData(new TextObject("Kingdom join decision", (Dictionary<string, object>) null).ToString(), "Do you wanna join your culture kingdom directly?", true, true, GameTexts.FindText("str_ok", (string) null).ToString(), GameTexts.FindText("str_cancel", (string) null).ToString(), (() => ChangeKingdomAction.ApplyByJoinToKingdom(Clan.PlayerClan, CharacterCreationOptionsStageVMPatch._kingdom)), (() => { }), "", 0.0f, null), false, false);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(CharacterCreationOptionsStageVM), nameof(CharacterCreationOptionsStageVM.OnPreviousStage))]
    public static void Post_OnPreviousStagee()
    {
        if (CharacterSelectionViewModel.IsPreBuildHero)
        {
            CharacterSelectionViewModel.Reset();
            CharacterCreationStagesHelper.GotoStage(typeof(CharacterCreationFaceGeneratorStage));
        }
    }
}