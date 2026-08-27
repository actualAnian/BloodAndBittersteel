using LanceSystem.DynamicTroops.TroopCreation.ItemSelection.Filters.TierFilters;
using LanceSystem.DynamicTroops.TroopCreation.ItemSelection.Filters.WeaponTypeFilters;
using LanceSystem.DynamicTroops.TroopCreation.ItemSelection.Filters.CultureFilters;
using LanceSystem.DynamicTroops.TroopCreation.ItemSelection.Filters.NameFilters;
using LanceSystem.DynamicTroops.TroopCreation.ItemSelection.Filters.ArmourMaterialFilters;
using LanceSystem.DynamicTroops.TroopCreation.ItemSelection.Filters.CharacterFilters;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.CampaignSystem;

namespace LanceSystem.DynamicTroops.TroopCreation.ItemSelection.Filters
{
    public static class FilterFactory
    {
        public sealed class FilterDefinition<TData>
        {
            public IDataFilter<TData> Filter { get; }
            public FilterViewModel ViewModel { get; }
            public IFilterContext Context { get; }

            public FilterDefinition(IDataFilter<TData> filter, FilterViewModel viewModel, IFilterContext context)
            {
                Filter = filter;
                ViewModel = viewModel;
                Context = context;
            }
        }

        public static List<FilterDefinition<ItemObject>> CreateEquipmentFilters()
        {
            return new()
            {
                CreateItemTierFilter(),
                CreateWeaponTypeFilter(),
                CreateItemCultureFilter(),
                CreateArmourMaterialFilter(),
                CreateItemNameFilter()
            };
        }

        public static List<FilterDefinition<CharacterObject>> CreateCharacterFilters()
        {
            return new()
            {
                CreateCharacterTierFilter(),
                CreateCharacterCultureFilter(),
                CreateCharacterNameFilter()
            };
        }

        static FilterDefinition<ItemObject> CreateItemTierFilter()
        {
            var context = new TierContext();
            var filter = new TierFilter(context);
            var vm = new FilterViewModel(context);
            return new FilterDefinition<ItemObject>(filter, vm, context);
        }

        static FilterDefinition<ItemObject> CreateWeaponTypeFilter()
        {
            var context = new WeaponTypeContext();
            var filter = new WeaponTypeFilter(context);
            var vm = new FilterViewModel(context);
            return new FilterDefinition<ItemObject>(filter, vm, context);
        }

        static FilterDefinition<ItemObject> CreateItemCultureFilter()
        {
            var context = new CultureContext();
            var filter = new ItemCultureFilter(context);
            var vm = new FilterViewModel(context);
            return new FilterDefinition<ItemObject>(filter, vm, context);
        }

        static FilterDefinition<ItemObject> CreateItemNameFilter()
        {
            var context = new NameContext();
            var filter = new NameFilter<ItemObject>(context, item => item?.Name?.ToString() ?? "");
            var vm = new FilterViewModel(context);
            return new FilterDefinition<ItemObject>(filter, vm, context);
        }

        static FilterDefinition<ItemObject> CreateArmourMaterialFilter()
        {
            var context = new ArmourMaterialContext();
            var filter = new ArmourMaterialFilter(context);
            var vm = new FilterViewModel(context);
            return new FilterDefinition<ItemObject>(filter, vm, context);
        }

        static FilterDefinition<CharacterObject> CreateCharacterTierFilter()
        {
            var context = new CharacterTierContext();
            var filter = new CharacterTierFilter(context);
            var vm = new FilterViewModel(context);
            return new FilterDefinition<CharacterObject>(filter, vm, context);
        }

        static FilterDefinition<CharacterObject> CreateCharacterCultureFilter()
        {
            var context = new CharacterCultureContext();
            var filter = new CharacterCultureFilter(context);
            var vm = new FilterViewModel(context);
            return new FilterDefinition<CharacterObject>(filter, vm, context);
        }

        static FilterDefinition<CharacterObject> CreateCharacterNameFilter()
        {
            var context = new CharacterNameContext();
            var filter = new CharacterNameFilter(context, c => c?.Name?.ToString() ?? "");
            var vm = new FilterViewModel(context);
            return new FilterDefinition<CharacterObject>(filter, vm, context);
        }
    }
}
