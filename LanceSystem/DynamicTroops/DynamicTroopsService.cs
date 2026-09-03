using System.Collections.Generic;
using LanceSystem.Common;
using TaleWorlds.CampaignSystem;
using TaleWorlds.ObjectSystem;

namespace LanceSystem.DynamicTroops
{
    public class DynamicTroopsService
    {
        static DynamicTroopsService? _instance;
        public static DynamicTroopsService Instance => _instance ??= new DynamicTroopsService(new DynamicTroopsXmlFactory());
        readonly HashSet<string> _dynamicIds = new();
        readonly IDynamicTroopsFactory _factory;
        public DynamicTroopsService(IDynamicTroopsFactory factory)
        {
            _factory = factory;
        }
        public void MarkDynamic(string stringId)
        {
            _dynamicIds.Add(stringId);
        }

        public bool IsDynamic(string stringId)
        {
            return _dynamicIds.Contains(stringId);
        }

        public IReadOnlyCollection<string> DynamicIds => _dynamicIds;

        public void Reset()
        {
            _dynamicIds.Clear();
        }

        public Result<CharacterObject> SaveCharacterFromData(TroopEditorData data)
        {
            bool exists = MBObjectManager.Instance.GetObject<CharacterObject>(data.Name) != null;
            if (exists && !IsDynamic(data.Name))
                return Result<CharacterObject>.Fail("non dynamic troop with the same id already exists.");
            if (exists)
            {
                _factory.UpdateCharacterFromData(data);
                return Result<CharacterObject>.Ok(MBObjectManager.Instance.GetObject<CharacterObject>(data.Name));
            }
            return Result<CharacterObject>.Ok(_factory.CreateCharacterFromData(data));
        }
    }
}
