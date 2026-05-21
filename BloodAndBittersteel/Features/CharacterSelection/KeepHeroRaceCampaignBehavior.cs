//using System;
//using System.Collections.Generic;
//using System.Linq;
//using TaleWorlds.CampaignSystem;
//using TaleWorlds.SaveSystem;

//namespace BloodAndBittersteel.Features.CharacterSelection
//{
//    public class KeepHeroRaceCampaignBehavior : CampaignBehaviorBase
//    {
//        private readonly IRaceManager _raceManager;

//        private Dictionary<string, string> _heroRaceMap = new();
//        private Dictionary<string, CampaignTime> _heroBirthDayMap = new();
//        private bool _hasBeenInitialized = false;

//        public KeepHeroRaceCampaignBehavior(IRaceManager raceManager)
//        {
//            _raceManager = raceManager ?? throw new ArgumentNullException(nameof(raceManager));
//        }

//        public override void RegisterEvents()
//        {
//            CampaignEvents.OnBeforeSaveEvent.AddNonSerializedListener(this, new Action(this.OnSave));
//            CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnSessionStart));
//            CampaignEvents.HeroCreated.AddNonSerializedListener(this, new Action<Hero, bool>(this.OnHeroCreated));
//        }

//        private void OnSessionStart(CampaignGameStarter obj)
//        {
//            if (!_hasBeenInitialized)
//            {
//                if (_heroRaceMap.Count > 0)
//                {
//                    // Old save loaded — migrate integer race names to string names
//                    MigrateRaceMapIfNeeded();

//                    // Restore race and birthday from saved data
//                    foreach (Hero hero in Hero.AllAliveHeroes)
//                    {
//                        RestoreHero(hero);
//                    }
//                    foreach (Hero hero in Hero.DeadOrDisabledHeroes)
//                    {
//                        RestoreHero(hero);
//                    }
//                }
//                else
//                {
//                    // Brand new game: populate initial race data for all heroes
//                    foreach (Hero hero in Hero.AllAliveHeroes)
//                    {
//                        var heroId = hero.StringId;
//                        _heroRaceMap[heroId] = _raceManager.GetRaceNameFromId(hero.CharacterObject.Race);
//                        _heroBirthDayMap[heroId] = hero.BirthDay;
//                    }
//                    foreach (Hero hero in Hero.DeadOrDisabledHeroes)
//                    {
//                        var heroId = hero.StringId;
//                        _heroRaceMap[heroId] = _raceManager.GetRaceNameFromId(hero.CharacterObject.Race);
//                        _heroBirthDayMap[heroId] = hero.BirthDay;
//                    }
//                }
//                _hasBeenInitialized = true;
//                return;
//            }

//            // Loaded game: restore race and birthday from saved data
//            foreach (Hero hero in Hero.AllAliveHeroes)
//            {
//                RestoreHero(hero);
//            }
//            foreach (Hero hero in Hero.DeadOrDisabledHeroes)
//            {
//                RestoreHero(hero);
//            }
//        }

//        private void RestoreHero(Hero hero)
//        {
//            var heroId = hero.StringId;

//            if (_heroRaceMap.TryGetValue(heroId, out var savedRaceName))
//            {
//                var savedRaceId = _raceManager.GetRaceIdFromName(savedRaceName);
//                if (savedRaceId != hero.CharacterObject.Race)
//                {
//                    hero.CharacterObject.Race = savedRaceId;
//                }
//            }

//            if (_heroBirthDayMap.TryGetValue(heroId, out var savedBirthDay))
//            {
//                if (!CampaignTime.Equals(hero.BirthDay, savedBirthDay))
//                {
//                    hero.SetBirthDay(savedBirthDay);
//                }
//            }

//            // Track heroes not yet in the map (from an older save version)
//            if (!_heroRaceMap.ContainsKey(heroId))
//            {
//                _heroRaceMap[heroId] = _raceManager.GetRaceNameFromId(hero.CharacterObject.Race);
//                _heroBirthDayMap[heroId] = hero.BirthDay;
//            }
//        }

//        private void MigrateRaceMapIfNeeded()
//        {
//            var keysToMigrate = _heroRaceMap
//                .Where(kv => int.TryParse(kv.Value, out _))
//                .Select(kv => kv.Key)
//                .ToList();

