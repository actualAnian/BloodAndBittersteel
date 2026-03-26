using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using static TaleWorlds.CampaignSystem.Party.PartyScreenLogic;

namespace LanceSystem.UI
{
    public class MercenaryTroopsBuyLogic
    {
        const int BuyMercTroopMultiplier = 100; 
        private static MercenaryTroopsBuyLogic? instance;
        public static MercenaryTroopsBuyLogic Instance { get => instance ??= new(); }
        int _goldToPay = 0;
        List<MercenaryPartyViewData> _data = new();
        PartyScreenLogic _screenLogic = null!;
        readonly MethodInfo _setGoldMethod = AccessTools.Method("PartyScreenLogic:SetPartyGoldChangeAmount");

        public void Init(TroopRoster beginningTroopRoster, PartyScreenLogic logic)
        {
            _goldToPay = 0;
            _data = new();

            foreach (var troop in beginningTroopRoster.GetTroopRoster())
                _data.Add(new(troop.Number, troop.Number, troop.Character));
            _screenLogic = logic;
        }
        public void Reset()
        {
            _goldToPay = 0;
            foreach (var troop in _data)
                troop.CurrentAmount = troop.OriginalAmount;
        }
        public void OnTroopTransfer(CharacterObject troop, int number, PartyRosterSide fromSide)
        {
            var data = _data.FirstOrDefault(d => d.Character == troop);
            if (data == null)
                return;
            var cost = troop.Tier * BuyMercTroopMultiplier;
            int isPaying = 1;
            int amount = 0;
            if (fromSide == PartyRosterSide.Left)
            {
                isPaying = -1;
                data.CurrentAmount -= number;
                amount = Math.Max(0, Math.Min(data.OriginalAmount - data.CurrentAmount, number));
            }
            else
            {
                isPaying = 1;
                amount = Math.Max(0, Math.Min(data.OriginalAmount - data.CurrentAmount, number));
                data.CurrentAmount += number;
            }
            cost *= amount;
            _goldToPay += isPaying * cost;
            _setGoldMethod.Invoke(_screenLogic, new object[1] { _goldToPay });
        }
        public void OnDone(PartyBase mercenaryParty)
        {
            foreach(var troopInfo in _data)
                mercenaryParty.MemberRoster.RemoveTroop(troopInfo.Character, troopInfo.OriginalAmount - troopInfo.CurrentAmount);
        }
    }
    public class MercenaryPartyViewData
    {
        public MercenaryPartyViewData(int currentAmount, int originalAmount, CharacterObject character)
        {
            CurrentAmount = currentAmount;
            OriginalAmount = originalAmount;
            Character = character;
        }
        public int CurrentAmount { get; set; }
        public int OriginalAmount { get; set; }
        public CharacterObject Character { get; set; }
    }

}
