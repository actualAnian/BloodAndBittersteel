using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.SaveSystem;

namespace BloodAndBittersteel.Features.BaBEvents
{
    public enum BaBEventTypes
    {
        OnDailyTick,
        OnWeeklyTick,
    }
    public class BaBEventsCampaignBehavior : CampaignBehaviorBase, INonReadyObjectHandler
    {
        readonly Random _random = new();
        readonly Dictionary<BaBEventTypes, List<IBaBEvent>> _eventsByType = new();
        [SaveableField(1)]
        private Dictionary<string, CampaignTime> _eventsOnCooldown;
        public BaBEventsCampaignBehavior()
        {
            _eventsOnCooldown = new();
        }
        public void AddEvent(IBaBEvent evt)
        {
            if (!_eventsByType.TryGetValue(evt.EventType, out var list))
                _eventsByType[evt.EventType] = list = new();
            if (list.Contains(evt)) return;
            list.Add(evt);

            if (evt is BaBIncident babIncident)
                Game.Current.ObjectManager.RegisterPresumedObject(babIncident);
        }
        private void InitializeEvents()
        {
            foreach (var evt in BaBEventRegister.Instance.AllEvents)
                AddEvent(evt);
        }
        public void OnBeforeNonReadyObjectsDeleted()
        {
            InitializeEvents();
        }

        public override void RegisterEvents()
        {
            CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, OnDailyTick);
            CampaignEvents.WeeklyTickEvent.AddNonSerializedListener(this, OnWeeklyTick);
        }
        private bool CanEventFire(IBaBEvent evt)
        {
            if (_eventsOnCooldown.TryGetValue(evt.StringId, out var result))
            {
                if (result.IsPast) _eventsOnCooldown.Remove(evt.StringId);
                else return false;
            }
            return evt.CheckCondition();
        }
        private void OnDailyTick()
        {
            ProcessEventsForType(BaBEventTypes.OnDailyTick);
        }
        private void OnWeeklyTick()
        {
            ProcessEventsForType(BaBEventTypes.OnWeeklyTick);
        }
        private void ProcessEventsForType(BaBEventTypes eventType)
        {
            var mapState = GameStateManager.Current.LastOrDefault<MapState>();
            if (mapState == null) return;

            if (!_eventsByType.TryGetValue(eventType, out var eventList)) return;

            var fireableEvents = new List<IBaBEvent>();
            foreach (var evt in eventList)
            {
                if (CanEventFire(evt) && evt.Chance > _random.NextFloat())
                    fireableEvents.Add(evt);
            }

            if (fireableEvents.IsEmpty()) return;

            var chosen = fireableEvents.GetRandomElementInefficiently();
            FireEvent(chosen, mapState);
            _eventsOnCooldown.Add(chosen.StringId, chosen.Cooldown);
        }
        public static void FireEvent(IBaBEvent evt, MapState mapState)
        {   
           try
           {
                switch (evt)
                {
                    case BaBImmediateEvent immediate:
                        immediate.Fire();
                        break;
                    case BaBIncident incident:
                        mapState.NextIncident = incident;
                        break;
                }

            }
            catch(Exception e)
            { 
                InformationManager.DisplayMessage(new($"ERROR firing an event with id {evt.StringId}", new Color(1, 0, 0)));
                InformationManager.DisplayMessage(new($"DEBUG MESSAGE: {e.Message}", new Color(1, 0, 0)));
            }
        }
        public override void SyncData(IDataStore dataStore) 
        {
            dataStore.SyncData("bab_eventsOnCooldown", ref _eventsOnCooldown);
        }
    }
}