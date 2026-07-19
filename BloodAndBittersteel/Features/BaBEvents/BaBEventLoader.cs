using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BloodAndBittersteel.Features.BaBEvents
{
    public class BaBEventLoader
    {
        private static BaBEventLoader? _instance;
        public static BaBEventLoader Instance
        {
            get
            {
                return _instance ??= new BaBEventLoader();
            }
        }
        public IEnumerable<IBaBEvent> AllEvents { get; private set; }
        private BaBEventLoader()
        {
            AllEvents = LoadEvents();
        }
        public IEnumerable<IBaBEvent> LoadEvents()
        {
            foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
            {
                foreach (var method in type.GetMethods(
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly))
                {
                    if (method.GetCustomAttribute<BaBEventAttribute>() is null)
                        continue;

                    if (!method.IsStatic)
                        throw new InvalidOperationException(
                            $"{method} must be static.");

                    if (method.GetParameters().Length != 0)
                        throw new InvalidOperationException(
                            $"{method} must have no parameters.");

                    if (!typeof(IBaBEvent).IsAssignableFrom(method.ReturnType))
                        throw new InvalidOperationException(
                            $"{method} must return IBaBEvent.");
                    yield return (IBaBEvent)method.Invoke(null, null)!;
                }
            }
        }
    }
}
