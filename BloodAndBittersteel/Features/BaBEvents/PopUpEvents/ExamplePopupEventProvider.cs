using System;
using System.Collections.Generic;
using TaleWorlds.Localization;

namespace BloodAndBittersteel.Features.BaBEvents.PopUpEvents
{
    public class ExamplePopupEventProvider : IEventProvider
    {
        public IEnumerable<IBaBEvent> InitializeEvents()
        {
            yield return new BaBPopupEvent(
                "popup_ghost_sighting",
                BaBEventTypes.OnDailyTick,
                0.15f,
                "ModuleData/images/ghost.png",
                new TextObject("{=bab_ghost_sight}A Ghostly Sighting"),
                new List<TextObject> { new TextObject("{=bab_ghost_desc}You have heard tales of a ghostly figure wandering the moors...") },
                () => { },
                () => { return true; }
            );
        }
    }
}
