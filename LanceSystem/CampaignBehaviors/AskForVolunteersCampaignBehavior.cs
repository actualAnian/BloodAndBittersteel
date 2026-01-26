using LanceSystem.Dialogues;
using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.SaveSystem;

namespace LanceSystem.CampaignBehaviors
{
    public class AskForVolunteersCampaignBehavior : CampaignBehaviorBase
    {
        [SaveableField(1)]
        private Dictionary<string, CampaignTime> _lastRequestTimes = new();

        private const float CooldownDays = 7f;
        private float GetSuccessChance 
        {
            get 
            {
                float chance = 0f;
                chance += Hero.MainHero.Clan.Tier == 0? 0.1f : Math.Min(0.5f, 0.2f * Hero.MainHero.Clan.Tier);
                if (Hero.MainHero.IsFemale && LanceSettings.Instance.FemalePrejudice) chance -= 0.05f * Hero.MainHero.Clan.Tier;
                if (Settlement.CurrentSettlement.OwnerClan == Clan.PlayerClan) chance += 0.3f;
                return chance; 
            }
        }
        private Tuple<int, int> GetMinMaxNumberOfVolunteers
        {                 
            get
            {
                int maxAmount = 3;
                if (Hero.MainHero.Clan.Tier > 0) maxAmount += 2;
                if (Hero.MainHero.IsFemale && LanceSettings.Instance.FemalePrejudice) maxAmount -= 1;
                return new(1, maxAmount);
            }
        }
        public override void RegisterEvents()
        {
            CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, AddDialogs);
        }
        public override void SyncData(IDataStore dataStore)
        {
            dataStore.SyncData("RecruitVolunteers_LastRequestTimes", ref _lastRequestTimes);
        }
        private void AddDialogs(CampaignGameStarter starter)
        {
            starter.AddPlayerLine("lance_ask_for_volunteers", "hero_main_options", "lance_volunteer_response", "{=lance_volunteers_ask}Do you have any volunteers willing to join my party?", CanAskForVolunteers, null);
            starter.AddDialogLine("rv_volunteer_response_line", "lance_volunteer_response", "volunteers_interested", "{VOLUNTEER_RESPONSE}", AskForVolunteersSuccess, null);
            starter.AddDialogLine("lance_volunteer_refusal", "lance_volunteer_response", "hero_main_options", "{REFUSAL_LINE}", AskForVolunteersFailure, null);
            starter.AddPlayerLine("volunteer_take", "volunteers_interested", "lord_pretalk", "{=lance_recruitment_take}I will take them", null, GiveVolunteers, 100, null);
            starter.AddPlayerLine("volunteer_no_take", "volunteers_interested", "lord_pretalk", "{=lance_options_no}I changed my mind", null, null, 100, null);
        }
        private bool AskForVolunteersFailure()
        {
            if (_askSucceded)
            {
                _askSucceded = false;
                return false;
            }
            var text = VolunteerSystemDialogs.GetNoVolunteersDialogue(CharacterObject.OneToOneConversationCharacter.HeroObject);
            MBTextManager.SetTextVariable("REFUSAL_LINE", new TextObject(text));
            return true;
        }
        private bool AskForVolunteersSuccess()
        {
            var notable = CharacterObject.OneToOneConversationCharacter.HeroObject;
            if (notable == null || !notable.IsNotable || notable.CurrentSettlement.IsCastle)
                return false;
            _lastRequestTimes[notable.StringId] = CampaignTime.Now;

            if (MBRandom.RandomFloat > GetSuccessChance)
                return false;
            var minmaxTuple = GetMinMaxNumberOfVolunteers;
            _amount = MBRandom.RandomInt(minmaxTuple.Item1, minmaxTuple.Item2);
            _cachedTroopToGive = notable.Culture.BasicTroop;

            var text = "{=lance_volunteer_dialog} {AMOUNT} {VOLUNTEER_NAME} want to join your cause";
            MBTextManager.SetTextVariable("VOLUNTEER_RESPONSE", new TextObject(text)
                .SetTextVariable("AMOUNT", _amount)
                .SetTextVariable("VOLUNTEER_NAME", _cachedTroopToGive.Name));
            _askSucceded = true;
            return true;
        }
        private bool CanAskForVolunteers()
        {
            var notable = CharacterObject.OneToOneConversationCharacter.HeroObject;
            if (notable == null || !notable.IsNotable || notable.CurrentSettlement.IsCastle)
                return false;

            if (_lastRequestTimes.TryGetValue(notable.StringId, out CampaignTime lastTime))
            {
                if (CampaignTime.Now - lastTime < CampaignTime.Days(CooldownDays))
                    return false;
            }
            return true;
        }
        private CharacterObject? _cachedTroopToGive;
        private int _amount;
        private bool _askSucceded = false;
        private void GiveVolunteers()
        {
            Hero notable = CharacterObject.OneToOneConversationCharacter.HeroObject;
            _lastRequestTimes[notable.StringId] = CampaignTime.Now;
            PartyBase.MainParty.AddMember(_cachedTroopToGive, _amount);
            MBTextManager.SetTextVariable("VOLUNTEER_RESPONSE", new TextObject("{=lance_volunteer_success}I can spare {COUNT} volunteers. Treat them well." ).SetTextVariable("COUNT", _amount));
        }
    }
}