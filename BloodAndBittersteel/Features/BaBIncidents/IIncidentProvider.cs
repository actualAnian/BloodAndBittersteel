using System.Collections.Generic;
using TaleWorlds.CampaignSystem.Incidents;

namespace BloodAndBittersteel.Features.BaBIncidents
{
    public interface IIncidentProvider
    {
        public IEnumerable<BaBIncident> InitializeEvents();
    }
}
