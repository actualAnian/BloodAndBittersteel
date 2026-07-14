using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.Core;

namespace BloodAndBittersteel.Features.BaBEvents
{
    public enum BaBEventTypes
    {
        OnDailyTick,
        OnWeeklyTick,
    }
    public class BaBEventsCampaignBehavior : CampaignBehaviorBase
    {
        readonly Random _random = new();
        readonly Dictionary<BaBEventTypes, List<IBaBEvent>> _eventsByType = new();
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
        public override void RegisterEvents()
        {
            CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, OnDailyTick);
            CampaignEvents.WeeklyTickEvent.AddNonSerializedListener(this, OnWeeklyTick);
            //CampaignEvents.OnGameEarlyLoadedEvent(this, InitializeEvents);
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
            _eventsOnCooldown.Add(chosen.StringId, CampaignTime.Days(24));
        }
        private void FireEvent(IBaBEvent evt, MapState mapState)
        {
            switch (evt)
            {
                case BaBImmediateEvent immediate:
                    immediate.Fire(mapState);
                    break;
                case BaBIncident incident:
                    mapState.NextIncident = incident;
                    break;
            }
        }
        public override void SyncData(IDataStore dataStore) 
        {
            dataStore.SyncData("bab_eventsOnCooldown", ref _eventsOnCooldown);
        }
    }
}