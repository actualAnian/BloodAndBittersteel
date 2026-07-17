using BloodAndBittersteel.Features.BaBEvents.PopUpEvents.Events;
using System.Collections.Generic;

namespace BloodAndBittersteel.Features.BaBEvents.PopUpEvents
{
    public class BaBDefaultPopupEventProvider : IEventProvider
    {
        public IEnumerable<IBaBEvent> InitializeEvents()
        {
            yield return RebellionPhase2Event.Instance;
            yield return VultureKingEvent.Instance;
            yield return KingBeyondTheWallEvent.Instance;
        }
    }
}
