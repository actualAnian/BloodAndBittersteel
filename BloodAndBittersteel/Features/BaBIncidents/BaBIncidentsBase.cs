using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using TaleWorlds.CampaignSystem.Incidents;
using TaleWorlds.Localization;

namespace BloodAndBittersteel.Features.BaBIncidents
{
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
