using BloodAndBittersteel.Features.ModifiableValues;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.CampaignSystem.Incidents;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace BloodAndBittersteel.Features.BaBIncidents
{
    public class BaBIncidentsCampaignBehavior : CampaignBehaviorBase, INonReadyObjectHandler
    {
        readonly Random _random = new();
        readonly Dictionary<BaBIncidentTypes, List<BaBIncident>> CustomIncidents = new();
        private Dictionary<string, CampaignTime> _incidentsOnCooldown;
        public BaBIncidentsCampaignBehavior()
        {
            _incidentsOnCooldown = new();
        }

        public void AddIncident(Incident incident)
        {
            if (incident is not BaBIncident bab)
            {
                Game.Current.ObjectManager.RegisterPresumedObject(incident);
                return;
            }
            if (!CustomIncidents.TryGetValue(bab.Trigger, out var list))
                CustomIncidents[bab.Trigger] = new();
            if (CustomIncidents[bab.Trigger].Contains(incident))
                return;
            CustomIncidents[bab.Trigger].Add(bab);
        }
        public void OnBeforeNonReadyObjectsDeleted()
        {
            InitializeIncidents();
        }
        private void InitializeIncidents()
        {
            foreach (var reg in BaBIncidentRegister.Instance.AllIncidents)
                AddIncident(reg);
        }
        public override void RegisterEvents()
        {
            CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, OnDailyTick);
        }


        public bool CanIncidentBeInvoked(Incident incident)
        {
            if (_incidentsOnCooldown.TryGetValue(incident.StringId, out var result))
            {
                if (result.IsPast) _incidentsOnCooldown.Remove(incident.StringId);
                else return false;
            }
            System.Reflection.FieldInfo? field = AccessTools.Field(typeof(Func<TextObject, bool>), "_condition");
            if (field == null) return true;
            var func = (Func<TextObject, bool>)field.GetValue(incident);

            return func(incident.Description);
        }
        private void OnDailyTick()
        {
            var mapState = GameStateManager.Current.LastOrDefault<MapState>();
            if (mapState == null) return;
            var possibleIndicents = new List<BaBIncident>();
            foreach (var incidentList in CustomIncidents.Values)
            {
                foreach (var inc in incidentList)
                {
                    if (CanIncidentBeInvoked(inc) && inc.Chance > _random.NextFloat()) 
                        possibleIndicents.Add(inc);
                }
            }
            InvokeRandomIncident(possibleIndicents);
        }
        private void InvokeRandomIncident(IEnumerable<Incident> incidents)
        {
            if (incidents.IsEmpty()) return;
            var incident = incidents.GetRandomElementInefficiently();
            var mapState = GameStateManager.Current.LastOrDefault<MapState>();
            if (mapState == null) return;
            mapState.NextIncident = incident;
            _incidentsOnCooldown.Add(incident.StringId, incident.Cooldown);
        }
        public override void SyncData(IDataStore dataStore) 
        {
            dataStore.SyncData("bab_incidentsOnCooldown", ref _incidentsOnCooldown);
        }
    }
}