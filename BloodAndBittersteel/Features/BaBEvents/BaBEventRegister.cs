using BloodAndBittersteel.Features.BaBEvents.Incidents;
using BloodAndBittersteel.Features.BaBEvents.PopUpEvents;
using BloodAndBittersteel.Features.BaBEvents.SceneEvents;
using System.Collections.Generic;

namespace BloodAndBittersteel.Features.BaBEvents
{
    public class BaBEventRegister
    {
        private static BaBEventRegister? _instance;
        public static BaBEventRegister Instance
        {
            get
            {
                return _instance ??= new BaBEventRegister();
            }
        }
        readonly List<IEventProvider> _providers;
        public List<IBaBEvent> AllEvents { get; private set; } = new();
        private BaBEventRegister()
        {
            _providers = new()
            {
                new BaBBlackfyreRebellionIncidents(),
                new ExamplePopupEventProvider(),
                new ExampleSceneEventProvider(),
            };
            RegisterEvents();
        }
        public void RegisterEvents()
        {
            foreach (var provider in _providers)
                foreach (var reg in provider.InitializeEvents())
                    AllEvents.Add(reg);
        }
    }
}
