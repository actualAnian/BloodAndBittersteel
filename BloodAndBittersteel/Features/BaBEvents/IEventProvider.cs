using System.Collections.Generic;

namespace BloodAndBittersteel.Features.BaBEvents
{
    public interface IEventProvider
    {
        public IEnumerable<IBaBEvent> InitializeEvents();
    }
}
