using BloodAndBittersteel.Features.BaBIncidents;
using HarmonyLib;
using SandBox.ViewModelCollection.Map.Incidents;
using System;
using System.Linq;
using TaleWorlds.CampaignSystem.Incidents;

[HarmonyPatch(typeof(MapIncidentVM), MethodType.Constructor, new Type[] { typeof(Incident), typeof(Action) })]
public class IncidentPatches
{
    public static void Postfix(MapIncidentVM __instance, Incident ____incident)
    {
        BaBIncident? babIncident = BaBIncidentRegister.Instance.AllIncidents.FirstOrDefault(i => i.StringId == ____incident.StringId);
        if (babIncident != null && babIncident.UsesCustomImage)
            __instance.IncidentType = babIncident.CustomImageName;
    }
}