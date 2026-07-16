using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Localization;

namespace BloodAndBittersteel.Features.BaBEvents.PopUpEvents
{
    public class BaBDefaultPopupEventProvider : IEventProvider
    {
        public IEnumerable<IBaBEvent> InitializeEvents()
        {
            yield return new BaBPopupEvent(
                "popup_ghost_sighting",
                BaBEventTypes.OnDailyTick,
                1f,
                "test",
                new TextObject("A Ghostly Sighting"),
                new TextObject("You have heard tales of a ghostly figure wandering the moors"),
                CampaignTime.Days(24),
                () => { },
                () => { return true; }
            );
        }
    }
}
