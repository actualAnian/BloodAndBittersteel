using BloodAndBittersteel.Features.CharacterSelection.Helper;
using BloodAndBittersteel.Features.CharacterSelection.ViewModel;
using HarmonyLib;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.CharacterCreationContent;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.ViewModelCollection.CharacterCreation.OptionsStage;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace BloodAndBittersteel.Features.CharacterSelection.Patches;

[HarmonyPatch]
public static class CharacterCreationOptionsStageVMPatch
{
    public static void CheckSkillReset()
    {
        var charSel = PreMadeCharacterSelection.Instance;

        InformationManager.ShowInquiry(new InquiryData(
            new TextObject("Skill reset decision").ToString(),
            "Do you wanna set your skills to default?",
            true,
            true,
            GameTexts.FindText("str_ok").ToString(),
            GameTexts.FindText("str_cancel").ToString(),
            () => charSel.ResetHeroSkills(Hero.MainHero), null
        ), true, true);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof (CharacterCreationOptionsStageVM), "OnNextStage")]
    public static void Post_OnNextStage()
    {
        Kingdom kingdom = Campaign.Current.Kingdoms.FirstOrDefault(item => item.Culture?.StringId == Hero.MainHero.Culture?.StringId);
        if (CharacterSelectionViewModel.Instance.PreBuildHero != null)
        {
            CheckSkillReset();
            Hero hero = CharacterSelectionViewModel.Instance.PreBuildHero;
            kingdom = Campaign.Current.Kingdoms.FirstOrDefault(item => item.Culture?.StringId == hero.Culture?.StringId);

            Hero originalMainHero = Hero.MainHero;
            Clan originalClan = originalMainHero.Clan;

            if (hero.IsWanderer || hero.Clan == null)
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
                ChangePlayerCharacterAction.Apply(hero);
                if (hero.Clan != null)
                    ReflectionHelper.SetPropertyValue(Campaign.Current, "PlayerDefaultFaction", hero.Clan);

                if (originalMainHero != null && originalMainHero != hero)
                    KillCharacterAction.ApplyByRemove(originalMainHero, false, true);
            }

            if (!Equals(Hero.MainHero.BirthDay, CharacterSelectionViewModel.Instance.OriginalHeroBirthday))
                Hero.MainHero.SetBirthDay(CharacterSelectionViewModel.Instance.OriginalHeroBirthday);

            MBInformationManager.Clear();
            InformationManager.ClearAllMessages();
            Clan.PlayerClan.WarPartyComponents.Where(item => (item.Leader == null && item.MobileParty != null) || item.MobileParty != Hero.MainHero.PartyBelongedTo).ToList().ForEach(warparty =>
            {
                PartyBase.MainParty.ItemRoster.Add(warparty.MobileParty.ItemRoster);
                PartyBase.MainParty.MemberRoster.Add(warparty.MobileParty.MemberRoster);
                DisbandPartyAction.StartDisband(warparty.MobileParty);
            });
            CharacterSelectionViewModel.RemoveInstance();
            if (GameStateManager.Current.ActiveState is MapState mapState)
            {
                mapState.Handler.ResetCamera(true, true);
                mapState.Handler.TeleportCameraToMainParty();
            }
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(CharacterCreationOptionsStageVM), nameof(CharacterCreationOptionsStageVM.OnPreviousStage))]
    public static void Post_OnPreviousStagee()
    {
        if (CharacterSelectionViewModel.Instance.IsPreBuiltHero)
        {
            CharacterSelectionViewModel.Instance.Reset();
            CharacterCreationStagesHelper.GotoStage(typeof(CharacterCreationFaceGeneratorStage));
        }
    }
}