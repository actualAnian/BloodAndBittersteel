using BloodAndBittersteel.Features.BaBEvents;
using BloodAndBittersteel.Features.BaBEvents.PopUpEvents;
using BloodAndBittersteel.Features.BaBEvents.SceneEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace BloodAndBittersteel.Features.CampaignCheats
{
    internal class BaBEventCheats
    {
        [CommandLineFunctionality.CommandLineArgumentFunction("events.fire_event", "bab")]
        public static string BabFireEvent(List<string> args)
        {
            if (args.Count != 1)
                return "Usage: bab.events.fire_event [eventStringId]";

            var eventId = args[0];
            var baBEvent = BaBEventRegister.Instance.AllEvents.FirstOrDefault(e => e.StringId == eventId);

            if (baBEvent is null)
                return $"Event '{eventId}' not found.";


            var mapState = GameStateManager.Current.LastOrDefault<MapState>();

            if (mapState is null)
                return "Error, no active map state.";
            try
            {
                BaBEventsCampaignBehavior.FireEvent(baBEvent, mapState);
            }
            catch { return "Error firing the event"; }
            return $"Event '{eventId}' fired successfully.";
        }

        [CommandLineFunctionality.CommandLineArgumentFunction("events.list_all", "bab")]
        public static string BabListEvents(List<string> args)
        {
            var grouped = new Dictionary<string, List<IBaBEvent>>();

            foreach (var evt in BaBEventRegister.Instance.AllEvents.OrderBy(e => e.StringId))
            {
                var category = GetCategoryName(evt);
                if (!grouped.ContainsKey(category))
                    grouped[category] = new List<IBaBEvent>();
                grouped[category].Add(evt);
            }

            var sb = new System.Text.StringBuilder();
            foreach (var group in grouped)
            {
                sb.Append("=======================================");
                sb.Append(Environment.NewLine);
                sb.Append($"{group.Key}:");
                sb.Append(Environment.NewLine);
                foreach (var evt in group.Value)
                {
                    sb.Append($"  {evt.StringId}");
                    sb.Append(Environment.NewLine);
                }
            }

            return sb.ToString();
        }

        static string GetCategoryName(IBaBEvent eventType)
        {
            return eventType switch
            {
                BaBIncident => "Incidents",
                BaBSceneEvent => "Scene Events",
                BaBPopupEvent => "Popup Events",
                _ => eventType.GetType().Name,
            };
        }
    }
}
