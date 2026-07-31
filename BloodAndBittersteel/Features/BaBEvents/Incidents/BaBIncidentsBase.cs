using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using TaleWorlds.CampaignSystem.Incidents;
using TaleWorlds.Localization;

namespace BloodAndBittersteel.Features.BaBEvents.Incidents
{
    // as of 1.4.7 the condition of each Effect for the incident has to return true, for the event to be fired -> Incident.CanIncidentBeInvoked
    public static class BaBIncidentsBase
    {
        static readonly ConstructorInfo _ctor =
        AccessTools.Constructor(
            typeof(IncidentEffect),
            new Type[] {
                    typeof(Func<bool>),
                    typeof(Func<List<TextObject>>),
                    typeof(Func<IncidentEffect, List<TextObject>>)
        });
        public static IncidentEffect CreateCustomIncidentEffect(Func<bool> condition, Func<List<TextObject>> consequence, Func<IncidentEffect, List<TextObject>> hint)
        {
            return (IncidentEffect)_ctor.Invoke(new object[] { condition, consequence, hint });
        }
    }
}
