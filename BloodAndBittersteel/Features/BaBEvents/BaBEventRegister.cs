using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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
        readonly IEnumerable<IEventProvider> _providers;
        public List<IBaBEvent> AllEvents { get; private set; } = new();
        private BaBEventRegister()
        {
            _providers = Discover();
            RegisterEvents();
        }
        public static IEnumerable<IEventProvider> Discover()
        {
            return Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t =>
                    typeof(IEventProvider).IsAssignableFrom(t) &&
                    !t.IsAbstract &&
                    !t.IsInterface)
                .Select(t => (IEventProvider)Activator.CreateInstance(t)!);
        }
        public void RegisterEvents()
        {
            foreach (var provider in _providers)
                foreach (var reg in provider.InitializeEvents())
                    AllEvents.Add(reg);
        }
    }
}
