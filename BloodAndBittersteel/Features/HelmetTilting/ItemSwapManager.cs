using System.Collections.Generic;

namespace BloodAndBittersteel.Features.HelmetTilting
{
    internal class ItemSwapManager
    {
        private static readonly string _path = PathHelper.BaBOutsideConfigPath + "item_swap.xml";
        private static ItemSwapManager? _instance;
        public static ItemSwapManager Instance => _instance ??= new ItemSwapManager();
        public IReadOnlyList<ItemSwap> Swaps { get; private set; } = new List<ItemSwap>();
        public void LoadFromFile()
        {
            Swaps = ItemSwapDeserializer.LoadFromFile(_path);
        }

    }
}
