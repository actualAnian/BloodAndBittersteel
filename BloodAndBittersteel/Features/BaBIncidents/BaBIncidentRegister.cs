using System.Collections.Generic;

namespace BloodAndBittersteel.Features.BaBIncidents
{
    public class BaBIncidentRegister
    {
        private static BaBIncidentRegister? _instance;
        public static BaBIncidentRegister Instance
        {
            get
            {
                return _instance ??= new BaBIncidentRegister();
            }
        }
        readonly List<IIncidentProvider> _providers;
        public List<BaBIncident> AllIncidents { get; private set; } = new();
        private BaBIncidentRegister()
        {
            _providers = new()
            {
                new BaBBlackfyreRebellionIncidents()
            };
            RegisterIncidents();
        }
        public void RegisterIncidents()
        {
            foreach (var provider in _providers)
                foreach (var reg in provider.InitializeEvents())
                    AllIncidents.Add(reg);
        }
    }
}
