using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.Incidents;
using static TaleWorlds.CampaignSystem.CampaignBehaviors.IncidentsCampaignBehaviour;

namespace BloodAndBittersteel.Features.BaBIncidents
{
    public enum BaBIncidentTypes
    {
        OnDailyTick,
    }

    public class BaBIncident : Incident
    {
        public bool UsesCustomImage { get; private set; } = false;
        public string CustomImageName { get; private set; } = "";
        public new BaBIncidentTypes Trigger { get; private set; }
        public float Chance { get; private set; }
        public BaBIncident(string id, BaBIncidentTypes trigger, float chance) : base(id)
        {
            Trigger = trigger;
            if (chance < 0 || chance > 1)
                throw new Exception("BaBIncident chance shpuld be between 0 and 1");
            Chance = chance;
        }
        public BaBIncident(string id, BaBIncidentTypes trigger, float chance, string customImageName) : base(id)
        {
            Trigger = trigger;
            if (chance < 0 || chance > 1)
                throw new Exception("BaBIncident chance shpuld be between 0 and 1");
            Chance = chance;
            UsesCustomImage = true;
            CustomImageName = customImageName;
        }
    }
}