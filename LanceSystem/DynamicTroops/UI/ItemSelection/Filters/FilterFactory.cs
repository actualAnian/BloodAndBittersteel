using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.CampaignSystem;
using LanceSystem.DynamicTroops.UI.ItemSelection.Filters.TierFilters;
using LanceSystem.DynamicTroops.UI.ItemSelection.Filters.WeaponTypeFilters;
using LanceSystem.DynamicTroops.UI.ItemSelection.Filters.CultureFilters;
using LanceSystem.DynamicTroops.UI.ItemSelection.Filters.NameFilters;
using LanceSystem.DynamicTroops.UI.ItemSelection.Filters.ArmourMaterialFilters;
using LanceSystem.DynamicTroops.UI.ItemSelection.Filters.CharacterFilters;

namespace LanceSystem.DynamicTroops.UI.ItemSelection.Filters
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
        // filters are reversed when run, see ObjectSelectorController.GetFilteredItems

        public static List<FilterDefinition<ItemObject>> CreateEquipmentFilters()
        {
            return new()
            {
                CreateItemNameFilter(),
                CreateItemTierFilter(),
                CreateWeaponTypeFilter(),
                CreateItemCultureFilter(),
                CreateArmourMaterialFilter(),
            };
        }

        public static List<FilterDefinition<CharacterObject>> CreateCharacterFilters()
        {
            return new()
            {
                CreateCharacterNameFilter(),
                CreateCharacterTierFilter(),
                CreateCharacterCultureFilter(),
            };
        }

        static FilterDefinition<T> CreateFilter<TContext, T>(TContext context, IDataFilter<T> filter) where TContext : IFilterContext
        {
            var vm = new FilterViewModel(context);
            context.OnChanged += vm.Refresh;
            context.OnReset += vm.Refresh;
            return new FilterDefinition<T>(filter, vm, context);
        }

        static FilterDefinition<ItemObject> CreateItemTierFilter()
        {
            var context = new TierContext();
            return CreateFilter(context, new TierFilter(context));
        }

        static FilterDefinition<ItemObject> CreateWeaponTypeFilter()
        {
            var context = new WeaponTypeContext();
            return CreateFilter(context, new WeaponTypeFilter(context));
        }

        static FilterDefinition<ItemObject> CreateItemCultureFilter()
        {
            var context = new CultureContext();
            return CreateFilter(context, new ItemCultureFilter(context));
        }

        static FilterDefinition<ItemObject> CreateItemNameFilter()
        {
            var context = new NameContext();
            return CreateFilter(context, new NameFilter<ItemObject>(context, item => item.Name.ToString() ?? ""));
        }

        static FilterDefinition<ItemObject> CreateArmourMaterialFilter()
        {
            var context = new ArmourMaterialContext();
            return CreateFilter(context, new ArmourMaterialFilter(context));
        }

        static FilterDefinition<CharacterObject> CreateCharacterTierFilter()
        {
            var context = new CharacterTierContext();
            return CreateFilter(context, new CharacterTierFilter(context));
        }

        static FilterDefinition<CharacterObject> CreateCharacterCultureFilter()
        {
            var context = new CharacterCultureContext();
            return CreateFilter(context, new CharacterCultureFilter(context));
        }

        static FilterDefinition<CharacterObject> CreateCharacterNameFilter()
        {
            var context = new CharacterNameContext();
            return CreateFilter(context, new CharacterNameFilter(context, c => c.Name.ToString() ?? ""));
        }
    }
}