//            foreach (var key in keysToMigrate)
//            {
//                _heroRaceMap[key] = MigrateIntegerRaceName(_heroRaceMap[key], _raceManager);
//            }
//        }

//        private static string MigrateIntegerRaceName(string value, IRaceManager raceManager)
//        {
//            if (int.TryParse(value, out var raceId))
//            {
//                return raceManager.GetRaceNameFromId(raceId);
//            }
//            return value;
//        }

//        private void OnHeroCreated(Hero hero, bool isBornNaturally)
//        {
//            if (hero == null)
//                return;

//            var heroId = hero.StringId;
//            if (!_heroRaceMap.ContainsKey(heroId))
//            {
//                _heroRaceMap[heroId] = _raceManager.GetRaceNameFromId(hero.CharacterObject.Race);
//                _heroBirthDayMap[heroId] = hero.BirthDay;
//            }
//        }

//        private void OnSave()
//        {
//            var knownHeroIds = new HashSet<string>();
//            foreach (Hero hero in Hero.AllAliveHeroes)
//            {
//                var heroId = hero.StringId;
//                knownHeroIds.Add(heroId);
//                _heroRaceMap[heroId] = _raceManager.GetRaceNameFromId(hero.CharacterObject.Race);
//                _heroBirthDayMap[heroId] = hero.BirthDay;
//            }
//            foreach (Hero hero in Hero.DeadOrDisabledHeroes)
//            {
//                var heroId = hero.StringId;
//                knownHeroIds.Add(heroId);
//                _heroRaceMap[heroId] = _raceManager.GetRaceNameFromId(hero.CharacterObject.Race);
//                _heroBirthDayMap[heroId] = hero.BirthDay;
//            }

//            // Prune heroes that no longer exist in either alive or dead/disabled sets
//            var unknownKeys = _heroRaceMap.Keys.Where(k => !knownHeroIds.Contains(k)).ToList();
//            foreach (var key in unknownKeys)
//            {
//                _heroRaceMap.Remove(key);
//                _heroBirthDayMap.Remove(key);
//            }
//        }

//        public override void SyncData(IDataStore dataStore)
//        {
//            dataStore.SyncData("_heroRaceMap", ref _heroRaceMap);
//            dataStore.SyncData("_heroBirthDayMap", ref _heroBirthDayMap);
//            dataStore.SyncData("_hasBeenInitialized", ref _hasBeenInitialized);

//            // Backwards compatibility: if SyncData failed to hydrate the new string dictionary
//            // (old save had Dictionary<string,int>), attempt to read the legacy int map.
//            if (dataStore.IsLoading && (_heroRaceMap == null || _heroRaceMap.Count == 0))
//            {
//                try
//                {
//                    Dictionary<string, int> legacyRaceMap = null;
//                    dataStore.SyncData("_heroRaceMap", ref legacyRaceMap);
//                    if (legacyRaceMap != null && legacyRaceMap.Count > 0)
//                    {
//                        _heroRaceMap = new Dictionary<string, string>();
//                        foreach (var kvp in legacyRaceMap)
//                        {
//                            _heroRaceMap[kvp.Key] = _raceManager.GetRaceNameFromId(kvp.Value);
//                        }
//                    }
//                }
//                catch (Exception)
//                {
//                    // Legacy load failed; OnSessionStart will repopulate from live hero data
//                }
//            }

//            _heroRaceMap ??= new Dictionary<string, string>();
//            _heroBirthDayMap ??= new Dictionary<string, CampaignTime>();
//        }
//    }
//}

//namespace LOTRAOM
//{
//    public class HeroRaceMapSaveableTypeDefiner : SaveableTypeDefiner
//    {
//        // Base ID 576011: in TaleWorlds range but cannot be changed without breaking existing saves (#247)
//        // Dictionary<string, CampaignTime> is already defined in AoMSaveDefiner — omitted here (#244)
//        public HeroRaceMapSaveableTypeDefiner() : base(576011) { }
//        protected override void DefineContainerDefinitions()
//        {
//            ConstructContainerDefinition(typeof(Dictionary<string, string>));
//        }
//    }
//}
