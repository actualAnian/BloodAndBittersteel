using HarmonyLib;
using System;
using System.Reflection;
using TaleWorlds.CampaignSystem.Incidents;
using TaleWorlds.Localization;

namespace BloodAndBittersteel.Features.BaBEvents
{
    public class BaBIncident : Incident, IBaBEvent
    {
        public bool UsesCustomImage { get; private set; } = false;
        public string CustomImageName { get; private set; } = "";
        public BaBEventTypes EventType { get; private set; }
        public float Chance { get; private set; }
        public BaBIncident(string id, BaBEventTypes eventType, float chance) : base(id)
        {
            EventType = eventType;
            if (chance < 0 || chance > 1)
                throw new Exception("BaBIncident chance should be between 0 and 1");
            Chance = chance;
        }
        public BaBIncident(string id, BaBEventTypes eventType, float chance, string customImageName) : base(id)
        {
            EventType = eventType;
            if (chance < 0 || chance > 1)
                throw new Exception("BaBIncident chance should be between 0 and 1");
            Chance = chance;
            UsesCustomImage = true;
            CustomImageName = customImageName;
        }

        bool IBaBEvent.Condition()
        {
            var field = AccessTools.Field("TaleWorlds.CampaignSystem.Incidents.Incident:_condition");
            if (field == null) return true;
            var func = (Func<TextObject, bool>)field.GetValue(this);
            return func?.Invoke(Description) ?? true;
        }
    }
}