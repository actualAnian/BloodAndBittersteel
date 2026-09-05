using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.CampaignSystem;
using LanceSystem.UI.ItemSelection.Filters.NameFilters;
using LanceSystem.UI.ItemSelection.Filters.CharacterTypeFilters;
using LanceSystem.UI.ItemSelection.Filters.TierFilters;
using LanceSystem.UI.ItemSelection.Filters.WeaponTypeFilters;
using LanceSystem.UI.ItemSelection.Filters.CultureFilters;
using LanceSystem.UI.ItemSelection.Filters.ArmourMaterialFilters;

namespace LanceSystem.UI.ItemSelection.Filters
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

        public static List<FilterDefinition<ItemObject>> CreateArmorFilters()
        {
            return new()
            {
                CreateItemNameFilter(),
                CreateItemTierFilter(),
                CreateItemCultureFilter(),
                CreateArmourMaterialFilter(),
            };
        }
        public static List<FilterDefinition<ItemObject>> CreateWeaponFilters()
        {
            return new()
            {
                CreateItemNameFilter(),
                CreateItemTierFilter(),
                CreateWeaponTypeFilter(),
                CreateItemCultureFilter(),
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
        public static List<FilterDefinition<CharacterObject>> CreateCharacterFiltersWithOccupation()
        {
            var filters = CreateCharacterFilters();
            filters.Add(CreateCharacterTypeFilter());
            return filters;
        }

        static FilterDefinition<T> CreateFilter<TContext, T>(TContext context, IDataFilter<T> filter) where TContext : IFilterContext
        {
            var vm = new FilterViewModel(context);
            context.OnChanged += vm.Refresh;
            context.OnReset += vm.Refresh;
            return new FilterDefinition<T>(filter, vm, context);
        }
        static FilterDefinition<CharacterObject> CreateCharacterTypeFilter()
        {
            var context = new CharacterTypeContext();
            return CreateFilter(context, new CharacterTypeFilter(context));
        }
        static FilterDefinition<ItemObject> CreateItemTierFilter()
        {
            var context = new TierContext();
            return CreateFilter(context, new TierFilter<ItemObject>(context, item => (int)item.Tier));
        }

        static FilterDefinition<ItemObject> CreateWeaponTypeFilter()
        {
            var context = new WeaponTypeContext();
            return CreateFilter(context, new WeaponTypeFilter(context));
        }

        static FilterDefinition<ItemObject> CreateItemCultureFilter()
        {
            var context = new CultureContext();
            return CreateFilter(context, new CultureFilter<ItemObject>(context, item => item.Culture as CultureObject));
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
            var context = new TierContext();
            return CreateFilter(context, new TierFilter<CharacterObject>(context, c => c.Tier));
        }

        static FilterDefinition<CharacterObject> CreateCharacterCultureFilter()
        {
            var context = new CultureContext();
            return CreateFilter(context, new CultureFilter<CharacterObject>(context, c => c.Culture as CultureObject));
        }

        static FilterDefinition<CharacterObject> CreateCharacterNameFilter()
        {
            var context = new NameContext();
            return CreateFilter(context, new NameFilter<CharacterObject>(context, c => c.Name.ToString() ?? ""));
        }
    }
}
