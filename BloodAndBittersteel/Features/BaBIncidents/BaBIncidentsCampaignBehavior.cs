using HarmonyLib;
using System;
using System.Collections.Generic;
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
        readonly Random _random = new Random();
        readonly Dictionary<BaBIncidentTypes, List<BaBIncident>> CustomIncidents = new();
        readonly List<IIncidentProvider> _providers;

        public BaBIncidentsCampaignBehavior()
        {
            _providers = new()
            {
                new BaBBlackfyreRebellionIncidents()
            };
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
            CustomIncidents[bab.Trigger].Add(bab);
        }
        public void OnBeforeNonReadyObjectsDeleted()
        {
            InitializeIncidents();
        }
        private void InitializeIncidents()
        {
            foreach (var provider in _providers)
                foreach (var reg in provider.InitializeEvents())
                    AddIncident(reg);
        }
        public override void RegisterEvents()
        {
            CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, OnDailyTick);
        }
        public bool CanIncidentBeInvoked(Incident incident)
        {
            System.Reflection.FieldInfo? field = AccessTools.Field(typeof(Func<TextObject, bool>), "_condition");
            if (field == null) return true;
            var func = (Func<TextObject, bool>)field.GetValue(incident);

            return func(incident.Description);
        }
        private void OnDailyTick()
        {
            var mapState = GameStateManager.Current.LastOrDefault<MapState>();
            if (mapState != null)
            {
                var possibleIndicents = new List<BaBIncident>();
                foreach (var incidentList in CustomIncidents.Values)
                {
                    foreach (var inc in incidentList)
                    {
                        if (CanIncidentBeInvoked(inc) && inc.Chance > _random.NextFloat()) 
                            possibleIndicents.Add(inc);
                    }
                }
                mapState.NextIncident = possibleIndicents.GetRandomElement();
            }
        }
        
        public override void SyncData(IDataStore dataStore) { }
    }
}