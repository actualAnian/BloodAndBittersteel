using System.Collections.Generic;

namespace BloodAndBittersteel.Features.BaBIncidents
{
    public interface IIncidentProvider
    {
        public IEnumerable<BaBIncident> InitializeEvents();
    }
}
