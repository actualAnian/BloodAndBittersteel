using BloodAndBittersteel.Features.BaBIncidents;
using HarmonyLib;
using SandBox.ViewModelCollection.Map.Incidents;
using System;
using TaleWorlds.CampaignSystem.Incidents;

[HarmonyPatch(typeof(MapIncidentVM), MethodType.Constructor, new Type[] { typeof(Incident), typeof(Action) })]
public class IncidentPatches
{
    public static void Postfix(MapIncidentVM __instance, Incident ____incident)
    {
        BaBIncident.AllBaBIncidents.TryGetValue(____incident.StringId, out var babIncident);
        if (babIncident != null && babIncident.UsesCustomImage)
            __instance.IncidentType = babIncident.CustomImageName;
    }
}